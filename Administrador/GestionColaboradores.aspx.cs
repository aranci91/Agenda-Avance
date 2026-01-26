using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agenda.Administrador
{
    public partial class GestionColaboradores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null || Session["Rol"] == null || Convert.ToInt32(Session["Rol"]) != 3)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                hfUsuarioID.Value = "";
                btnCrear.Text = "CREAR COLABORADOR";
                CargarColaboradores();
            }
        }

        protected void btnInicio_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrador/InicioAdministrador.aspx");
        }

        private void CargarColaboradores()
        {
            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT u.UsuarioID,u.Rut,u.Telefono,u.Correo,c.Nombre,c.Especialidad
FROM Usuarios u
INNER JOIN Colaboradores c ON u.UsuarioID=c.UsuarioID
WHERE u.Rol=2
ORDER BY u.UsuarioID DESC";

            DataTable dt = bd.EjecutarConsulta(sql, null);

            rptColaboradores.DataSource = dt;
            rptColaboradores.DataBind();

            lblVacio.Text = dt.Rows.Count == 0 ? "No hay colaboradores registrados." : "";
        }

        protected void btnCrear_Click(object sender, EventArgs e)
        {
            string rut = NormalizarRut(txtRut.Text);
            string telefono = txtTelefono.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string especialidad = txtEspecialidad.Text.Trim();
            string clave = txtClaveTemp.Text.Trim();

            if (rut == "" || telefono == "" || correo == "" || nombre == "" || especialidad == "")
            {
                lblMsg.Text = "Completa todos los campos.";
                return;
            }

            ConexionBD bd = new ConexionBD();

            if (hfUsuarioID.Value == "")
            {
                string patron = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[-#$%]).{6,8}$";
                if (!Regex.IsMatch(clave, patron))
                {
                    lblMsg.Text = "Contraseña no válida.";
                    return;
                }

                string hash = Seguridad.HashSha256(clave);

                string sqlUser = @"INSERT INTO Usuarios(Rol,Rut,Telefono,Correo,ClaveHash)
VALUES(2,@Rut,@Telefono,@Correo,@ClaveHash);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

                SqlParameter[] p1 =
                {
                    new SqlParameter("@Rut",rut),
                    new SqlParameter("@Telefono",telefono),
                    new SqlParameter("@Correo",correo),
                    new SqlParameter("@ClaveHash",hash)
                };

                int id = Convert.ToInt32(bd.EjecutarConsulta(sqlUser, p1).Rows[0][0]);

                string sqlCol = @"INSERT INTO Colaboradores(UsuarioID,Nombre,Especialidad)
VALUES(@ID,@Nombre,@Especialidad)";

                SqlParameter[] p2 =
                {
                    new SqlParameter("@ID",id),
                    new SqlParameter("@Nombre",nombre),
                    new SqlParameter("@Especialidad",especialidad)
                };

                bd.EjecutarComando(sqlCol, p2);

                lblMsg.Text = "Colaborador creado ✅";
            }
            else
            {
                int id = Convert.ToInt32(hfUsuarioID.Value);

                string sql = @"
UPDATE Usuarios SET Telefono=@Telefono,Correo=@Correo WHERE UsuarioID=@ID;
UPDATE Colaboradores SET Nombre=@Nombre,Especialidad=@Especialidad WHERE UsuarioID=@ID;";

                SqlParameter[] p =
                {
                    new SqlParameter("@Telefono",telefono),
                    new SqlParameter("@Correo",correo),
                    new SqlParameter("@Nombre",nombre),
                    new SqlParameter("@Especialidad",especialidad),
                    new SqlParameter("@ID",id)
                };

                bd.EjecutarComando(sql, p);

                lblMsg.Text = "Colaborador actualizado ✅";
                hfUsuarioID.Value = "";
                btnCrear.Text = "CREAR COLABORADOR";
            }

            Limpiar();
            CargarColaboradores();
        }

        protected void rptColaboradores_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);
            ConexionBD bd = new ConexionBD();

            if (e.CommandName == "editar")
            {
                string sql = @"SELECT Rut,Telefono,Correo,Nombre,Especialidad
FROM Usuarios u INNER JOIN Colaboradores c ON u.UsuarioID=c.UsuarioID
WHERE u.UsuarioID=@ID";

                SqlParameter[] p = { new SqlParameter("@ID", id) };
                DataTable dt = bd.EjecutarConsulta(sql, p);

                txtRut.Text = dt.Rows[0]["Rut"].ToString();
                txtTelefono.Text = dt.Rows[0]["Telefono"].ToString();
                txtCorreo.Text = dt.Rows[0]["Correo"].ToString();
                txtNombre.Text = dt.Rows[0]["Nombre"].ToString();
                txtEspecialidad.Text = dt.Rows[0]["Especialidad"].ToString();

                hfUsuarioID.Value = id.ToString();
                btnCrear.Text = "ACTUALIZAR COLABORADOR";
            }

            if (e.CommandName == "eliminar")
            {
                SqlParameter[] p = { new SqlParameter("@ID", id) };
                bd.EjecutarComando("DELETE FROM Colaboradores WHERE UsuarioID=@ID", p);
                bd.EjecutarComando("DELETE FROM Usuarios WHERE UsuarioID=@ID", p);

                lblMsg.Text = "Colaborador eliminado ✅";
                CargarColaboradores();
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }

        private void Limpiar()
        {
            txtRut.Text = "";
            txtTelefono.Text = "";
            txtCorreo.Text = "";
            txtNombre.Text = "";
            txtEspecialidad.Text = "";
            txtClaveTemp.Text = "";
        }

        private string NormalizarRut(string rutInput)
        {
            if (string.IsNullOrWhiteSpace(rutInput)) return "";

            string rut = rutInput.Trim().ToUpper();
            rut = rut.Replace(".", "").Replace(" ", "").Replace("-", "");

            if (rut.Length < 2) return "";

            for (int i = 0; i < rut.Length; i++)
            {
                char ch = rut[i];
                bool ok = (ch >= '0' && ch <= '9') || ch == 'K';
                if (!ok) return "";
            }

            string cuerpo = rut.Substring(0, rut.Length - 1);
            string dv = rut.Substring(rut.Length - 1, 1);

            return cuerpo + "-" + dv;
        }
    }
}