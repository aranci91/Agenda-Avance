using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agenda.Clientes
{
    public partial class AgendaCliente : System.Web.UI.Page
    {
        protected int UsuarioID_Cliente;
        protected string NombreCliente;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Validar sesión
            if (Session["UsuarioID"] == null || Session["Nombre"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // datos del cliente logueado
            UsuarioID_Cliente = Convert.ToInt32(Session["UsuarioID"]);
            NombreCliente = Session["Nombre"].ToString();
        }
    }
}
