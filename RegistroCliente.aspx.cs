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

namespace Agenda
{
    public partial class RegistroCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("Ingreso.aspx");
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";

            // 1) Validaciones básicas
            if (string.IsNullOrWhiteSpace(txtNombres.Text) ||
                string.IsNullOrWhiteSpace(txtApellidos.Text) ||
                string.IsNullOrWhiteSpace(txtRut.Text) ||
                string.IsNullOrWhiteSpace(txtFechaNacimiento.Text) ||   // ✅ obligatoria
                string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                string.IsNullOrWhiteSpace(txtClave.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmarClave.Text))
            {
                lblMensaje.Text = "Debe completar todos los campos obligatorios.";
                return;
            }

            // 2) Validación de contraseña (regla UI: 6-8 y .-#$%)
            string clave = txtClave.Text.Trim();
            string confirmar = txtConfirmarClave.Text.Trim();
            string patron = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[.\-#$%]).{6,8}$";

            if (!Regex.IsMatch(clave, patron))
            {
                lblMensaje.Text = "La contraseña no cumple los requisitos (6 a 8 caracteres, mayúscula, minúscula, número y carácter especial).";
                return;
            }

            if (clave != confirmar)
            {
                lblMensaje.Text = "Las contraseñas no coinciden.";
                return;
            }

            // 3) Normalizar RUT
            string rut = NormalizarRut(txtRut.Text);

            // 4) Hash de contraseña ( SIEMPRE el mismo: Seguridad.HashSha256)
            string claveHash = Seguridad.HashSha256(clave);

            int rolCliente = 1; // según Roles

            // 5) Fecha nacimiento
            DateTime fechaNac;
            if (!DateTime.TryParse(txtFechaNacimiento.Text, out fechaNac))
            {
                lblMensaje.Text = "La fecha de nacimiento no es válida.";
                return;
            }

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
                            // 5.1) Verificar si ya existe el RUT
                            using (SqlCommand cmdExiste = new SqlCommand(
                                "SELECT COUNT(1) FROM dbo.Usuarios WHERE Rut = @Rut", cn, tx))
                            {
                                cmdExiste.Parameters.AddWithValue("@Rut", rut);

                                int existe = Convert.ToInt32(cmdExiste.ExecuteScalar());
                                if (existe > 0)
                                {
                                    lblMensaje.Text = "Este RUT ya está registrado. Intente iniciar sesión o recuperar contraseña.";
                                    tx.Rollback();
                                    return;
                                }
                            }

                            // 5.2) Insertar en Usuarios y obtener UsuarioID
                            int usuarioId;
                            using (SqlCommand cmdUser = new SqlCommand(@"
INSERT INTO dbo.Usuarios (RolID, Rut, Correo, ClaveHash, Telefono)
OUTPUT INSERTED.UsuarioID
VALUES (@RolID, @Rut, @Correo, @ClaveHash, @Telefono);
", cn, tx))
                            {
                                cmdUser.Parameters.AddWithValue("@RolID", rolCliente);
                                cmdUser.Parameters.AddWithValue("@Rut", rut);
                                cmdUser.Parameters.AddWithValue("@Correo", txtCorreo.Text.Trim());
                                cmdUser.Parameters.AddWithValue("@ClaveHash", claveHash);
                                cmdUser.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());

                                usuarioId = Convert.ToInt32(cmdUser.ExecuteScalar());
                            }

                            // 5.3) Insertar en Clientes
                            using (SqlCommand cmdCliente = new SqlCommand(@"
INSERT INTO dbo.Clientes (UsuarioID, Nombre, Apellido, FechaNacimiento)
VALUES (@UsuarioID, @Nombre, @Apellido, @FechaNacimiento);
", cn, tx))
                            {
                                cmdCliente.Parameters.AddWithValue("@UsuarioID", usuarioId);
                                cmdCliente.Parameters.AddWithValue("@Nombre", txtNombres.Text.Trim());
                                cmdCliente.Parameters.AddWithValue("@Apellido", txtApellidos.Text.Trim());
                                cmdCliente.Parameters.AddWithValue("@FechaNacimiento", fechaNac);

                                cmdCliente.ExecuteNonQuery();
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

                Response.Redirect("Ingreso.aspx");
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al registrar: " + ex.Message;
            }
        }

        private string NormalizarRut(string rutInput)
        {
            if (string.IsNullOrWhiteSpace(rutInput)) return "";

            string rut = rutInput.Replace(".", "").Replace(" ", "").ToUpper();
            rut = rut.Replace("-", "");

            if (rut.Length >= 2)
            {
                rut = rut.Substring(0, rut.Length - 1) + "-" + rut.Substring(rut.Length - 1);
            }

            return rut;
        }
    }
}