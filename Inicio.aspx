<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inicio.aspx.cs" Inherits="Agenda.Inicio" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Yessi Aranci</title>
    <link rel="stylesheet" href="<%= ResolveUrl("~/styles.css") %>" />
    <link rel="shortcut icon" href="<%= ResolveUrl("~/imagenes/logo.ico") %>" />
</head>

<body>
    <form id="form1" runat="server">

        <div class="contenedor">
            <div id="MenuCentral">

                <img src="<%= ResolveUrl("~/imagenes/logo.png") %>" class="logo" alt="Yessi Aranci" />

                <h3>¡Bienvenida a Yessi Aranci!</h3>
                <p>Ingresa a tu sesión y agenda tu cita</p>

                <asp:Button 
                    ID="btnIngresar" 
                    runat="server"
                    Text="INGRESAR"
                    CssClass="btn"
                    OnClick="btnIngresar_Click" />

            </div>
        </div>

    </form>
</body>
</html>
