using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agenda.Colaboradores
{
    public partial class AgendaColaborador : Page
    {
        // horarios están en bloques de 60 min
        private const int MINUTOS_SLOT = 60;

        private enum ModoVista
        {
            Hoy,
            Semana,
            Mes
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null || Session["Rol"] == null)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (Convert.ToInt32(Session["Rol"]) != 2)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ViewState["Modo"] = ModoVista.Hoy.ToString();
                CargarAgenda();
            }
        }

        protected void btnHoy_Click(object sender, EventArgs e)
        {
            ViewState["Modo"] = ModoVista.Hoy.ToString();
            CargarAgenda();
        }

        protected void btnSemana_Click(object sender, EventArgs e)
        {
            ViewState["Modo"] = ModoVista.Semana.ToString();
            CargarAgenda();
        }

        protected void btnMes_Click(object sender, EventArgs e)
        {
            ViewState["Modo"] = ModoVista.Mes.ToString();
            CargarAgenda();
        }

        private ModoVista GetModo()
        {
            string v = Convert.ToString(ViewState["Modo"]);
            if (string.Equals(v, ModoVista.Semana.ToString(), StringComparison.OrdinalIgnoreCase)) return ModoVista.Semana;
            if (string.Equals(v, ModoVista.Mes.ToString(), StringComparison.OrdinalIgnoreCase)) return ModoVista.Mes;
            return ModoVista.Hoy;
        }

        private void SetBotonesActivos(ModoVista modo)
        {
            btnHoy.CssClass = "btn-tab" + (modo == ModoVista.Hoy ? " activo" : "");
            btnSemana.CssClass = "btn-tab" + (modo == ModoVista.Semana ? " activo" : "");
            btnMes.CssClass = "btn-tab" + (modo == ModoVista.Mes ? " activo" : "");
        }

        private void ObtenerRango(ModoVista modo, out DateTime desde, out DateTime hasta)
        {
            DateTime hoy = DateTime.Now.Date;

            if (modo == ModoVista.Hoy)
            {
                desde = hoy;
                hasta = hoy.AddDays(1); // exclusivo
                return;
            }

            if (modo == ModoVista.Semana)
            {
                // semana lunes-domingo
                int diff = ((int)hoy.DayOfWeek + 6) % 7; // lunes=0
                DateTime lunes = hoy.AddDays(-diff);
                DateTime domingoMasUno = lunes.AddDays(7);

                desde = lunes;
                hasta = domingoMasUno; // exclusivo
                return;
            }

            // Mes
            DateTime primero = new DateTime(hoy.Year, hoy.Month, 1);
            DateTime primeroSiguiente = primero.AddMonths(1);

            desde = primero;
            hasta = primeroSiguiente; // exclusivo
        }

        private void CargarAgenda()
        {
            lblMsg.Text = "";
            lblVacio.Text = "";

            int colaboradorId = Convert.ToInt32(Session["UsuarioID"]);
            ConexionBD bd = new ConexionBD();

            ModoVista modo = GetModo();
            SetBotonesActivos(modo);

            ObtenerRango(modo, out DateTime desde, out DateTime hasta);

            CultureInfo clCulture = new CultureInfo("es-CL");
            if (modo == ModoVista.Hoy)
            {
                lblRango.Text = DateTime.Now.ToString("dddd dd 'de' MMMM yyyy", clCulture);
            }
            else if (modo == ModoVista.Semana)
            {
                DateTime finIncl = hasta.AddDays(-1);
                lblRango.Text = "Semana: " + desde.ToString("dd MMM", clCulture) + " - " + finIncl.ToString("dd MMM", clCulture);
            }
            else
            {
                lblRango.Text = DateTime.Now.ToString("MMMM yyyy", clCulture);
            }

            string sql = @"
SELECT
    ci.CitaID,
    ci.FechaHora,
    ci.Estado,
    s.NombreServicio AS Servicio,
    (cl.Nombre + ' ' + cl.Apellido) AS Cliente,
    ISNULL(a.Observacion, '') AS Observacion,
    CASE WHEN a.AtencionID IS NULL THEN 0 ELSE 1 END AS TieneAtencion
FROM dbo.Citas ci
INNER JOIN dbo.Servicios s ON s.ServicioID = ci.ServicioID
INNER JOIN dbo.Clientes cl ON cl.UsuarioID = ci.UsuarioID_Cliente
LEFT JOIN dbo.Atenciones a ON a.CitaID = ci.CitaID
WHERE ci.UsuarioID_Colaborador = @Colaborador
  AND ci.FechaHora >= @Desde
  AND ci.FechaHora <  @Hasta
ORDER BY ci.FechaHora ASC;";

            SqlParameter[] p =
            {
                new SqlParameter("@Colaborador", colaboradorId),
                new SqlParameter("@Desde", desde),
                new SqlParameter("@Hasta", hasta)
            };

            DataTable dt = bd.EjecutarConsulta(sql, p);

            if (dt.Rows.Count == 0)
            {
                rptCitas.DataSource = null;
                rptCitas.DataBind();
                lblVacio.Text = "No hay citas en este rango 💖";
                return;
            }

            if (!dt.Columns.Contains("FechaHoraTexto")) dt.Columns.Add("FechaHoraTexto", typeof(string));
            if (!dt.Columns.Contains("BadgeClass")) dt.Columns.Add("BadgeClass", typeof(string));
            if (!dt.Columns.Contains("PuedeConfirmar")) dt.Columns.Add("PuedeConfirmar", typeof(bool));
            if (!dt.Columns.Contains("PuedeCancelar")) dt.Columns.Add("PuedeCancelar", typeof(bool));
            if (!dt.Columns.Contains("PuedeAtender")) dt.Columns.Add("PuedeAtender", typeof(bool));
            if (!dt.Columns.Contains("PuedeEditarObs")) dt.Columns.Add("PuedeEditarObs", typeof(bool));

            foreach (DataRow row in dt.Rows)
            {
                DateTime fh = Convert.ToDateTime(row["FechaHora"]);
                string estado = Convert.ToString(row["Estado"]);
                bool tieneAtencion = Convert.ToInt32(row["TieneAtencion"]) == 1;

                row["FechaHoraTexto"] = fh.ToString("dddd dd 'de' MMMM yyyy - HH:mm", clCulture);
                row["BadgeClass"] = GetBadgeClass(estado);

                bool esFutura = fh > DateTime.Now;
                bool esPasadaOAhora = fh <= DateTime.Now;

                bool pendiente = string.Equals(estado, "Pendiente", StringComparison.OrdinalIgnoreCase);
                bool confirmada = string.Equals(estado, "Confirmada", StringComparison.OrdinalIgnoreCase);
                bool atendida = string.Equals(estado, "Atendida", StringComparison.OrdinalIgnoreCase);

                row["PuedeConfirmar"] = pendiente;
                row["PuedeCancelar"] = pendiente && esFutura;

                // Atender: confirmada y ya llegó la hora (o pasó)
                row["PuedeAtender"] = confirmada && esPasadaOAhora;

                // Editar observación: si está atendida o tiene atencion guardada
                row["PuedeEditarObs"] = atendida || tieneAtencion;

                // Si está atendida pero por algún motivo no tiene atención, igual dejaremos el panel visible?:
                // En el aspx el panel se muestra por TieneAtencion, así que dejamos TieneAtencion como 1 si está atendida.
                if (atendida && !tieneAtencion)
                {
                    row["TieneAtencion"] = 1; // para que muestre el bloque y puedas entrar a editar/crear observación
                    if (string.IsNullOrWhiteSpace(Convert.ToString(row["Observacion"])))
                        row["Observacion"] = "(Sin observación registrada)";
                }
            }

            rptCitas.DataSource = dt;
            rptCitas.DataBind();
        }

        private string GetBadgeClass(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return "b-pendiente";

            switch (estado.Trim().ToLower())
            {
                case "pendiente": return "b-pendiente";
                case "confirmada": return "b-confirmada";
                case "cancelada": return "b-cancelada";
                case "atendida": return "b-atendida";
                default: return "b-pendiente";
            }
        }

        protected void rptCitas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Text = "";

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int citaId))
                return;

            if (e.CommandName == "confirmar")
            {
                ConfirmarCita(citaId);
                CargarAgenda();
                return;
            }

            if (e.CommandName == "cancelar")
            {
                CancelarCitaYLiberarSlots(citaId);
                CargarAgenda();
                return;
            }

            if (e.CommandName == "atender")
            {
                Response.Redirect("~/Colaboradores/AtenderCita.aspx?citaId=" + citaId);
                return;
            }

            if (e.CommandName == "editarObs")
            {
                // reutilizamos el mismo form (en AtenderCita tú decides si edita o re-graba)
                Response.Redirect("~/Colaboradores/AtenderCita.aspx?citaId=" + citaId + "&edit=1");
                return;
            }
        }

        private void ConfirmarCita(int citaId)
        {
            try
            {
                int colaboradorId = Convert.ToInt32(Session["UsuarioID"]);
                ConexionBD bd = new ConexionBD();

                string sql = @"
UPDATE dbo.Citas
SET Estado = 'Confirmada'
WHERE CitaID = @CitaID
  AND UsuarioID_Colaborador = @Colaborador
  AND Estado = 'Pendiente';";

                SqlParameter[] p =
                {
                    new SqlParameter("@CitaID", citaId),
                    new SqlParameter("@Colaborador", colaboradorId),
                };

                bd.EjecutarComando(sql, p);
                lblMsg.Text = "Cita confirmada ✅";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "No se pudo confirmar. Detalle: " + ex.Message;
            }
        }

        private void CancelarCitaYLiberarSlots(int citaId)
        {
            try
            {
                int colaboradorId = Convert.ToInt32(Session["UsuarioID"]);
                ConexionBD bd = new ConexionBD();

                string sqlGet = @"
SELECT TOP 1
    ci.CitaID,
    ci.Estado,
    ci.FechaHora,
    ci.HorarioID,
    ci.ServicioID,
    s.DuracionMin,
    h.Fecha,
    h.HoraInicio
FROM dbo.Citas ci
INNER JOIN dbo.Servicios s ON s.ServicioID = ci.ServicioID
INNER JOIN dbo.Horarios h ON h.HorarioID = ci.HorarioID
WHERE ci.CitaID = @CitaID
  AND ci.UsuarioID_Colaborador = @Colaborador;";

                SqlParameter[] pGet =
                {
                    new SqlParameter("@CitaID", citaId),
                    new SqlParameter("@Colaborador", colaboradorId),
                };

                DataTable dt = bd.EjecutarConsulta(sqlGet, pGet);

                if (dt.Rows.Count == 0)
                {
                    lblMsg.Text = "No se encontró la cita.";
                    return;
                }

                string estado = Convert.ToString(dt.Rows[0]["Estado"]);
                DateTime fechaHora = Convert.ToDateTime(dt.Rows[0]["FechaHora"]);

                if (!string.Equals(estado, "Pendiente", StringComparison.OrdinalIgnoreCase) || fechaHora <= DateTime.Now)
                {
                    lblMsg.Text = "Esta cita ya no se puede cancelar.";
                    return;
                }

                int duracionMin = Convert.ToInt32(dt.Rows[0]["DuracionMin"]);
                DateTime fecha = Convert.ToDateTime(dt.Rows[0]["Fecha"]).Date;
                TimeSpan horaInicio = (TimeSpan)dt.Rows[0]["HoraInicio"];

                int slotsNecesarios = (int)Math.Ceiling(duracionMin / (double)MINUTOS_SLOT);

                string sqlCancel = @"
UPDATE dbo.Citas
SET Estado = 'Cancelada'
WHERE CitaID = @CitaID;";

                SqlParameter[] pCancel = { new SqlParameter("@CitaID", citaId) };
                bd.EjecutarComando(sqlCancel, pCancel);

                for (int i = 0; i < slotsNecesarios; i++)
                {
                    TimeSpan horaSlot = horaInicio.Add(TimeSpan.FromMinutes(MINUTOS_SLOT * i));

                    string sqlFree = @"
UPDATE dbo.Horarios
SET Disponible = 1
WHERE Fecha = @Fecha
  AND UsuarioID_Colaborador = @Colaborador
  AND HoraInicio = @HoraInicio;";

                    SqlParameter[] pFree =
                    {
                        new SqlParameter("@Fecha", fecha),
                        new SqlParameter("@Colaborador", colaboradorId),
                        new SqlParameter("@HoraInicio", horaSlot),
                    };

                    bd.EjecutarComando(sqlFree, pFree);
                }

                lblMsg.Text = "Cita cancelada ✅ y los horarios fueron liberados 💖";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "No se pudo cancelar. Detalle: " + ex.Message;
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }
    }
}