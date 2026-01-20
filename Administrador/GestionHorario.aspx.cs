using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agenda.Administrador
{
    public partial class GestionHorario : Page
    {
           protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null || Convert.ToInt32(Session["RolID"]) != 3)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarColaboradores();
                lblMsg.Text = "";
            }
        }

        private void CargarColaboradores()
        {
            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT u.UsuarioID, c.Nombre
FROM dbo.Colaboradores c
INNER JOIN dbo.Usuarios u ON u.UsuarioID = c.UsuarioID
WHERE u.RolID = 2
ORDER BY c.Nombre;";

            DataTable dt = bd.EjecutarConsulta(sql, null);

            ddlColaboradores.DataSource = dt;
            ddlColaboradores.DataTextField = "Nombre";
            ddlColaboradores.DataValueField = "UsuarioID";
            ddlColaboradores.DataBind();

            ddlColaboradores.Items.Insert(0, new System.Web.UI.WebControls.ListItem("SELECCIONE COLABORADOR", ""));
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }
    }
}