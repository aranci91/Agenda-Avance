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
    public partial class MisDatos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null || Convert.ToInt32(Session["Rol"]) != 3)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarDatos();
            }
        }

        private void CargarDatos()
        {
            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT u.Rut, u.Telefono, u.Correo, c.Nombre, c.Apellido
FROM Usuarios u
INNER JOIN Clientes c ON c.UsuarioID = u.UsuarioID
WHERE u.UsuarioID = @id";

            SqlParameter[] param =
            {
                new SqlParameter("@id", Session["UsuarioID"])
            };

            DataTable dt = bd.EjecutarConsulta(sql, param);

            if (dt.Rows.Count > 0)
            {
                txtRut.Text = dt.Rows[0]["Rut"].ToString();
                txtNombre.Text = dt.Rows[0]["Nombre"].ToString();
                txtApellido.Text = dt.Rows[0]["Apellido"].ToString();
                txtTelefono.Text = dt.Rows[0]["Telefono"].ToString();
                txtCorreo.Text = dt.Rows[0]["Correo"].ToString();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            ConexionBD bd = new ConexionBD();

            string sql = @"
UPDATE Usuarios
SET Telefono=@tel, Correo=@correo
WHERE UsuarioID=@id;

UPDATE Clientes
SET Nombre=@nombre, Apellido=@apellido
WHERE UsuarioID=@id;";

            if (!string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                sql += " UPDATE Usuarios SET PasswordHash = HASHBYTES('SHA2_256', @pass) WHERE UsuarioID=@id;";
            }

            SqlParameter[] param =
            {
                new SqlParameter("@tel", txtTelefono.Text),
                new SqlParameter("@correo", txtCorreo.Text),
                new SqlParameter("@nombre", txtNombre.Text),
                new SqlParameter("@apellido", txtApellido.Text),
                new SqlParameter("@pass", txtPassword.Text),
                new SqlParameter("@id", Session["UsuarioID"])
            };

            bd.EjecutarComando(sql, param);

            lblMsg.Text = "✅ Datos actualizados correctamente";
            txtPassword.Text = "";
        }

        protected void btnInicio_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrador/InicioAdministrador.aspx");
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }
    }
}