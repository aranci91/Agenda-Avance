using Agenda.Servicios;
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
    public partial class InicioAdministrador : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Seguridad: sesión obligatoria
            if (Session["UsuarioID"] == null || Session["RolID"] == null)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            // SOLO ADMIN (RolID = 3)
            if (Convert.ToInt32(Session["RolID"]) != 3)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarResenasCarrusel();
                lblEnvioMsg.Text = "";
            }
        }

        private void CargarResenasCarrusel()
        {
            lblSinResenas.Text = "";
            carWrap.Visible = false;

            ConexionBD bd = new ConexionBD();

            // ✅ SOLO visibles + últimas 10 + solo NOMBRE
            string query = @"
SELECT TOP 10
    c.Nombre AS NombreCliente,
    r.Calificacion,
    ISNULL(r.Comentario,'') AS Comentario
FROM dbo.Reseñas r
INNER JOIN dbo.Atenciones a ON a.AtencionID = r.AtencionID
INNER JOIN dbo.Citas ci ON ci.CitaID = a.CitaID
INNER JOIN dbo.Clientes c ON c.UsuarioID = ci.UsuarioID_Cliente
WHERE r.Visible = 1
ORDER BY r.ReseñaID DESC;";

            DataTable dt = bd.EjecutarConsulta(query, null);

            if (dt.Rows.Count == 0)
            {
                rptResenas.DataSource = null;
                rptResenas.DataBind();
                lblSinResenas.Text = "No hay valoraciones visibles aún.";
                return;
            }

            rptResenas.DataSource = dt;
            rptResenas.DataBind();
            carWrap.Visible = true;
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            lblEnvioMsg.Text = "";

            try
            {
                string asunto = (txtAsunto.Text ?? "").Trim();
                string cuerpoEditor = (hfHtml.Value ?? "").Trim();

                if (string.IsNullOrWhiteSpace(asunto))
                {
                    lblEnvioMsg.Text = "Debes escribir un asunto.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(cuerpoEditor))
                {
                    lblEnvioMsg.Text = "Debes escribir un mensaje antes de enviar.";
                    return;
                }

                // ✅ Obtener correos clientes (RolID = 1)
                List<string> correos = ObtenerCorreosClientes();

                if (correos.Count == 0)
                {
                    lblEnvioMsg.Text = "No hay correos de clientes registrados para enviar.";
                    return;
                }

                // ✅ Logo absoluto (para que lo vean en el correo)
                string logoAbs = ObtenerLogoAbsoluto();

                // ✅ Armar HTML final con estilo Yessi Aranci
                string htmlFinal = $@"
<div style='font-family: Arial, sans-serif; line-height:1.6; color:#222;'>
  <div style='background:#fff1f7; border:1px solid #ffd0e3; border-radius:16px; padding:16px 18px;'>
    {cuerpoEditor}
  </div>

  <div style='margin-top:16px; padding-top:12px; border-top:1px solid #eee; display:flex; gap:12px; align-items:center;'>
    <img src='{logoAbs}' alt='Yessi Aranci' style='width:52px; height:auto; border-radius:12px;'/>
    <div style='font-weight:900; letter-spacing:0.5px;'>Yessi Aranci</div>
  </div>

  <div style='font-size:12px; color:#777; margin-top:8px;'>
    Este mensaje fue enviado desde el sistema de agenda del estudio.
  </div>
</div>";

                // ✅ Enviar masivo
                EmailService.EnviarMasivoClientes(correos, asunto, htmlFinal);

                // Limpieza
                txtAsunto.Text = "";
                hfHtml.Value = "";

                lblEnvioMsg.Text = $"✅ Mensaje enviado a {correos.Count} clientes.";
            }
            catch (Exception ex)
            {
                lblEnvioMsg.Text = "No se pudo enviar. Detalle: " + ex.Message;
            }
        }

        private List<string> ObtenerCorreosClientes()
        {
            ConexionBD bd = new ConexionBD();

            // ✅ asumiendo que el correo está en dbo.Usuarios.Correo y RolID=1
            string sql = @"
SELECT DISTINCT u.Correo
FROM dbo.Usuarios u
WHERE u.RolID = 1
  AND u.Correo IS NOT NULL
  AND LTRIM(RTRIM(u.Correo)) <> '';";

            DataTable dt = bd.EjecutarConsulta(sql, null);

            List<string> lista = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                string correo = Convert.ToString(row["Correo"]).Trim();
                if (!string.IsNullOrWhiteSpace(correo))
                    lista.Add(correo);
            }

            return lista;
        }

        private string ObtenerLogoAbsoluto()
        {
            // ejemplo: https://tusitio.cl/imagenes/logo.png
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            string rel = ResolveUrl("~/imagenes/logo.png");
            return baseUrl + rel;
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }

        protected void btnGestionServicios_Click(object sender, EventArgs e)
        {
            Response.Redirect("GestionServicios.aspx");
        }

        protected void btnPanelServicios_Click(object sender, EventArgs e)
        {
            Response.Redirect("PanelServicio.aspx");
        }

        protected void btnGestionHorario_Click(object sender, EventArgs e)
        {
            Response.Redirect("GestionHorario.aspx");
        }

        protected void btnGestionColaboradores_Click(object sender, EventArgs e)
        {
            Response.Redirect("GestionColaboradores.aspx");
        }
    }
}
