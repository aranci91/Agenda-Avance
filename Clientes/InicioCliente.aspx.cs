using Agenda.Servicios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Agenda
{
    public partial class InicioCliente : System.Web.UI.Page
    {
        //horarios están creados por bloques de 60 minutos 
        private const int MINUTOS_SLOT = 60;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Seguridad
            if (Session["UsuarioID"] == null || Session["RolID"] == null)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            // solo cliente (RolID = 1)
            if (Convert.ToInt32(Session["RolID"]) != 1)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarNombreCliente();
                CargarResenasCarrusel(); // últimas 10 visibles
                CargarServicios();

                // Inicializar horas vacías
                ddlHora.Items.Clear();
                ddlHora.Items.Add(new ListItem("SELECCIONE HORA", ""));

                lblSinHoras.Text = "";
                lblDuracionServicio.Text = "-";
                lblPrecioServicio.Text = "-";

                lblMensajeCita.Text = "";
                lblMensajeResena.Text = "";
                lblMensaje.Text = "";
            }
        }

        // =========================
        // FECHA -> CARGAR HORAS
        // =========================
        protected void txtFecha_TextChanged(object sender, EventArgs e)
        {
            lblMensajeCita.Text = "";
            lblSinHoras.Text = "";

            ddlHora.Items.Clear();
            ddlHora.Items.Add(new ListItem("SELECCIONE HORA", ""));

            if (!DateTime.TryParse(txtFecha.Text, out DateTime fecha))
                return;

            CargarHorasDisponiblesInteligentes(fecha);
        }

        // =========================
        // HORAS INTELIGENTES
        // =========================
        private void CargarHorasDisponiblesInteligentes(DateTime fecha)
        {
            lblSinHoras.Text = "";
            lblMensajeCita.Text = "";

            ConexionBD bd = new ConexionBD();

            if (!int.TryParse(ddlServicios.SelectedValue, out int servicioId))
            {
                lblMensajeCita.Text = "Debes seleccionar un servicio.";
                return;
            }

            int usuarioColaboradorId = ObtenerColaboradorPorServicio(servicioId);
            if (usuarioColaboradorId == 0)
            {
                lblMensajeCita.Text = "Este servicio no tiene colaborador asignado";
                return;
            }

            // Duración del servicio
            int duracionMin = 60;
            ServicioInfo s = ObtenerServicio(servicioId);
            if (s != null) duracionMin = s.DuracionMin;

            int slotsNecesarios = (int)Math.Ceiling(duracionMin / (double)MINUTOS_SLOT);

            string sql = @"
SELECT HoraInicio
FROM dbo.Horarios
WHERE Disponible = 1
  AND Fecha = @Fecha
  AND UsuarioID_Colaborador = @Colaborador
ORDER BY HoraInicio;";

            SqlParameter[] p =
            {
                new SqlParameter("@Fecha", fecha.Date),
                new SqlParameter("@Colaborador", usuarioColaboradorId),
            };

            DataTable dt = bd.EjecutarConsulta(sql, p);

            if (dt.Rows.Count == 0)
            {
                lblSinHoras.Text = "Sin horas disponibles para este día 💖";
                return;
            }

            int agregadas = 0;

            foreach (DataRow row in dt.Rows)
            {
                TimeSpan horaInicio = TimeSpan.Parse(row["HoraInicio"].ToString());

                if (TieneContinuidad(bd, fecha.Date, usuarioColaboradorId, horaInicio, slotsNecesarios))
                {
                    string hora = horaInicio.ToString(@"hh\:mm");
                    ddlHora.Items.Add(new ListItem(hora, hora));
                    agregadas++;
                }
            }

            if (agregadas == 0)
            {
                lblSinHoras.Text = "Sin horas disponibles para este día según la duración del servicio 💖";
            }
        }

        private bool TieneContinuidad(ConexionBD bd, DateTime fecha, int colaboradorId, TimeSpan horaInicio, int slotsNecesarios)
        {
            for (int i = 0; i < slotsNecesarios; i++)
            {
                TimeSpan horaSlot = horaInicio.Add(TimeSpan.FromMinutes(MINUTOS_SLOT * i));

                string sqlSlot = @"
SELECT TOP 1 HorarioID
FROM dbo.Horarios
WHERE Fecha = @Fecha
  AND UsuarioID_Colaborador = @Colaborador
  AND HoraInicio = @HoraInicio
  AND Disponible = 1;";

                SqlParameter[] p =
                {
                    new SqlParameter("@Fecha", fecha.Date),
                    new SqlParameter("@Colaborador", colaboradorId),
                    new SqlParameter("@HoraInicio", horaSlot)
                };

                DataTable dt = bd.EjecutarConsulta(sqlSlot, p);

                if (dt.Rows.Count == 0)
                    return false;
            }

            return true;
        }

        // =========================
        // RESEÑAS: CARRUSEL (TOP 10)
        // =========================
        private void CargarResenasCarrusel()
        {
            lblMensajeResena.Text = "";
            lblSinResenas.Text = "";

            ConexionBD bd = new ConexionBD();

            // SOLO visibles. Solo nombre (sin apellido).
            string query = @"
SELECT TOP 10
    c.Nombre AS NombreCliente,
    r.Calificacion,
    ISNULL(r.Comentario,'') AS Comentario
FROM dbo.Reseñas r
INNER JOIN dbo.Atenciones a ON a.AtencionID = r.AtencionID
INNER JOIN dbo.Citas ci ON ci.CitaID = a.CitaID
INNER JOIN dbo.Clientes c ON c.UsuarioID = ci.UsuarioID_Cliente
WHERE r.Visible = 1
ORDER BY r.ReseñaID DESC;";

            DataTable dt = bd.EjecutarConsulta(query, null);

            if (dt.Rows.Count == 0)
            {
                rptResenas.DataSource = null;
                rptResenas.DataBind();
                lblSinResenas.Text = "Aún no hay valoraciones publicadas.";
                return;
            }

            rptResenas.DataSource = dt;
            rptResenas.DataBind();
        }

        protected void btnPublicarValoracion_Click(object sender, EventArgs e)
        {
            lblMensajeResena.Text = "";

            if (Session["UsuarioID"] == null)
            {
                lblMensajeResena.Text = "Debes iniciar sesión para publicar una valoración.";
                return;
            }

            int usuarioId = Convert.ToInt32(Session["UsuarioID"]);
            int calificacion = Convert.ToInt32(ddlCalificacion.SelectedValue);
            string comentario = (txtComentario.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(comentario))
            {
                lblMensajeResena.Text = "Escribe un comentario antes de publicar.";
                return;
            }

            // Tomar última atención del cliente que AÚN NO tenga reseña
            var data = ObtenerUltimaAtencionSinResena(usuarioId);

            int atencionId = data.AtencionID;
            int servicioId = data.ServicioID;

            if (atencionId == 0)
            {
                lblMensajeResena.Text = "No tienes atenciones pendientes de reseña (o ya reseñaste tu última atención). 💖";
                return;
            }

            try
            {
                ConexionBD bd = new ConexionBD();

                // Validación extra por si acaso (además del UNIQUE)
                string sqlExiste = @"
SELECT COUNT(*) AS Total
FROM dbo.Reseñas
WHERE AtencionID = @AtencionID;";

                SqlParameter[] pEx =
                {
                    new SqlParameter("@AtencionID", atencionId)
                };

                DataTable dtEx = bd.EjecutarConsulta(sqlExiste, pEx);
                int total = (dtEx.Rows.Count > 0) ? Convert.ToInt32(dtEx.Rows[0]["Total"]) : 0;

                if (total > 0)
                {
                    lblMensajeResena.Text = "Esa atención ya tiene una reseña registrada 💖";
                    return;
                }

                // Insert completo considerando NOT NULL: UsuarioID, ServicioID, Activo, Visible
                string insert = @"
INSERT INTO dbo.Reseñas (AtencionID, Calificacion, Comentario, ServicioID, UsuarioID, Activo, Visible)
VALUES (@AtencionID, @Calificacion, @Comentario, @ServicioID, @UsuarioID, 1, 1);";

                SqlParameter[] p =
                {
                    new SqlParameter("@AtencionID", atencionId),
                    new SqlParameter("@Calificacion", calificacion),
                    new SqlParameter("@Comentario", comentario),
                    new SqlParameter("@ServicioID", servicioId),
                    new SqlParameter("@UsuarioID", usuarioId),
                };

                bd.EjecutarComando(insert, p);

                txtComentario.Text = "";
                ddlCalificacion.SelectedValue = "5";

                lblMensajeResena.Text = "¡Gracias! Tu valoración fue publicada ✅";
                CargarResenasCarrusel();
            }
            catch (SqlException ex)
            {
                // Si igual chocara con el UNIQUE
                lblMensajeResena.Text = "Esa atención ya fue reseñada. 💖 Detalle: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblMensajeResena.Text = "No se pudo publicar la valoración. Detalle: " + ex.Message;
            }
        }

        private (int AtencionID, int ServicioID) ObtenerUltimaAtencionSinResena(int usuarioIdCliente)
        {
            ConexionBD bd = new ConexionBD();

            // Última atención del cliente cuyo AtencionID NO esté en Reseñas
            string query = @"
SELECT TOP 1
    a.AtencionID,
    ci.ServicioID
FROM dbo.Atenciones a
INNER JOIN dbo.Citas ci ON ci.CitaID = a.CitaID
LEFT JOIN dbo.Reseñas r ON r.AtencionID = a.AtencionID
WHERE ci.UsuarioID_Cliente = @UsuarioID
  AND r.ReseñaID IS NULL
ORDER BY a.AtencionID DESC;";

            SqlParameter[] p =
            {
                new SqlParameter("@UsuarioID", usuarioIdCliente)
            };

            DataTable dt = bd.EjecutarConsulta(query, p);
            if (dt.Rows.Count == 0) return (0, 0);

            return (Convert.ToInt32(dt.Rows[0]["AtencionID"]), Convert.ToInt32(dt.Rows[0]["ServicioID"]));
        }

        // =========================
        // NOMBRE CLIENTE
        // =========================
        private void CargarNombreCliente()
        {
            try
            {
                int usuarioId = Convert.ToInt32(Session["UsuarioID"]);
                ConexionBD bd = new ConexionBD();

                string query = @"
SELECT TOP 1 Nombre, Apellido
FROM dbo.Clientes
WHERE UsuarioID = @UsuarioID;";

                SqlParameter[] parametros =
                {
                    new SqlParameter("@UsuarioID", usuarioId)
                };

                DataTable dt = bd.EjecutarConsulta(query, parametros);

                if (dt.Rows.Count > 0)
                {
                    lblNombreCliente.Text = dt.Rows[0]["Nombre"] + " " + dt.Rows[0]["Apellido"];
                }
                else
                {
                    lblNombreCliente.Text = "CLIENTE";
                }
            }
            catch
            {
                lblNombreCliente.Text = "CLIENTE";
            }
        }

        // =========================
        // SERVICIOS
        // =========================
        private void CargarServicios()
        {
            ddlServicios.Items.Clear();
            ddlServicios.Items.Add(new ListItem("SELECCIONE SERVICIO", ""));

            ConexionBD bd = new ConexionBD();

            string query = @"
SELECT DISTINCT s.ServicioID, s.NombreServicio
FROM dbo.Servicios s
INNER JOIN dbo.ServiciosColaboradores sc ON sc.ServicioID = s.ServicioID
WHERE s.Activo = 1
  AND sc.Activo = 1
ORDER BY s.NombreServicio;";

            DataTable dt = bd.EjecutarConsulta(query, null);

            foreach (DataRow row in dt.Rows)
            {
                ddlServicios.Items.Add(
                    new ListItem(row["NombreServicio"].ToString(), row["ServicioID"].ToString())
                );
            }
        }

        private int ObtenerColaboradorPorServicio(int servicioId)
        {
            ConexionBD bd = new ConexionBD();

            string query = @"
SELECT TOP 1 UsuarioID_Colaborador
FROM dbo.ServiciosColaboradores
WHERE ServicioID = @ServicioID
  AND Activo = 1
ORDER BY UsuarioID_Colaborador;";

            SqlParameter[] p =
            {
                new SqlParameter("@ServicioID", servicioId)
            };

            DataTable dt = bd.EjecutarConsulta(query, p);

            if (dt.Rows.Count == 0)
                return 0;

            return Convert.ToInt32(dt.Rows[0]["UsuarioID_Colaborador"]);
        }

        protected void ddlServicios_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMensajeCita.Text = "";

            if (!int.TryParse(ddlServicios.SelectedValue, out int servicioId))
            {
                lblDuracionServicio.Text = "-";
                lblPrecioServicio.Text = "-";
                return;
            }

            ServicioInfo info = ObtenerServicio(servicioId);
            if (info == null)
            {
                lblDuracionServicio.Text = "-";
                lblPrecioServicio.Text = "-";
                return;
            }

            lblDuracionServicio.Text = info.DuracionMin + " min";
            lblPrecioServicio.Text = info.Precio.ToString("C0", new CultureInfo("es-CL"));

            if (DateTime.TryParse(txtFecha.Text, out DateTime fecha))
            {
                ddlHora.Items.Clear();
                ddlHora.Items.Add(new ListItem("SELECCIONE HORA", ""));
                lblSinHoras.Text = "";
                CargarHorasDisponiblesInteligentes(fecha);
            }
        }

        private ServicioInfo ObtenerServicio(int servicioId)
        {
            ConexionBD bd = new ConexionBD();

            string query = @"
SELECT TOP 1 ServicioID, NombreServicio, DuracionMin, Precio
FROM dbo.Servicios
WHERE ServicioID = @ServicioID
  AND Activo = 1;";

            SqlParameter[] p =
            {
                new SqlParameter("@ServicioID", servicioId)
            };

            DataTable dt = bd.EjecutarConsulta(query, p);
            if (dt.Rows.Count == 0) return null;

            return new ServicioInfo
            {
                ServicioID = Convert.ToInt32(dt.Rows[0]["ServicioID"]),
                NombreServicio = dt.Rows[0]["NombreServicio"].ToString(),
                DuracionMin = Convert.ToInt32(dt.Rows[0]["DuracionMin"]),
                Precio = Convert.ToDecimal(dt.Rows[0]["Precio"])
            };
        }

        // =========================
        // CONFIRMAR CITA (BLOQUEA HORARIOS)
        // =========================
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            lblMensajeCita.Text = "";
            lblSinHoras.Text = "";

            if (string.IsNullOrWhiteSpace(ddlServicios.SelectedValue))
            {
                lblMensajeCita.Text = "Debes seleccionar un servicio.";
                return;
            }

            if (!DateTime.TryParse(txtFecha.Text, out DateTime fechaSeleccionada))
            {
                lblMensajeCita.Text = "Debes seleccionar una fecha.";
                return;
            }

            if (ddlHora.Items.Count <= 1)
            {
                lblSinHoras.Text = "Sin horas disponibles para este día 💖";
                lblMensajeCita.Text = "Elige otra fecha con horas disponibles.";
                return;
            }

            if (string.IsNullOrWhiteSpace(ddlHora.SelectedValue))
            {
                lblMensajeCita.Text = "Debes seleccionar una hora.";
                return;
            }

            if (!int.TryParse(ddlServicios.SelectedValue, out int servicioId))
            {
                lblMensajeCita.Text = "Servicio inválido.";
                return;
            }

            ServicioInfo servicio = ObtenerServicio(servicioId);
            if (servicio == null)
            {
                lblMensajeCita.Text = "No se encontró el servicio.";
                return;
            }

            int usuarioClienteId = Convert.ToInt32(Session["UsuarioID"]);

            int usuarioColaboradorId = ObtenerColaboradorPorServicio(servicioId);
            if (usuarioColaboradorId == 0)
            {
                lblMensajeCita.Text = "Este servicio no tiene colaborador asignado 😭";
                return;
            }

            TimeSpan horaInicio = TimeSpan.Parse(ddlHora.SelectedValue);
            int slotsNecesarios = (int)Math.Ceiling(servicio.DuracionMin / (double)MINUTOS_SLOT);

            try
            {
                ConexionBD bd = new ConexionBD();

                int[] horarioIds = new int[slotsNecesarios];

                for (int i = 0; i < slotsNecesarios; i++)
                {
                    TimeSpan horaSlot = horaInicio.Add(TimeSpan.FromMinutes(MINUTOS_SLOT * i));

                    string sqlBuscarSlot = @"
SELECT TOP 1 HorarioID
FROM dbo.Horarios
WHERE Fecha = @Fecha
  AND UsuarioID_Colaborador = @Colaborador
  AND HoraInicio = @HoraInicio
  AND Disponible = 1;";

                    SqlParameter[] pSlot =
                    {
                        new SqlParameter("@Fecha", fechaSeleccionada.Date),
                        new SqlParameter("@Colaborador", usuarioColaboradorId),
                        new SqlParameter("@HoraInicio", horaSlot),
                    };

                    DataTable dtSlot = bd.EjecutarConsulta(sqlBuscarSlot, pSlot);

                    if (dtSlot.Rows.Count == 0)
                    {
                        lblMensajeCita.Text =
                            "Esa hora no tiene continuidad suficiente según la duración del servicio. Elige otra hora 💖";

                        ddlHora.Items.Clear();
                        ddlHora.Items.Add(new ListItem("SELECCIONE HORA", ""));
                        CargarHorasDisponiblesInteligentes(fechaSeleccionada);
                        return;
                    }

                    horarioIds[i] = Convert.ToInt32(dtSlot.Rows[0]["HorarioID"]);
                }

                DateTime fechaHora = fechaSeleccionada.Date.Add(horaInicio);

                string sqlInsertCita = @"
INSERT INTO dbo.Citas (UsuarioID_Cliente, UsuarioID_Colaborador, ServicioID, HorarioID, FechaHora, Estado)
VALUES (@Cliente, @Colaborador, @Servicio, @HorarioID, @FechaHora, @Estado);";

                SqlParameter[] pInsert =
                {
                    new SqlParameter("@Cliente", usuarioClienteId),
                    new SqlParameter("@Colaborador", usuarioColaboradorId),
                    new SqlParameter("@Servicio", servicioId),
                    new SqlParameter("@HorarioID", horarioIds[0]),
                    new SqlParameter("@FechaHora", fechaHora),
                    new SqlParameter("@Estado", "Pendiente")
                };

                bd.EjecutarComando(sqlInsertCita, pInsert);

                for (int i = 0; i < horarioIds.Length; i++)
                {
                    string sqlBloquear = @"
UPDATE dbo.Horarios
SET Disponible = 0
WHERE HorarioID = @HorarioID;";

                    SqlParameter[] pBloq =
                    {
                        new SqlParameter("@HorarioID", horarioIds[i])
                    };

                    bd.EjecutarComando(sqlBloquear, pBloq);
                }

                lblMensajeCita.Text = "¡Hora agendada con éxito! 💖";

                ddlServicios.SelectedIndex = 0;
                txtFecha.Text = "";

                ddlHora.Items.Clear();
                ddlHora.Items.Add(new ListItem("SELECCIONE HORA", ""));

                lblDuracionServicio.Text = "-";
                lblPrecioServicio.Text = "-";
                lblSinHoras.Text = "";
            }
            catch (SqlException)
            {
                lblMensajeCita.Text = "Esa hora ya fue tomada justo antes 😭 Elige otra porfis.";
                ddlHora.Items.Clear();
                ddlHora.Items.Add(new ListItem("SELECCIONE HORA", ""));
                CargarHorasDisponiblesInteligentes(fechaSeleccionada);
            }
            catch (Exception ex)
            {
                lblMensajeCita.Text = "No se pudo agendar. Detalle: " + ex.Message;
            }
        }

        // =========================
        // MENÚ
        // =========================
        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }

        protected void btnMisDatos_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Clientes/EditardatosCliente.aspx");
        }

        protected void btnHistorial_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Clientes/HistorialCitas.aspx");
        }

        // =========================
        // DTO
        // =========================
        private class ServicioInfo
        {
            public int ServicioID { get; set; }
            public string NombreServicio { get; set; }
            public int DuracionMin { get; set; }
            public decimal Precio { get; set; }
        }
    }
}