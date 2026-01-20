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
    public partial class GestionServicios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Seguridad: solo ADMIN (RolID = 3)
            if (Session["UsuarioID"] == null || Convert.ToInt32(Session["RolID"]) != 3)
            {
                Response.Redirect("~/Ingreso.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarServicios();
            }
        }

        // CARGAR LISTA DE SERVICIOS
 
        private void CargarServicios()
        {
            ConexionBD bd = new ConexionBD();

            string sql = @"
                SELECT ServicioID, NombreServicio, DuracionMin, Precio, Activo
                FROM dbo.Servicios
                ORDER BY NombreServicio";

            DataTable dt = bd.EjecutarConsulta(sql, null);

            rptServicios.DataSource = dt;
            rptServicios.DataBind();
        }

       
        // ACTIVAR / DESACTIVAR SERVICIO (SWITCH)
     
        protected void chkActivoServicio_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            // Recuperamos el ServicioID desde el atributo data-id
            int servicioId = Convert.ToInt32(chk.Attributes["data-id"]);
            bool activo = chk.Checked;

            ConexionBD bd = new ConexionBD();

            string sql = @"
                UPDATE dbo.Servicios
                SET Activo = @Activo
                WHERE ServicioID = @ServicioID";

            SqlParameter[] p =
            {
                new SqlParameter("@Activo", activo),
                new SqlParameter("@ServicioID", servicioId)
            };

            bd.EjecutarComando(sql, p);

            // Recargar lista para reflejar cambios
            CargarServicios();
        }


        // EDITAR SERVICIO 

        protected void EditarServicio(object sender, CommandEventArgs e)
        {
            int servicioId = Convert.ToInt32(e.CommandArgument);
            Response.Redirect("PanelServicio.aspx?id=" + servicioId);
        }



        // NAVEGACIÓN

        protected void btnInicio_Click(object sender, EventArgs e)
        {
            Response.Redirect("InicioAdministrador.aspx");
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Ingreso.aspx");
        }

    }
}