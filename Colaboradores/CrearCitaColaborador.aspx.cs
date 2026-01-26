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
    public partial class CrearCitaColaborador : System.Web.UI.Page
    {
        private const int MINUTOS_SLOT = 60;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Seguridad
            if (Session["UsuarioID"] == null || Session["Rol"] == null)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            // Solo colaborador (Rol = 2)
            if (Convert.ToInt32(Session["Rol"]) != 2)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                //  CAMBIO CLAVE: solo desde HOY en adelante (bloquea pasado desde UI)
                // Esto funciona cuando el input está en type='date' (al hacer focus)
                txtFecha.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");

                InicializarCombos();
                CargarServiciosActivos();
            }
        }

        private void InicializarCombos()
        {
            ddlClientes.Items.Clear();
            ddlClientes.Items.Add(new ListItem("SELECCIONE CLIENTE", ""));

            ddlServicios.Items.Clear();
            ddlServicios.Items.Add(new ListItem("SELECCIONE SERVICIO", ""));

            ddlHora.Items.Clear();
            ddlHora.Items.Add(new ListItem("SELECCIONE HORA", ""));

            lblMsgBusqueda.Text = "";
            lblClienteSeleccionado.Text = "";
            lblDuracionServicio.Text = "-";
            lblPrecioServicio.Text = "-";
            lblSinHoras.Text = "";
            lblMensajeCita.Text = "";
        }

      
        // 1) BUSCAR CLIENTE (RUT o nombre)
      
        protected void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            lblMsgBusqueda.Text = "";
            lblClienteSeleccionado.Text = "";
            lblMensajeCita.Text = "";

            ddlClientes.Items.Clear();
            ddlClientes.Items.Add(new ListItem("SELECCIONE CLIENTE", ""));

            string termino = (txtBuscarCliente.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(termino))
            {
                lblMsgBusqueda.Text = "Escribe un RUT o un nombre para buscar.";
                return;
            }

            try
            {
                ConexionBD bd = new ConexionBD();

                // Buscar por:
                // - Rut exacto o parcial
                // - Nombre o Apellido (LIKE)
                string sql = @"
SELECT TOP 20
    c.UsuarioID,
    (c.Nombre + ' ' + c.Apellido) AS NombreCompleto,
    u.Rut
FROM dbo.Clientes c
INNER JOIN dbo.Usuarios u ON u.UsuarioID = c.UsuarioID
WHERE u.Rut LIKE @TerminoRut
   OR (c.Nombre + ' ' + c.Apellido) LIKE @TerminoNombre
   OR c.Nombre LIKE @TerminoNombre
   OR c.Apellido LIKE @TerminoNombre
ORDER BY c.Nombre, c.Apellido;";

                // Normalización simple para comparar RUT sin puntos/espacios
                string terminoRut = termino.Replace(".", "").Replace(" ", "");
                string likeRut = "%" + terminoRut + "%";
                string likeNombre = "%" + termino + "%";

                SqlParameter[] p =
                {
                    new SqlParameter("@TerminoRut", likeRut),
                    new SqlParameter("@TerminoNombre", likeNombre),
                };

                DataTable dt = bd.EjecutarConsulta(sql, p);

                if (dt.Rows.Count == 0)
                {
                    lblMsgBusqueda.Text = "No se encontró cliente. Debe estar registrado.";
                    return;
                }

                foreach (DataRow row in dt.Rows)
                {
                    string usuarioId = Convert.ToString(row["UsuarioID"]);
                    string nombre = Convert.ToString(row["NombreCompleto"]);
                    string rut = Convert.ToString(row["Rut"]);

                    ddlClientes.Items.Add(new ListItem(nombre + " - " + rut, usuarioId));
                }

                lblMsgBusqueda.Text = "Selecciona el cliente encontrado en la lista 💖";
            }
            catch (Exception ex)
            {
                lblMsgBusqueda.Text = "No se pudo buscar. Detalle: " + ex.Message;
            }
        }

     
        // 2) SERVICIOS (solo activos)
   
        private void CargarServiciosActivos()
        {
            ddlServicios.Items.Clear();
            ddlServicios.Items.Add(new ListItem("SELECCIONE SERVICIO", ""));

            ConexionBD bd = new ConexionBD();

            string query = @"
SELECT ServicioID, NombreServicio
FROM dbo.Servicios
WHERE Activo = 1
ORDER BY NombreServicio;";

            DataTable dt = bd.EjecutarConsulta(query, null);

            foreach (DataRow row in dt.Rows)
            {
                ddlServicios.Items.Add(
                    new ListItem(row["NombreServicio"].ToString(), row["ServicioID"].ToString())
                );
            }
        }

        protected void ddlServicios_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMensajeCita.Text = "";
            lblSinHoras.Text = "";

            if (!int.TryParse(ddlServicios.SelectedValue, out int servicioId))
            {
                lblDuracionServicio.Text = "-";
                lblPrecioServicio.Text = "-";
                ResetHoras();
                return;
            }

            ServicioInfo info = ObtenerServicioActivo(servicioId);
            if (info == null)
            {
                lblDuracionServicio.Text = "-";
                lblPrecioServicio.Text = "-";
                ResetHoras();
                return;
            }

            lblDuracionServicio.Text = info.DuracionMin + " min";
            lblPrecioServicio.Text = info.Precio.ToString("C0", new CultureInfo("es-CL"));

            // Si ya hay fecha, recargar horas inteligentes
            if (DateTime.TryParse(txtFecha.Text, out DateTime fecha))
            {
                ResetHoras();
                CargarHorasDisponiblesInteligentes(fecha);
            }
        }

        private ServicioInfo ObtenerServicioActivo(int servicioId)
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
                Precio = Convert.ToInt32(dt.Rows[0]["Precio"])
            };
        }

        // 3) FECHA -> CARGAR HORAS INTELIGENTES
  
        protected void txtFecha_TextChanged(object sender, EventArgs e)
        {
            lblMensajeCita.Text = "";
            lblSinHoras.Text = "";

            ResetHoras();

            if (!DateTime.TryParse(txtFecha.Text, out DateTime fecha))
                return;

            // 🔴 VALIDACIÓN CLAVE: no permitir pasado
            if (fecha.Date < DateTime.Today)
            {
                lblMensajeCita.Text = "No puedes crear citas en el pasado. Selecciona desde hoy en adelante 💖";
                return;
            }

            CargarHorasDisponiblesInteligentes(fecha);
        }

        private void ResetHoras()
        {
            ddlHora.Items.Clear();
            ddlHora.Items.Add(new ListItem("SELECCIONE HORA", ""));
        }

        private void CargarHorasDisponiblesInteligentes(DateTime fecha)
        {
            lblSinHoras.Text = "";

            // Requisitos mínimos
            if (!int.TryParse(ddlServicios.SelectedValue, out int servicioId))
            {
                lblSinHoras.Text = "Primero selecciona un servicio.";
                return;
            }

            ServicioInfo s = ObtenerServicioActivo(servicioId);
            if (s == null)
            {
                lblSinHoras.Text = "Servicio inválido o desactivado.";
                return;
            }

            int duracionMin = s.DuracionMin;
            int slotsNecesarios = (int)Math.Ceiling(duracionMin / (double)MINUTOS_SLOT);

            int colaboradorId = Convert.ToInt32(Session["UsuarioID"]); // 🔴 colaborador logueado

            ConexionBD bd = new ConexionBD();

            // Traer horas disponibles de ese día para ESTE colaborador
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
                new SqlParameter("@Colaborador", colaboradorId),
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

                if (TieneContinuidad(bd, fecha.Date, colaboradorId, horaInicio, slotsNecesarios))
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

    
        // 4) CONFIRMAR CITA (INSERT + BLOQUEO)
     
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            lblMensajeCita.Text = "";
            lblSinHoras.Text = "";

            // Cliente seleccionado
            if (!int.TryParse(ddlClientes.SelectedValue, out int usuarioClienteId))
            {
                lblMensajeCita.Text = "Debes buscar y seleccionar un cliente registrado.";
                return;
            }

            // Servicio
            if (!int.TryParse(ddlServicios.SelectedValue, out int servicioId))
            {
                lblMensajeCita.Text = "Debes seleccionar un servicio.";
                return;
            }

            ServicioInfo servicio = ObtenerServicioActivo(servicioId);
            if (servicio == null)
            {
                lblMensajeCita.Text = "Servicio inválido o desactivado.";
                return;
            }

            // Fecha
            if (!DateTime.TryParse(txtFecha.Text, out DateTime fechaSeleccionada))
            {
                lblMensajeCita.Text = "Debes seleccionar una fecha.";
                return;
            }

            // VALIDACIÓN CLAVE: no permitir pasado
            if (fechaSeleccionada.Date < DateTime.Today)
            {
                lblMensajeCita.Text = "No puedes crear citas en el pasado. Selecciona desde hoy en adelante 💖";
                return;
            }

            // Hora
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

            int colaboradorId = Convert.ToInt32(Session["UsuarioID"]); //  colaborador logueado
            TimeSpan horaInicio = TimeSpan.Parse(ddlHora.SelectedValue);

            int slotsNecesarios = (int)Math.Ceiling(servicio.DuracionMin / (double)MINUTOS_SLOT);

            try
            {
                ConexionBD bd = new ConexionBD();

                using (SqlConnection cn = bd.ObtenerConexion())
                {
                    cn.Open();

                    using (SqlTransaction tx = cn.BeginTransaction())
                    {
                        try
                        {
                            // 1) Validar TODOS los slots y guardar HorarioIDs
                            int[] horarioIds = new int[slotsNecesarios];

                            for (int i = 0; i < slotsNecesarios; i++)
                            {
                                TimeSpan horaSlot = horaInicio.Add(TimeSpan.FromMinutes(MINUTOS_SLOT * i));

                                using (SqlCommand cmdSlot = new SqlCommand(@"
SELECT TOP 1 HorarioID
FROM dbo.Horarios
WHERE Fecha = @Fecha
  AND UsuarioID_Colaborador = @Colaborador
  AND HoraInicio = @HoraInicio
  AND Disponible = 1;", cn, tx))
                                {
                                    cmdSlot.Parameters.AddWithValue("@Fecha", fechaSeleccionada.Date);
                                    cmdSlot.Parameters.AddWithValue("@Colaborador", colaboradorId);
                                    cmdSlot.Parameters.AddWithValue("@HoraInicio", horaSlot);

                                    object r = cmdSlot.ExecuteScalar();
                                    if (r == null)
                                    {
                                        throw new Exception("La hora seleccionada ya no tiene continuidad suficiente.");
                                    }

                                    horarioIds[i] = Convert.ToInt32(r);
                                }
                            }

                            // 2) Insertar la cita (HorarioID = primer bloque)
                            DateTime fechaHora = fechaSeleccionada.Date.Add(horaInicio);

                            using (SqlCommand cmdIns = new SqlCommand(@"
INSERT INTO dbo.Citas (UsuarioID_Cliente, UsuarioID_Colaborador, ServicioID, HorarioID, FechaHora, Estado)
VALUES (@Cliente, @Colaborador, @Servicio, @HorarioID, @FechaHora, @Estado);", cn, tx))
                            {
                                cmdIns.Parameters.AddWithValue("@Cliente", usuarioClienteId);
                                cmdIns.Parameters.AddWithValue("@Colaborador", colaboradorId);
                                cmdIns.Parameters.AddWithValue("@Servicio", servicioId);
                                cmdIns.Parameters.AddWithValue("@HorarioID", horarioIds[0]);
                                cmdIns.Parameters.AddWithValue("@FechaHora", fechaHora);
                                cmdIns.Parameters.AddWithValue("@Estado", "Pendiente");

                                cmdIns.ExecuteNonQuery();
                            }

                            // 3) Bloquear slots
                            for (int i = 0; i < horarioIds.Length; i++)
                            {
                                using (SqlCommand cmdBloq = new SqlCommand(@"
UPDATE dbo.Horarios
SET Disponible = 0
WHERE HorarioID = @HorarioID;", cn, tx))
                                {
                                    cmdBloq.Parameters.AddWithValue("@HorarioID", horarioIds[i]);
                                    cmdBloq.ExecuteNonQuery();
                                }
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                lblMensajeCita.Text = "¡Cita creada con éxito! 💖";

                // Limpiar formulario (pero mantenemos lo buscado si quieres, aquí lo dejo limpio)
                ddlServicios.SelectedIndex = 0;
                txtFecha.Text = "";
                ResetHoras();
                lblDuracionServicio.Text = "-";
                lblPrecioServicio.Text = "-";
                lblSinHoras.Text = "";
            }
            catch (Exception ex)
            {
                lblMensajeCita.Text = "No se pudo crear la cita. Detalle: " + ex.Message;

                // Recargar horas por si cambió disponibilidad
                if (DateTime.TryParse(txtFecha.Text, out DateTime fecha))
                {
                    ResetHoras();
                    CargarHorasDisponiblesInteligentes(fecha);
                }
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }

        // DTO
        private class ServicioInfo
        {
            public int ServicioID { get; set; }
            public string NombreServicio { get; set; }
            public int DuracionMin { get; set; }
            public int Precio { get; set; }
        }
    }
}