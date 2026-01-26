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
    public partial class AtenderCita : Page
    {
        private int _citaId;
        private bool _modoEdicion;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Seguridad
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

            if (!int.TryParse(Request.QueryString["citaId"], out _citaId))
            {
                Response.Redirect("~/Colaboradores/AgendaColaborador.aspx");
                return;
            }

            _modoEdicion = Request.QueryString["edit"] == "1";

            if (!IsPostBack)
            {
                CargarDetalle();
                CargarObservacionSiExiste();
            }
        }

        private void CargarDetalle()
        {
            lblMsg.Text = "";

            int colaboradorId = Convert.ToInt32(Session["UsuarioID"]);
            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT TOP 1
    ci.CitaID,
    ci.FechaHora,
    ci.Estado,
    s.NombreServicio AS Servicio,
    (cl.Nombre + ' ' + cl.Apellido) AS Cliente
FROM dbo.Citas ci
INNER JOIN dbo.Servicios s ON s.ServicioID = ci.ServicioID
INNER JOIN dbo.Clientes cl ON cl.UsuarioID = ci.UsuarioID_Cliente
WHERE ci.CitaID = @CitaID
  AND ci.UsuarioID_Colaborador = @Colaborador;";

            SqlParameter[] p =
            {
                new SqlParameter("@CitaID", _citaId),
                new SqlParameter("@Colaborador", colaboradorId),
            };

            DataTable dt = bd.EjecutarConsulta(sql, p);

            if (dt.Rows.Count == 0)
            {
                Response.Redirect("~/Colaboradores/AgendaColaborador.aspx");
                return;
            }

            string estado = Convert.ToString(dt.Rows[0]["Estado"]);
            DateTime fh = Convert.ToDateTime(dt.Rows[0]["FechaHora"]);

            CultureInfo cl = new CultureInfo("es-CL");

            lblServicio.Text = Convert.ToString(dt.Rows[0]["Servicio"]);
            lblCliente.Text = Convert.ToString(dt.Rows[0]["Cliente"]);
            lblFecha.Text = fh.ToString("dddd dd 'de' MMMM yyyy - HH:mm", cl);

            if (!_modoEdicion && !string.Equals(estado, "Confirmada", StringComparison.OrdinalIgnoreCase))
            {
                lblMsg.Text = "Solo puedes atender citas en estado 'Confirmada'.";
                btnGuardar.Enabled = false;
            }
        }

        private void CargarObservacionSiExiste()
        {
            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT Observacion
FROM dbo.Atenciones
WHERE CitaID = @CitaID;";

            SqlParameter[] p =
            {
                new SqlParameter("@CitaID", _citaId)
            };

            DataTable dt = bd.EjecutarConsulta(sql, p);

            if (dt.Rows.Count > 0)
            {
                txtObs.Text = Convert.ToString(dt.Rows[0]["Observacion"]);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";

            try
            {
                int colaboradorId = Convert.ToInt32(Session["UsuarioID"]);
                ConexionBD bd = new ConexionBD();

                string obs = (txtObs.Text ?? "").Trim();

                // 1) Verificar si ya existe atención
                string sqlExiste = @"
SELECT COUNT(*) AS Total
FROM dbo.Atenciones
WHERE CitaID = @CitaID;";

                SqlParameter[] pEx =
                {
                    new SqlParameter("@CitaID", _citaId)
                };

                DataTable dtEx = bd.EjecutarConsulta(sqlExiste, pEx);
                int total = Convert.ToInt32(dtEx.Rows[0]["Total"]);

                if (total == 0)
                {
                    string sqlIns = @"
INSERT INTO dbo.Atenciones (CitaID, Observacion)
VALUES (@CitaID, @Obs);";

                    SqlParameter[] pIns =
                    {
                        new SqlParameter("@CitaID", _citaId),
                        new SqlParameter("@Obs", string.IsNullOrWhiteSpace(obs) ? (object)DBNull.Value : obs)
                    };

                    bd.EjecutarComando(sqlIns, pIns);
                }
                else
                {
                    string sqlUpdAt = @"
UPDATE dbo.Atenciones
SET Observacion = @Obs
WHERE CitaID = @CitaID;";

                    SqlParameter[] pUpdAt =
                    {
                        new SqlParameter("@CitaID", _citaId),
                        new SqlParameter("@Obs", string.IsNullOrWhiteSpace(obs) ? (object)DBNull.Value : obs)
                    };

                    bd.EjecutarComando(sqlUpdAt, pUpdAt);
                }

                // 2) Cambiar estado solo si NO es edición
                if (!_modoEdicion)
                {
                    string sqlUpd = @"
UPDATE dbo.Citas
SET Estado = 'Atendida'
WHERE CitaID = @CitaID
  AND UsuarioID_Colaborador = @Colaborador
  AND Estado = 'Confirmada';";

                    SqlParameter[] pUpd =
                    {
                        new SqlParameter("@CitaID", _citaId),
                        new SqlParameter("@Colaborador", colaboradorId),
                    };

                    bd.EjecutarComando(sqlUpd, pUpd);
                }

                Response.Redirect("~/Colaboradores/AgendaColaborador.aspx");
            }
            catch (Exception ex)
            {
                lblMsg.Text = "No se pudo guardar. Detalle: " + ex.Message;
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