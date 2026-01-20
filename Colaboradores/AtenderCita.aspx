<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AtenderCita.aspx.cs" Inherits="Agenda.Colaboradores.AtenderCita" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>Atender Cita - Yessi Aranci</title>
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
                Text="VOLVER"
                CssClass="menu-btn"
                PostBackUrl="~/Colaboradores/AgendaColaborador.aspx" />

            <div class="sidebar-spacer"></div>

            <asp:Button ID="btnCerrarSesion" runat="server"
                Text="Cerrar sesión"
                CssClass="menu-logout"
                OnClick="btnCerrarSesion_Click" />
        </div>

        <div class="content">
            <h1>Registrar Atención</h1>
            <p class="sub">Escribe una observación y marca la cita como atendida.</p>

            <div class="card">
                <div class="card-title">Detalle</div>
                <p style="margin-top:8px;">
                    <strong>Servicio:</strong> <asp:Label ID="lblServicio" runat="server" Text="-"></asp:Label><br/>
                    <strong>Cliente:</strong> <asp:Label ID="lblCliente" runat="server" Text="-"></asp:Label><br/>
                    <strong>Fecha:</strong> <asp:Label ID="lblFecha" runat="server" Text="-"></asp:Label>
                </p>

                <div style="margin-top:12px; font-weight:800;">Observación</div>

                <asp:TextBox ID="txtObs" runat="server"
                    CssClass="textbox"
                    TextMode="MultiLine"
                    Rows="4"
                    placeholder="Ej: se realizó servicio sin novedades..."></asp:TextBox>

                <asp:Button ID="btnGuardar" runat="server"
                    Text="MARCAR COMO ATENDIDA"
                    CssClass="btn-confirm"
                    OnClick="btnGuardar_Click"
                    style="margin-top:12px;" />

                <asp:Label ID="lblMsg" runat="server" CssClass="mensaje-app"></asp:Label>
            </div>

        </div>
    </div>

</div>
</div>

</form>
</body>
</html>
