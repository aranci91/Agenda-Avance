using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;


namespace Agenda.Administrador
{
    public partial class Reportes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null || Session["Rol"] == null)
                Response.Redirect("~/Ingreso.aspx");

            if (Convert.ToInt32(Session["Rol"]) != 3)
                Response.Redirect("~/Ingreso.aspx");

            if (!IsPostBack)
            {
                CargarServiciosMasSolicitados();
                CargarTopClientes();
            }
        }

        private void CargarServiciosMasSolicitados()
        {
            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT s.NombreServicio, COUNT(*) Total
FROM Citas c
INNER JOIN Servicios s ON s.ServicioID = c.ServicioID
GROUP BY s.NombreServicio
ORDER BY Total DESC";

            DataTable dt = bd.EjecutarConsulta(sql, null);

            var servicios = new
            {
                labels = new System.Collections.Generic.List<string>(),
                data = new System.Collections.Generic.List<int>()
            };

            foreach (DataRow row in dt.Rows)
            {
                servicios.labels.Add(row["NombreServicio"].ToString());
                servicios.data.Add(Convert.ToInt32(row["Total"]));
            }

            hfServicios.Value = JsonConvert.SerializeObject(servicios);
        }

        private void CargarTopClientes()
        {
            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT TOP 5 c.Nombre, COUNT(*) Total
FROM Citas ci
INNER JOIN Clientes c ON c.UsuarioID = ci.UsuarioID_Cliente
GROUP BY c.Nombre
ORDER BY Total DESC";

            DataTable dt = bd.EjecutarConsulta(sql, null);

            var clientes = new
            {
                labels = new System.Collections.Generic.List<string>(),
                data = new System.Collections.Generic.List<int>()
            };

            foreach (DataRow row in dt.Rows)
            {
                clientes.labels.Add(row["Nombre"].ToString());
                clientes.data.Add(Convert.ToInt32(row["Total"]));
            }

            hfClientes.Value = JsonConvert.SerializeObject(clientes);
        }

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