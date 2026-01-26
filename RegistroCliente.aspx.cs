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
                string.IsNullOrWhiteSpace(txtFechaNacimiento.Text) ||
                string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                string.IsNullOrWhiteSpace(txtClave.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmarClave.Text))
            {
                lblMensaje.Text = "Debe completar todos los campos obligatorios.";
                return;
            }


            // Capitalizar nombres y apellidos

            string nombres = Capitalizar(txtNombres.Text.Trim());
            string apellidos = Capitalizar(txtApellidos.Text.Trim());

            // Validar RUT real

            string rut = NormalizarRut(txtRut.Text);

            if (!EsRutValido(rut))
            {
                lblMensaje.Text = "El RUT ingresado no es válido.";
                return;
            }

            // 2) Validación de contraseña
            string clave = txtClave.Text.Trim();
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

            string claveHash = Seguridad.HashSha256(clave);
            int rolCliente = 1;

            // 3) Fecha nacimiento
            if (!DateTime.TryParse(txtFechaNacimiento.Text, out DateTime fechaNac))
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

                            // Verificar RUT existente

                            using (SqlCommand cmdExisteRut = new SqlCommand(
                                "SELECT COUNT(1) FROM dbo.Usuarios WHERE Rut = @Rut", cn, tx))
                            {
                                cmdExisteRut.Parameters.AddWithValue("@Rut", rut);

                                int existeRut = Convert.ToInt32(cmdExisteRut.ExecuteScalar());
                                if (existeRut > 0)
                                {
                                    lblMensaje.Text = "Este RUT ya está registrado.";
                                    tx.Rollback();
                                    return;
                                }
                            }

                          
                            // Verificar CORREO existente
                            using (SqlCommand cmdExisteCorreo = new SqlCommand(
                                "SELECT COUNT(1) FROM dbo.Usuarios WHERE Correo = @Correo", cn, tx))
                            {
                                cmdExisteCorreo.Parameters.AddWithValue("@Correo", txtCorreo.Text.Trim());

                                int existeCorreo = Convert.ToInt32(cmdExisteCorreo.ExecuteScalar());
                                if (existeCorreo > 0)
                                {
                                    lblMensaje.Text = "Este correo ya está registrado. Intenta iniciar sesión.";
                                    tx.Rollback();
                                    return;
                                }
                            }

                            // Insertar usuario
                            int usuarioId;
                            using (SqlCommand cmdUser = new SqlCommand(@"
INSERT INTO dbo.Usuarios (Rol, Rut, Correo, ClaveHash, Telefono)
OUTPUT INSERTED.UsuarioID
VALUES (@Rol, @Rut, @Correo, @ClaveHash, @Telefono);", cn, tx))
                            {
                                cmdUser.Parameters.AddWithValue("@Rol", rolCliente);
                                cmdUser.Parameters.AddWithValue("@Rut", rut);
                                cmdUser.Parameters.AddWithValue("@Correo", txtCorreo.Text.Trim());
                                cmdUser.Parameters.AddWithValue("@ClaveHash", claveHash);
                                cmdUser.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());

                                usuarioId = Convert.ToInt32(cmdUser.ExecuteScalar());
                            }

                            // Insertar cliente
                            using (SqlCommand cmdCliente = new SqlCommand(@"
INSERT INTO dbo.Clientes (UsuarioID, Nombre, Apellido, FechaNacimiento)
VALUES (@UsuarioID, @Nombre, @Apellido, @FechaNacimiento);", cn, tx))
                            {
                                cmdCliente.Parameters.AddWithValue("@UsuarioID", usuarioId);
                                cmdCliente.Parameters.AddWithValue("@Nombre", nombres);
                                cmdCliente.Parameters.AddWithValue("@Apellido", apellidos);
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


                // Mensaje de éxito

                Session["RegistroExitoso"] = true;
                lblMensaje.Text = "Registro exitoso. Ya puedes iniciar sesión.";
                lblMensaje.ForeColor = System.Drawing.Color.Green;


                ClientScript.RegisterStartupScript(this.GetType(), "redirigir",
                    "setTimeout(function(){ window.location='Ingreso.aspx'; }, 5000);", true);

            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al registrar: " + ex.Message;
            }
        }

        //  UTILIDADES 


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



        // Validador real de RUT chileno

        private bool EsRutValido(string rut)
        {
            try
            {
                rut = rut.Replace(".", "").Replace("-", "").ToUpper();
                if (rut.Length < 2) return false;

                string numero = rut.Substring(0, rut.Length - 1);
                char dv = rut[rut.Length - 1];

                int suma = 0;
                int multiplicador = 2;

                for (int i = numero.Length - 1; i >= 0; i--)
                {
                    suma += int.Parse(numero[i].ToString()) * multiplicador;
                    multiplicador++;
                    if (multiplicador > 7) multiplicador = 2;
                }

                int resto = suma % 11;
                int dvCalculado = 11 - resto;

                string dvFinal;
                if (dvCalculado == 11) dvFinal = "0";
                else if (dvCalculado == 10) dvFinal = "K";
                else dvFinal = dvCalculado.ToString();

                return dv.ToString() == dvFinal;
            }
            catch
            {
                return false;
            }
        }


        // Capitalización backend

        private string Capitalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return texto;

            texto = texto.ToLower();
            string[] partes = texto.Split(' ');
            for (int i = 0; i < partes.Length; i++)
            {
                if (partes[i].Length > 0)
                {
                    partes[i] = char.ToUpper(partes[i][0]) + partes[i].Substring(1);
                }
            }
            return string.Join(" ", partes);
        }
    }
}