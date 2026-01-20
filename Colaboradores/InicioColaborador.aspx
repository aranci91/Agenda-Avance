<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InicioColaborador.aspx.cs" Inherits="Agenda.Colaboradores.InicioColaborador" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>Inicio Colaborador - Yessi Aranci</title>
<link rel="stylesheet" href="<%= ResolveUrl("~/styles.css") %>" />
<link rel="shortcut icon" href="<%= ResolveUrl("~/imagenes/logo.ico") %>" />
</head>

<body>
<form id="form1" runat="server">

<div class="app-shell">
<div class="panel-app">

    <div class="app-topbar">
        <div class="brand">
            <img src="<%= ResolveUrl("~/imagenes/logo.png") %>" class="brand-logo" alt="Yessi Aranci" />
            <div class="brand-name">YESSI ARANCI</div>
        </div>
    </div>

    <div class="app-body">

        <div class="sidebar">
            <div class="menu-title">MENÚ</div>

            <asp:Button runat="server"
                Text="MI AGENDA"
                CssClass="menu-btn"
                PostBackUrl="~/Colaboradores/AgendaColaborador.aspx" />

            <asp:Button runat="server"
                Text="CREAR CITA"
                CssClass="menu-btn"
                PostBackUrl="~/Colaboradores/CrearCitaColaborador.aspx" />

            <div class="sidebar-spacer"></div>

            <asp:Button ID="btnCerrarSesion" runat="server"
                Text="Cerrar sesión"
                CssClass="menu-logout"
                OnClick="btnCerrarSesion_Click" />
        </div>

        <div class="content">
            <div class="hola">
                ¡Bienvenid@ <asp:Label ID="lblNombreColaborador" runat="server" Text="COLABORADOR"></asp:Label>!
            </div>

            <p class="sub">Desde aquí podrás ver tu agenda, confirmar citas y registrar atenciones.</p>

            <div class="card">
                <div class="card-title">Resumen de hoy</div>
                <p style="margin-top:8px;">
                    Citas de hoy: <strong><asp:Label ID="lblCitasHoy" runat="server" Text="0"></asp:Label></strong>
                </p>

                <asp:Button runat="server"
                    Text="IR A MI AGENDA"
                    CssClass="btn-confirm"
                    PostBackUrl="~/Colaboradores/AgendaColaborador.aspx"
                    style="margin-top:10px;" />
            </div>

            <asp:Label ID="lblMsg" runat="server" CssClass="mensaje-app"></asp:Label>
        </div>

    </div>
</div>
</div>

</form>
</body>
</html>
