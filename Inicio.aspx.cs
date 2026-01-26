using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agenda
{
    public partial class Inicio : System.Web.UI.Page
    {
        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("Ingreso.aspx");
        }
    }
}
