using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agenda
{
    public partial class EditardatosCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Seguridad sesión
            if (Session["UsuarioID"] == null || Session["Rol"] == null)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            // Solo cliente (Rol = 1)
            if (Convert.ToInt32(Session["Rol"]) != 1)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarDatosContacto();
            }
        }

        private void CargarDatosContacto()
        {
            lblMensaje.Text = "";

            int usuarioId = Convert.ToInt32(Session["UsuarioID"]);

            try
            {
                ConexionBD bd = new ConexionBD();

                using (SqlConnection cn = bd.ObtenerConexion())
                {
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT Correo, Telefono
                        FROM dbo.Usuarios
                        WHERE UsuarioID = @UsuarioID;
                    ", cn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioId);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                txtCorreo.Text = dr["Correo"].ToString();
                                txtTelefono.Text = dr["Telefono"].ToString();
                            }
                            else
                            {
                                lblMensaje.Text = "No se encontraron tus datos de usuario.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar datos: " + ex.Message;
            }
        }

        protected void btnGuardarContacto_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";

            int usuarioId = Convert.ToInt32(Session["UsuarioID"]);
            string correo = (txtCorreo.Text ?? "").Trim();
            string telefono = (txtTelefono.Text ?? "").Trim();

            // Validaciones
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(telefono))
            {
                lblMensaje.Text = "Debe completar correo y teléfono.";
                return;
            }

            // validación simple de correo
            if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                lblMensaje.Text = "Ingrese un correo válido.";
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
                            // Evitar correos duplicados (de otro usuario)
                            using (SqlCommand cmdExisteCorreo = new SqlCommand(@"
                                SELECT COUNT(1)
                                FROM dbo.Usuarios
                                WHERE Correo = @Correo AND UsuarioID <> @UsuarioID;
                            ", cn, tx))
                            {
                                cmdExisteCorreo.Parameters.AddWithValue("@Correo", correo);
                                cmdExisteCorreo.Parameters.AddWithValue("@UsuarioID", usuarioId);

                                int existe = Convert.ToInt32(cmdExisteCorreo.ExecuteScalar());
                                if (existe > 0)
                                {
                                    lblMensaje.Text = "Ese correo ya está en uso. Intenta con otro.";
                                    tx.Rollback();
                                    return;
                                }
                            }

                            using (SqlCommand cmdUpdate = new SqlCommand(@"
                                UPDATE dbo.Usuarios
                                SET Correo = @Correo,
                                    Telefono = @Telefono
                                WHERE UsuarioID = @UsuarioID;
                            ", cn, tx))
                            {
                                cmdUpdate.Parameters.AddWithValue("@Correo", correo);
                                cmdUpdate.Parameters.AddWithValue("@Telefono", telefono);
                                cmdUpdate.Parameters.AddWithValue("@UsuarioID", usuarioId);

                                cmdUpdate.ExecuteNonQuery();
                            }

                            tx.Commit();
                            lblMensaje.Text = "✅ Datos actualizados correctamente.";
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al guardar datos: " + ex.Message;
            }
        }

        protected void btnCambiarClave_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";

            if (Session["UsuarioID"] == null)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            int usuarioId = Convert.ToInt32(Session["UsuarioID"]);

            string claveActual = txtClaveActual.Text.Trim();
            string claveNueva = txtClaveNueva.Text.Trim();
            string confirmar = txtClaveConfirmar.Text.Trim();

            if (string.IsNullOrWhiteSpace(claveActual) ||
                string.IsNullOrWhiteSpace(claveNueva) ||
                string.IsNullOrWhiteSpace(confirmar))
            {
                lblMensaje.Text = "Debe completar los campos de contraseña.";
                return;
            }

            //  MISMA REGLA que RegistroCliente
            string patron = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[.\-#$%]).{6,8}$";

            if (!Regex.IsMatch(claveNueva, patron))
            {
                lblMensaje.Text = "La contraseña no cumple los requisitos (6 a 8 caracteres, mayúscula, minúscula, número y carácter especial).";
                return;
            }

            if (claveNueva != confirmar)
            {
                lblMensaje.Text = "Las contraseñas no coinciden.";
                return;
            }

            try
            {
                ConexionBD bd = new ConexionBD();

                using (SqlConnection cn = bd.ObtenerConexion())
                {
                    cn.Open();

                    // 1) Traer hash actual de BD
                    string hashActualBD = "";
                    using (SqlCommand cmdGet = new SqlCommand(
                        "SELECT ClaveHash FROM dbo.Usuarios WHERE UsuarioID = @UsuarioID", cn))
                    {
                        cmdGet.Parameters.AddWithValue("@UsuarioID", usuarioId);

                        object result = cmdGet.ExecuteScalar();
                        hashActualBD = (result == null) ? "" : result.ToString();
                    }

                    // 2) Comparar clave actual ingresada vs BD (con el mismo hash)
                    string hashIngresado = Seguridad.HashSha256(claveActual);

                    if (hashIngresado != hashActualBD)
                    {
                        lblMensaje.Text = "La contraseña actual es incorrecta.";
                        return;
                    }

                    // 3) Guardar nueva clave (hasheada)
                    string nuevoHash = Seguridad.HashSha256(claveNueva);

                    using (SqlCommand cmdUpd = new SqlCommand(@"
UPDATE dbo.Usuarios
SET ClaveHash = @ClaveHash
WHERE UsuarioID = @UsuarioID
", cn))
                    {
                        cmdUpd.Parameters.AddWithValue("@ClaveHash", nuevoHash);
                        cmdUpd.Parameters.AddWithValue("@UsuarioID", usuarioId);

                        cmdUpd.ExecuteNonQuery();
                    }

                    lblMensaje.Text = "Contraseña actualizada correctamente ✅";
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cambiar contraseña: " + ex.Message;
            }
        }


        // Navegación

        protected void btnInicio_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Clientes/InicioCliente.aspx");
        }

        protected void btnHistorial_Click(object sender, EventArgs e)
        {
            
            // Response.Redirect("HistorialCitasCliente.aspx");
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }
    }
}
