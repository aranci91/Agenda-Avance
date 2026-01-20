using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agenda.Clientes
{
    public partial class HistorialCitas : System.Web.UI.Page
    {
        // bloques en Horarios son de 60 minutos
        private const int MINUTOS_SLOT = 60;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Seguridad
            if (Session["UsuarioID"] == null || Session["RolID"] == null)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            // Solo cliente (RolID = 1)
            if (Convert.ToInt32(Session["RolID"]) != 1)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarHistorial();
            }
        }

        private void CargarHistorial()
        {
            lblMsg.Text = "";
            lblVacio.Text = "";

            int usuarioClienteId = Convert.ToInt32(Session["UsuarioID"]);
            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT
    ci.CitaID,
    ci.FechaHora,
    ci.Estado,
    s.NombreServicio AS Servicio,
    (co.Nombre + ' (' + co.Especialidad + ')') AS Colaborador
FROM dbo.Citas ci
INNER JOIN dbo.Servicios s ON s.ServicioID = ci.ServicioID
INNER JOIN dbo.Colaboradores co ON co.UsuarioID = ci.UsuarioID_Colaborador
WHERE ci.UsuarioID_Cliente = @Cliente
ORDER BY ci.FechaHora DESC;";

            SqlParameter[] p =
            {
                new SqlParameter("@Cliente", usuarioClienteId)
            };

            DataTable dt = bd.EjecutarConsulta(sql, p);

            if (dt.Rows.Count == 0)
            {
                rptCitas.DataSource = null;
                rptCitas.DataBind();
                lblVacio.Text = "Aún no tienes citas registradas 💖";
                return;
            }

            // Columnas extra para el repeater
            if (!dt.Columns.Contains("FechaHoraTexto"))
                dt.Columns.Add("FechaHoraTexto", typeof(string));
            if (!dt.Columns.Contains("BadgeClass"))
                dt.Columns.Add("BadgeClass", typeof(string));
            if (!dt.Columns.Contains("PuedeCancelar"))
                dt.Columns.Add("PuedeCancelar", typeof(bool));

            CultureInfo cl = new CultureInfo("es-CL");

            foreach (DataRow row in dt.Rows)
            {
                DateTime fh = Convert.ToDateTime(row["FechaHora"]);
                string estado = Convert.ToString(row["Estado"]);

                row["FechaHoraTexto"] = fh.ToString("dddd dd 'de' MMMM yyyy - HH:mm", cl);
                row["BadgeClass"] = GetBadgeClass(estado);

                bool esFutura = fh > DateTime.Now;
                bool pendiente = string.Equals(estado, "Pendiente", StringComparison.OrdinalIgnoreCase);
                row["PuedeCancelar"] = (pendiente && esFutura);
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

     
        // Cancelar cita (Pendiente y futura) + LIBERAR TODOS LOS SLOTS
        
        protected void rptCitas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "cancelar") return;

            lblMsg.Text = "";

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int citaId))
                return;

            int usuarioClienteId = Convert.ToInt32(Session["UsuarioID"]);
            ConexionBD bd = new ConexionBD();

         
            string sqlGet = @"
SELECT TOP 1
    ci.Estado,
    ci.FechaHora,
    ci.UsuarioID_Colaborador,
    s.DuracionMin,
    h.Fecha,
    h.HoraInicio
FROM dbo.Citas ci
INNER JOIN dbo.Servicios s ON s.ServicioID = ci.ServicioID
INNER JOIN dbo.Horarios h ON h.HorarioID = ci.HorarioID
WHERE ci.CitaID = @CitaID
  AND ci.UsuarioID_Cliente = @Cliente;";

            SqlParameter[] pGet =
            {
                new SqlParameter("@CitaID", citaId),
                new SqlParameter("@Cliente", usuarioClienteId)
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
                CargarHistorial();
                return;
            }

            int duracionMin = Convert.ToInt32(dt.Rows[0]["DuracionMin"]);
            int usuarioColaboradorId = Convert.ToInt32(dt.Rows[0]["UsuarioID_Colaborador"]);
            DateTime fecha = Convert.ToDateTime(dt.Rows[0]["Fecha"]);
            TimeSpan horaInicio = (TimeSpan)dt.Rows[0]["HoraInicio"];

            int slotsNecesarios = (int)Math.Ceiling(duracionMin / (double)MINUTOS_SLOT);

            try
            {
                // 1) Cancelar cita
                string sqlCancel = @"
UPDATE dbo.Citas
SET Estado = 'Cancelada'
WHERE CitaID = @CitaID;";

                SqlParameter[] pCancel =
                {
                    new SqlParameter("@CitaID", citaId)
                };

                bd.EjecutarComando(sqlCancel, pCancel);

                // 2) Liberar TODOS los slots que bloqueó el servicio
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
                        new SqlParameter("@Fecha", fecha.Date),
                        new SqlParameter("@Colaborador", usuarioColaboradorId),
                        new SqlParameter("@HoraInicio", horaSlot)
                    };

                    bd.EjecutarComando(sqlFree, pFree);
                }

                lblMsg.Text = "Cita cancelada ✅ y el horario volvió a quedar disponible 💖";
                CargarHistorial();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "No se pudo cancelar. Detalle: " + ex.Message;
            }
        }

       
        // MENÚ
       
        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }
    }
}