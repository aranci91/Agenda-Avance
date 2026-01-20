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
            // Seguridad
            if (Session["UsuarioID"] == null || Session["RolID"] == null)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            // Solo admin (RolID = 3)
            if (Convert.ToInt32(Session["RolID"]) != 3)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblMsg.Text = "";
                lblVacio.Text = "";
                CargarColaboradores();
            }
        }

        private void CargarColaboradores()
        {
            lblVacio.Text = "";

            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT 
    u.UsuarioID,
    u.Rut,
    u.Correo,
    c.Nombre,
    c.Especialidad
FROM dbo.Usuarios u
INNER JOIN dbo.Colaboradores c ON c.UsuarioID = u.UsuarioID
WHERE u.RolID = 2
ORDER BY u.UsuarioID DESC;";

            DataTable dt = bd.EjecutarConsulta(sql, null);

            rptColaboradores.DataSource = dt;
            rptColaboradores.DataBind();

            if (dt.Rows.Count == 0)
                lblVacio.Text = "Aún no hay colaboradores registrados.";
        }

        
        // CREAR COLABORADOR
       
        protected void btnCrear_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";

            string rut = NormalizarRut(txtRut.Text);
            string correo = (txtCorreo.Text ?? "").Trim();
            string nombre = (txtNombre.Text ?? "").Trim();
            string especialidad = (txtEspecialidad.Text ?? "").Trim();
            string claveTemp = (txtClaveTemp.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(rut) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(especialidad) ||
                string.IsNullOrWhiteSpace(claveTemp))
            {
                lblMsg.Text = "Completa todos los campos para crear el colaborador.";
                return;
            }

            // Misma regla de clave del RegistroCliente
            string patron = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[-#$%]).{6,8}$";
            if (!Regex.IsMatch(claveTemp, patron))
            {
                lblMsg.Text = "La contraseña no cumple requisitos (6 a 8 caracteres, mayúscula, minúscula, número y caracter especial -#$%).";
                return;
            }

            try
            {
                ConexionBD bd = new ConexionBD();

                // 1) Validar que el RUT no exista
                string sqlExiste = @"SELECT COUNT(1) AS Existe FROM dbo.Usuarios WHERE Rut = @Rut;";
                SqlParameter[] pExiste = { new SqlParameter("@Rut", rut) };
                DataTable dtExiste = bd.EjecutarConsulta(sqlExiste, pExiste);

                int existe = Convert.ToInt32(dtExiste.Rows[0]["Existe"]);
                if (existe > 0)
                {
                    lblMsg.Text = "Ese RUT ya existe en el sistema.";
                    return;
                }

                // CLAVE HASH igual que Ingreso.aspx.cs
                string claveHash = Seguridad.HashSha256(claveTemp);

                // 2) Insert en Usuarios (RolID = 2)
                string sqlInsertUsuario = @"
INSERT INTO dbo.Usuarios (RolID, Rut, Correo, ClaveHash, Telefono)
VALUES (2, @Rut, @Correo, @ClaveHash, NULL);

SELECT CAST(SCOPE_IDENTITY() AS INT) AS NuevoUsuarioID;";

                SqlParameter[] pU =
                {
                    new SqlParameter("@Rut", rut),
                    new SqlParameter("@Correo", correo),
                    new SqlParameter("@ClaveHash", claveHash)
                };

                DataTable dtNew = bd.EjecutarConsulta(sqlInsertUsuario, pU);
                int nuevoUsuarioId = Convert.ToInt32(dtNew.Rows[0]["NuevoUsuarioID"]);

                // 3) Insert en Colaboradores
                string sqlInsertColab = @"
INSERT INTO dbo.Colaboradores (UsuarioID, Nombre, Especialidad)
VALUES (@UsuarioID, @Nombre, @Especialidad);";

                SqlParameter[] pC =
                {
                    new SqlParameter("@UsuarioID", nuevoUsuarioId),
                    new SqlParameter("@Nombre", nombre),
                    new SqlParameter("@Especialidad", especialidad)
                };

                bd.EjecutarComando(sqlInsertColab, pC);

                // Limpiar inputs
                txtRut.Text = "";
                txtCorreo.Text = "";
                txtNombre.Text = "";
                txtEspecialidad.Text = "";
                txtClaveTemp.Text = "";

                lblMsg.Text = "Colaborador creado ✅ (ya puede iniciar sesión con su clave temporal).";
                CargarColaboradores();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "No se pudo crear. Detalle: " + ex.Message;
            }
        }

        
        //  EDITAR / ELIMINAR
        
        protected void rptColaboradores_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Text = "";

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int usuarioId))
                return;

            ConexionBD bd = new ConexionBD();

            if (e.CommandName == "editar")
            {
                TextBox txtNombreRow = (TextBox)e.Item.FindControl("txtNombreRow");
                TextBox txtEspecialidadRow = (TextBox)e.Item.FindControl("txtEspecialidadRow");

                string nombre = (txtNombreRow.Text ?? "").Trim();
                string especialidad = (txtEspecialidadRow.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(especialidad))
                {
                    lblMsg.Text = "Nombre y especialidad no pueden ir vacíos.";
                    return;
                }

                try
                {
                    string sqlUp = @"
UPDATE dbo.Colaboradores
SET Nombre = @Nombre,
    Especialidad = @Especialidad
WHERE UsuarioID = @UsuarioID;";

                    SqlParameter[] pUp =
                    {
                        new SqlParameter("@Nombre", nombre),
                        new SqlParameter("@Especialidad", especialidad),
                        new SqlParameter("@UsuarioID", usuarioId)
                    };

                    bd.EjecutarComando(sqlUp, pUp);
                    lblMsg.Text = "Colaborador actualizado ✅";
                    CargarColaboradores();
                }
                catch (Exception ex)
                {
                    lblMsg.Text = "No se pudo actualizar. Detalle: " + ex.Message;
                }
            }
            else if (e.CommandName == "eliminar")
            {
                try
                {
                    // Si hay citas asociadas, podría fallar por FK.
                    string sqlDelColab = @"DELETE FROM dbo.Colaboradores WHERE UsuarioID = @UsuarioID;";
                    SqlParameter[] p1 = { new SqlParameter("@UsuarioID", usuarioId) };
                    bd.EjecutarComando(sqlDelColab, p1);

                    string sqlDelUser = @"DELETE FROM dbo.Usuarios WHERE UsuarioID = @UsuarioID;";
                    SqlParameter[] p2 = { new SqlParameter("@UsuarioID", usuarioId) };
                    bd.EjecutarComando(sqlDelUser, p2);

                    lblMsg.Text = "Colaborador eliminado ✅";
                    CargarColaboradores();
                }
                catch (Exception ex)
                {
                    lblMsg.Text = "No se pudo eliminar (probablemente tiene citas asociadas). Detalle: " + ex.Message;
                }
            }
        }

        // MENÚ
      
        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }

        // HELPERS
    
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