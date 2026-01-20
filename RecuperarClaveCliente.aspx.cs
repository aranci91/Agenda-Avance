using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agenda
{
    public partial class RecuperarClaveCliente : System.Web.UI.Page
    {
        private const string SESSION_CODIGO = "CODIGO_RECUP_CLIENTE";
        private const string SESSION_RUT = "RUT_RECUP_CLIENTE";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlCodigo.Visible = false;
                pnlNuevaClave.Visible = false;
                lblMensaje.Text = "";
            }
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("Ingreso.aspx");
        }

        protected void btnEnviarCodigo_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";

            string rut = NormalizarRut(txtRut.Text);

            if (string.IsNullOrWhiteSpace(rut))
            {
                lblMensaje.Text = "Debe ingresar su RUT.";
                pnlCodigo.Visible = false;
                pnlNuevaClave.Visible = false;
                return;
            }

            // Verificar que el RUT exista en BD
            ConexionBD bd = new ConexionBD();
            string qExiste = "SELECT COUNT(1) FROM dbo.Usuarios WHERE Rut = @Rut;";
            DataTable dt = bd.EjecutarConsulta(qExiste, new SqlParameter[] { new SqlParameter("@Rut", rut) });

            int existe = Convert.ToInt32(dt.Rows[0][0]);
            if (existe == 0)
            {
                lblMensaje.Text = "Este RUT no está registrado.";
                pnlCodigo.Visible = false;
                pnlNuevaClave.Visible = false;
                return;
            }

            // Generar código 6 dígitos
            string codigo = new Random().Next(100000, 999999).ToString();
            Session[SESSION_CODIGO] = codigo;
            Session[SESSION_RUT] = rut;

            pnlCodigo.Visible = true;
            pnlNuevaClave.Visible = false;

            // Solo para pruebas
            lblMensaje.Text = "Código simulado enviado: " + codigo;
        }

        protected void btnValidarCodigo_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";

            string codigoIngresado = (txtCodigo.Text ?? "").Trim();
            string codigoReal = Session[SESSION_CODIGO] as string;

            if (string.IsNullOrWhiteSpace(codigoReal))
            {
                lblMensaje.Text = "Primero debe solicitar el envío del código.";
                pnlNuevaClave.Visible = false;
                return;
            }

            if (codigoIngresado == codigoReal)
            {
                pnlNuevaClave.Visible = true;
                lblMensaje.Text = "";
            }
            else
            {
                pnlNuevaClave.Visible = false;
                lblMensaje.Text = "Código incorrecto. Intente nuevamente.";
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";

            string rut = Session[SESSION_RUT] as string;
            if (string.IsNullOrWhiteSpace(rut))
            {
                lblMensaje.Text = "La sesión expiró. Vuelva a solicitar el código.";
                pnlCodigo.Visible = false;
                pnlNuevaClave.Visible = false;
                return;
            }

            string clave = (txtNuevaClave.Text ?? "").Trim();
            string confirmar = (txtConfirmarClave.Text ?? "").Trim();

            // Validar reglas (mismo patrón)
            string patron = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[.\-#$%]).{6,8}$";
            if (!Regex.IsMatch(clave, patron))
            {
                lblMensaje.Text = "La contraseña no cumple los requisitos (6 a 8 caracteres, mayúscula, minúscula, número y carácter especial).";
                pnlCodigo.Visible = true;
                pnlNuevaClave.Visible = true;
                return;
            }

            if (clave != confirmar)
            {
                lblMensaje.Text = "Las contraseñas no coinciden.";
                pnlCodigo.Visible = true;
                pnlNuevaClave.Visible = true;
                return;
            }

            // Hash (mismo del sistema)
            string nuevaHash = Seguridad.HashSha256(clave);

            // UPDATE en BD
            ConexionBD bd = new ConexionBD();
            string qUpdate = "UPDATE dbo.Usuarios SET ClaveHash = @Hash WHERE Rut = @Rut;";

            int filas = bd.EjecutarComando(qUpdate, new SqlParameter[]
            {
                new SqlParameter("@Hash", nuevaHash),
                new SqlParameter("@Rut", rut)
            });

            // Limpia sesión
            Session.Remove(SESSION_CODIGO);
            Session.Remove(SESSION_RUT);

            if (filas > 0)
                Response.Redirect("Ingreso.aspx");
            else
                lblMensaje.Text = "No se pudo actualizar la contraseña. Intente nuevamente.";
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