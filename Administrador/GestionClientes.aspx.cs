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
    public partial class GestionClientes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Seguridad
            if (Session["UsuarioID"] == null || Session["Rol"] == null)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            // Solo admin (Rol = 3)
            if (Convert.ToInt32(Session["Rol"]) != 3)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblMsg.Text = "";
                lblVacio.Text = "";
                txtBuscar.Text = "";
                CargarClientes("");
            }
        }

        protected void btnInicio_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrador/InicioAdministrador.aspx");
        }

    
        // CARGAR CLIENTES 
      
        private void CargarClientes(string filtro)
        {
            lblVacio.Text = "";
            lblMsg.Text = "";

            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT 
    u.UsuarioID,
    u.Rut,
    u.Telefono,
    u.Correo,
    c.Nombre,
    c.Apellido
FROM dbo.Usuarios u
INNER JOIN dbo.Clientes c ON c.UsuarioID = u.UsuarioID
WHERE u.Rol = 1
";

            SqlParameter[] parametros = null;

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                sql += @"
  AND (
        c.Nombre LIKE @Filtro
     OR c.Apellido LIKE @Filtro
     OR u.Rut LIKE @Filtro
     OR u.Correo LIKE @Filtro
     OR u.Telefono LIKE @Filtro
  )
";
                parametros = new SqlParameter[]
                {
                    new SqlParameter("@Filtro", "%" + filtro.Trim() + "%")
                };
            }

            sql += " ORDER BY c.Apellido, c.Nombre;";

            DataTable dt = bd.EjecutarConsulta(sql, parametros);

            rptClientes.DataSource = dt;
            rptClientes.DataBind();

            if (dt.Rows.Count == 0)
            {
                lblVacio.Text = string.IsNullOrWhiteSpace(filtro)
                    ? "Aún no hay clientes registrados."
                    : "No se encontraron clientes con ese criterio.";
            }
        }


        // BUSCAR

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string texto = (txtBuscar.Text ?? "").Trim();
            CargarClientes(texto);
        }


        // ELIMINAR DESDE EL REPEATER

        protected void rptClientes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int usuarioId))
                return;

            if (e.CommandName == "eliminar")
            {
                try
                {
                    ConexionBD bd = new ConexionBD();

                   
                    string sqlDelCliente = "DELETE FROM dbo.Clientes WHERE UsuarioID = @UsuarioID;";
                    SqlParameter[] p1 = { new SqlParameter("@UsuarioID", usuarioId) };
                    bd.EjecutarComando(sqlDelCliente, p1);

                    string sqlDelUsuario = "DELETE FROM dbo.Usuarios WHERE UsuarioID = @UsuarioID;";
                    SqlParameter[] p2 = { new SqlParameter("@UsuarioID", usuarioId) };
                    bd.EjecutarComando(sqlDelUsuario, p2);

                    lblMsg.Text = "Cliente eliminado correctamente ✅";

                    // Mantener el filtro 
                    string filtroActual = (txtBuscar.Text ?? "").Trim();
                    CargarClientes(filtroActual);
                }
                catch (Exception ex)
                {
                    lblMsg.Text = "No se pudo eliminar el cliente (puede tener citas asociadas). Detalle: " + ex.Message;
                }
            }
        }


        // CERRAR SESIÓN

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }
    }
}