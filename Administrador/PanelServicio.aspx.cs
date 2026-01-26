using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agenda.Administrador
{
    public partial class PanelServicio : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Seguridad ADMIN
            if (Session["UsuarioID"] == null || Convert.ToInt32(Session["Rol"]) != 3)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarColaboradores();

                if (Request.QueryString["id"] != null)
                {
                    lblTitulo.Text = "Editar servicio";
                    CargarServicio(Request.QueryString["id"]);

                    int servicioId = Convert.ToInt32(Request.QueryString["id"]);
                    CargarColaboradorAsignado(servicioId);
                }
            }
        }

        // =========================
        // CARGAR SERVICIO
        // =========================
        private void CargarServicio(string id)
        {
            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT ServicioID, NombreServicio, DuracionMin, Precio, Activo
FROM dbo.Servicios
WHERE ServicioID = @id";

            SqlParameter[] p =
            {
                new SqlParameter("@id", id)
            };

            DataTable dt = bd.EjecutarConsulta(sql, p);
            if (dt.Rows.Count == 0) return;

            hfServicioID.Value = dt.Rows[0]["ServicioID"].ToString();
            txtNombre.Text = dt.Rows[0]["NombreServicio"].ToString();
            txtDuracion.Text = dt.Rows[0]["DuracionMin"].ToString();
            txtPrecio.Text = dt.Rows[0]["Precio"].ToString();
            chkActivo.Checked = Convert.ToBoolean(dt.Rows[0]["Activo"]);
        }

        // =========================
        // CARGAR COLABORADORES
        // =========================
        private void CargarColaboradores()
        {
            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT u.UsuarioID, c.Nombre
FROM dbo.Colaboradores c
INNER JOIN dbo.Usuarios u ON u.UsuarioID = c.UsuarioID
ORDER BY c.Nombre";

            ddlColaboradores.DataSource = bd.EjecutarConsulta(sql, null);
            ddlColaboradores.DataTextField = "Nombre";
            ddlColaboradores.DataValueField = "UsuarioID";
            ddlColaboradores.DataBind();
        }

        // =========================
        // CARGAR COLABORADOR ASIGNADO
        // =========================
        private void CargarColaboradorAsignado(int servicioId)
        {
            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT TOP 1 UsuarioID_Colaborador, Activo
FROM dbo.ServiciosColaboradores
WHERE ServicioID = @s AND Activo = 1";

            SqlParameter[] p =
            {
                new SqlParameter("@s", servicioId)
            };

            DataTable dt = bd.EjecutarConsulta(sql, p);

            if (dt.Rows.Count > 0)
            {
                ddlColaboradores.SelectedValue = dt.Rows[0]["UsuarioID_Colaborador"].ToString();
                chkColabActivo.Checked = Convert.ToBoolean(dt.Rows[0]["Activo"]);
            }
        }

        // =========================
        // GUARDAR SERVICIO
        // =========================
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";

            if (!int.TryParse(txtDuracion.Text, out int duracion))
            {
                lblMensaje.Text = "❌ Duración inválida";
                return;
            }

            if (!int.TryParse(txtPrecio.Text, out int precio))
            {
                lblMensaje.Text = "❌ Precio inválido";
                return;
            }

            ConexionBD bd = new ConexionBD();
            bool nuevo = string.IsNullOrEmpty(hfServicioID.Value);

            int servicioId;

            if (nuevo)
            {
                string sqlInsert = @"
INSERT INTO dbo.Servicios (NombreServicio, DuracionMin, Precio, Activo)
OUTPUT INSERTED.ServicioID
VALUES (@n,@d,@p,@a)";

                SqlParameter[] pInsert =
                {
                    new SqlParameter("@n", txtNombre.Text.Trim()),
                    new SqlParameter("@d", duracion),
                    new SqlParameter("@p", precio),
                    new SqlParameter("@a", chkActivo.Checked)
                };

                servicioId = Convert.ToInt32(bd.EjecutarEscalar(sqlInsert, pInsert));
            }
            else
            {
                servicioId = Convert.ToInt32(hfServicioID.Value);

                string sqlUpdate = @"
UPDATE dbo.Servicios
SET NombreServicio=@n, DuracionMin=@d, Precio=@p, Activo=@a
WHERE ServicioID=@id";

                SqlParameter[] pUpdate =
                {
                    new SqlParameter("@n", txtNombre.Text.Trim()),
                    new SqlParameter("@d", duracion),
                    new SqlParameter("@p", precio),
                    new SqlParameter("@a", chkActivo.Checked),
                    new SqlParameter("@id", servicioId)
                };

                bd.EjecutarComando(sqlUpdate, pUpdate);
            }

            // DESACTIVAR TODOS LOS COLABORADORES
            string sqlReset = @"
UPDATE dbo.ServiciosColaboradores
SET Activo = 0
WHERE ServicioID = @s";

            bd.EjecutarComando(sqlReset, new SqlParameter[] {
                new SqlParameter("@s", servicioId)
            });

            // ACTIVAR / INSERTAR COLABORADOR
            string sqlColab = @"
IF EXISTS (
    SELECT 1 FROM dbo.ServiciosColaboradores
    WHERE ServicioID=@s AND UsuarioID_Colaborador=@c
)
    UPDATE dbo.ServiciosColaboradores
    SET Activo=@a
    WHERE ServicioID=@s AND UsuarioID_Colaborador=@c
ELSE
    INSERT INTO dbo.ServiciosColaboradores (ServicioID, UsuarioID_Colaborador, Activo)
    VALUES (@s,@c,@a)";

            SqlParameter[] pColab =
            {
                new SqlParameter("@s", servicioId),
                new SqlParameter("@c", ddlColaboradores.SelectedValue),
                new SqlParameter("@a", chkColabActivo.Checked)
            };

            bd.EjecutarComando(sqlColab, pColab);

            lblMensaje.Text = "✅ Servicio guardado correctamente";
        }

        // =========================
        // NAVEGACIÓN
        // =========================
        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("GestionServicios.aspx");
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }
    }
}