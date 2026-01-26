using Agenda.Servicios;
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
                return;
            }

            ConexionBD bd = new ConexionBD();
            string sql = "SELECT Correo FROM Usuarios WHERE Rut=@Rut";

            DataTable dt = bd.EjecutarConsulta(sql, new SqlParameter[]
            {
                new SqlParameter("@Rut", rut)
            });

            if (dt.Rows.Count == 0)
            {
                lblMensaje.Text = "Este RUT no está registrado.";
                return;
            }

            string correo = dt.Rows[0]["Correo"].ToString();

            string codigo = new Random().Next(100000, 999999).ToString();

            Session[SESSION_CODIGO] = codigo;
            Session[SESSION_RUT] = rut;

            EmailService.EnviarCodigoVerificacion(correo, codigo);

            pnlCodigo.Visible = true;
            pnlNuevaClave.Visible = false;

            lblMensaje.Text = "Se envió un código a tu correo.";
        }

        protected void btnValidarCodigo_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";

            string codigoIngresado = txtCodigo.Text.Trim();
            string codigoReal = Session[SESSION_CODIGO] as string;

            if (codigoIngresado == codigoReal)
            {
                pnlNuevaClave.Visible = true;
            }
            else
            {
                lblMensaje.Text = "Código incorrecto.";
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string rut = Session[SESSION_RUT] as string;

            string clave = txtNuevaClave.Text.Trim();
            string confirmar = txtConfirmarClave.Text.Trim();

            string patron = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[.\-#$%]).{6,8}$";

            if (!Regex.IsMatch(clave, patron))
            {
                lblMensaje.Text = "La contraseña no cumple los requisitos.";
                return;
            }

            if (clave != confirmar)
            {
                lblMensaje.Text = "Las contraseñas no coinciden.";
                return;
            }

            string hash = Seguridad.HashSha256(clave);

            ConexionBD bd = new ConexionBD();
            string sql = "UPDATE Usuarios SET ClaveHash=@Hash WHERE Rut=@Rut";

            bd.EjecutarComando(sql, new SqlParameter[]
            {
                new SqlParameter("@Hash", hash),
                new SqlParameter("@Rut", rut)
            });

            Session.Remove(SESSION_CODIGO);
            Session.Remove(SESSION_RUT);

            Response.Redirect("Ingreso.aspx");
        }

        private string NormalizarRut(string rutInput)
        {
            rutInput = rutInput.Replace(".", "").Replace("-", "").Trim().ToUpper();
            return rutInput.Substring(0, rutInput.Length - 1) + "-" + rutInput.Substring(rutInput.Length - 1);
        }
    }
}