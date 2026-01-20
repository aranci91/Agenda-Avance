using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agenda.Colaboradores
{
    public partial class InicioColaborador : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Seguridad
            if (Session["UsuarioID"] == null || Session["RolID"] == null)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            // Solo colaborador (RolID = 2)
            if (Convert.ToInt32(Session["RolID"]) != 2)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarNombreColaborador();
                CargarResumenHoy();
            }
        }

        private void CargarNombreColaborador()
        {
            try
            {
                int usuarioId = Convert.ToInt32(Session["UsuarioID"]);
                ConexionBD bd = new ConexionBD();

                string sql = @"
SELECT TOP 1 Nombre
FROM dbo.Colaboradores
WHERE UsuarioID = @UsuarioID;";

                SqlParameter[] p =
                {
                    new SqlParameter("@UsuarioID", usuarioId)
                };

                DataTable dt = bd.EjecutarConsulta(sql, p);

                lblNombreColaborador.Text = (dt.Rows.Count > 0)
                    ? Convert.ToString(dt.Rows[0]["Nombre"])
                    : "COLABORADOR";
            }
            catch
            {
                lblNombreColaborador.Text = "COLABORADOR";
            }
        }

        private void CargarResumenHoy()
        {
            try
            {
                int colaboradorId = Convert.ToInt32(Session["UsuarioID"]);
                ConexionBD bd = new ConexionBD();

                string sql = @"
SELECT COUNT(*) AS Total
FROM dbo.Citas
WHERE UsuarioID_Colaborador = @Colaborador
  AND CAST(FechaHora AS date) = CAST(GETDATE() AS date);";

                SqlParameter[] p =
                {
                    new SqlParameter("@Colaborador", colaboradorId)
                };

                DataTable dt = bd.EjecutarConsulta(sql, p);
                lblCitasHoy.Text = (dt.Rows.Count > 0) ? Convert.ToString(dt.Rows[0]["Total"]) : "0";
            }
            catch
            {
                lblCitasHoy.Text = "0";
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }
    }
}