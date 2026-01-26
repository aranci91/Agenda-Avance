using Agenda.Servicios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Agenda
{
    public partial class Ingreso : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Inicio.aspx");
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            lblErrorClave.Text = "";
            lblErrorRut.Text = "";
            pnlVerificacion.Visible = false;

            string rut = NormalizarRut(txtRut.Text);
            string clave = (txtClave.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(rut) || string.IsNullOrWhiteSpace(clave))
            {
                lblMensaje.Text = "Debe ingresar su RUT y contraseña.";
                return;
            }

            if (!RutValido(rut))
            {
                lblErrorRut.Text = "RUT inválido.";
                return;
            }

            string claveHash = Seguridad.HashSha256(clave);
            ConexionBD bd = new ConexionBD();

            string query = @"
SELECT TOP 1 UsuarioID, Rol, Correo
FROM dbo.Usuarios
WHERE Rut=@Rut AND ClaveHash=@ClaveHash";

            SqlParameter[] p =
            {
                new SqlParameter("@Rut", rut),
                new SqlParameter("@ClaveHash", claveHash)
            };

            DataTable dt = bd.EjecutarConsulta(query, p);

            if (dt.Rows.Count == 0)
            {
                lblErrorClave.Text = "Contraseña incorrecta.";
                return;
            }

            int usuarioId = Convert.ToInt32(dt.Rows[0]["UsuarioID"]);
            int rol = Convert.ToInt32(dt.Rows[0]["Rol"]);
            string correo = dt.Rows[0]["Correo"].ToString();

            Session.Clear();
            Session["UsuarioID"] = usuarioId;
            Session["Rol"] = rol;
            Session["Rut"] = rut;

            EnviarCodigoCorreo(correo);

            pnlVerificacion.Visible = true;
            lblMensaje.Text = "Se envió un código a tu correo.";
        }

        protected void btnVerificar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";

            string codigoEsperado = Session["CodigoVerificacion"] as string;
            string codigoIngresado = txtCodigo.Text.Trim();

            if (Session["CodigoExpira"] != null &&
                DateTime.Now > (DateTime)Session["CodigoExpira"])
            {
                lblMensaje.Text = "El código expiró. Reenvíalo.";
                return;
            }

            if (codigoIngresado == codigoEsperado)
            {
                int rol = Convert.ToInt32(Session["Rol"]);

                Session.Remove("CodigoVerificacion");
                Session.Remove("CodigoExpira");

                if (rol == 1)
                    Response.Redirect("~/Clientes/InicioCliente.aspx");
                else if (rol == 2)
                    Response.Redirect("~/Colaboradores/InicioColaborador.aspx");
                else if (rol == 3)
                    Response.Redirect("~/Administrador/InicioAdministrador.aspx");
            }
            else
            {
                lblMensaje.Text = "Código incorrecto.";
            }
        }

        protected void btnReenviarCodigo_Click(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null) return;

            int usuarioId = Convert.ToInt32(Session["UsuarioID"]);
            string correo = ObtenerCorreoUsuario(usuarioId);

            EnviarCodigoCorreo(correo);
            lblMensaje.Text = "Código reenviado a tu correo.";
        }

        private void EnviarCodigoCorreo(string correo)
        {
            string codigo = new Random().Next(100000, 999999).ToString();

            Session["CodigoVerificacion"] = codigo;
            Session["CodigoExpira"] = DateTime.Now.AddMinutes(5);

            EmailService.EnviarCodigoVerificacion(correo, codigo);
        }

        private string ObtenerCorreoUsuario(int usuarioId)
        {
            ConexionBD bd = new ConexionBD();
            string sql = "SELECT Correo FROM Usuarios WHERE UsuarioID=@id";

            SqlParameter[] p =
            {
                new SqlParameter("@id", usuarioId)
            };

            DataTable dt = bd.EjecutarConsulta(sql, p);
            return dt.Rows.Count > 0 ? dt.Rows[0]["Correo"].ToString() : "";
        }

        protected void lnkCrearCuenta_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/RegistroCliente.aspx");
        }

        protected void lnkRecuperarClave_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/RecuperarClaveCliente.aspx");
        }

        private string NormalizarRut(string rutInput)
        {
            if (string.IsNullOrWhiteSpace(rutInput)) return "";

            rutInput = rutInput.Replace(".", "").Replace("-", "").Trim().ToUpper();
            return rutInput.Substring(0, rutInput.Length - 1) + "-" + rutInput.Substring(rutInput.Length - 1);
        }

        private bool RutValido(string rutConDv)
        {
            rutConDv = rutConDv.Replace(".", "").ToUpper();
            if (!rutConDv.Contains("-")) return false;

            string[] partes = rutConDv.Split('-');
            int suma = 0, multiplo = 2;

            for (int i = partes[0].Length - 1; i >= 0; i--)
            {
                suma += (partes[0][i] - '0') * multiplo;
                multiplo = multiplo == 7 ? 2 : multiplo + 1;
            }

            int resto = 11 - (suma % 11);
            string dv = resto == 11 ? "0" : resto == 10 ? "K" : resto.ToString();

            return dv == partes[1];
        }
    }
}