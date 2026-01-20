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
            pnlVerificacion.Visible = false;

            string rut = NormalizarRut(txtRut.Text);
            string clave = (txtClave.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(rut) || string.IsNullOrWhiteSpace(clave))
            {
                lblMensaje.Text = "Debe ingresar su RUT y contraseña.";
                return;
            }

            string claveHash = Seguridad.HashSha256(clave);

            ConexionBD bd = new ConexionBD();

            string query = @"
SELECT TOP 1 UsuarioID, RolID
FROM dbo.Usuarios
WHERE Rut = @Rut AND ClaveHash = @ClaveHash;
";

            SqlParameter[] parametros =
            {
                new SqlParameter("@Rut", rut),
                new SqlParameter("@ClaveHash", claveHash)
            };

            DataTable dt = bd.EjecutarConsulta(query, parametros);

            if (dt.Rows.Count == 0)
            {
                lblErrorClave.Text = "Contraseña inválida.";
                return;
            }

            int usuarioId = Convert.ToInt32(dt.Rows[0]["UsuarioID"]);
            int rolId = Convert.ToInt32(dt.Rows[0]["RolID"]);

            Session["UsuarioID"] = usuarioId;
            Session["RolID"] = rolId;
            Session["Rut"] = rut;

            string codigo = new Random().Next(100000, 999999).ToString();
            Session["CodigoVerificacion"] = codigo;

            pnlVerificacion.Visible = true;

            // Solo para pruebas
            lblMensaje.Text = "Se envió un código de verificación (simulado). Código: " + codigo;
        }

        protected void btnVerificar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";

            string codigoEsperado = Session["CodigoVerificacion"] as string;
            string codigoIngresado = (txtCodigo.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(codigoEsperado))
            {
                lblMensaje.Text = "Debe generar un código primero.";
                pnlVerificacion.Visible = true;
                return;
            }

            if (codigoIngresado == codigoEsperado)
            {
                int rolId = Convert.ToInt32(Session["RolID"]);

                if (rolId == 1) Response.Redirect("~/Clientes/InicioCliente.aspx");
                else if (rolId == 2) Response.Redirect("~/Colaboradores/InicioColaborador.aspx");
                else if (rolId == 3) Response.Redirect("~/Administrador/InicioAdministrador.aspx");
                else Response.Redirect("~/Inicio.aspx");
            }
            else
            {
                lblMensaje.Text = "El código ingresado no es válido. Intente nuevamente.";
                pnlVerificacion.Visible = true;
            }
        }

        protected void btnEnviarCorreo_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            pnlVerificacion.Visible = true;

            // 1) Validar sesión
            if (Session["UsuarioID"] == null)
            {
                lblMensaje.Text = "Debe iniciar sesión primero para enviar el código por correo.";
                return;
            }

            int usuarioId = Convert.ToInt32(Session["UsuarioID"]);

            // 2) Obtener correo real del usuario desde BD
            string correoCliente = ObtenerCorreoUsuario(usuarioId);

            if (string.IsNullOrWhiteSpace(correoCliente))
            {
                lblMensaje.Text = "No se encontró un correo asociado a tu cuenta.";
                return;
            }

            // 3) Generar nuevo código y guardarlo en sesión
            string codigo = new Random().Next(100000, 999999).ToString();
            Session["CodigoVerificacion"] = codigo;

            try
            {
                // 4) Enviar correo REAL por SMTP
                EmailService.EnviarCodigoVerificacion(correoCliente, codigo);

                string correoOculto = Seguridad.EnmascararCorreo(correoCliente);
                lblMensaje.Text = "Te enviamos el código a tu correo: " + correoOculto;
            }
            catch (Exception ex)
            {
                // Si falla, no botamos el flujo, solo avisamos
                lblMensaje.Text = " No se pudo enviar el correo. Detalle: " + ex.Message;
            }
        }

        private string ObtenerCorreoUsuario(int usuarioId)
        {
            ConexionBD bd = new ConexionBD();

            string query = @"SELECT TOP 1 Correo FROM dbo.Usuarios WHERE UsuarioID = @UsuarioID;";
            SqlParameter[] parametros =
            {
                new SqlParameter("@UsuarioID", usuarioId)
            };

            DataTable dt = bd.EjecutarConsulta(query, parametros);

            if (dt.Rows.Count == 0) return "";
            return Convert.ToString(dt.Rows[0]["Correo"]);
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

            string rut = rutInput.Trim().ToUpper();
            rut = rut.Replace(".", "").Replace(" ", "").Replace("-", "");

            if (rut.Length >= 2)
            {
                rut = rut.Substring(0, rut.Length - 1) + "-" + rut.Substring(rut.Length - 1);
            }

            return rut;
        }
    }
}