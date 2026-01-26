<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MisDatos.aspx.cs" Inherits="Agenda.Administrador.MisDatos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Mis Datos - Yessi Aranci</title>

<link rel="stylesheet" href="<%= ResolveUrl("~/styles.css") %>" />
</head>

<body>
<form id="form1" runat="server">

<div class="app-shell">
<div class="panel-app">

<!-- TOP -->
<div class="app-topbar">
    <div class="brand">
        <img src="<%= ResolveUrl("~/imagenes/logo.png") %>" class="brand-logo" />
        <div class="brand-name">YESSI ARANCI</div>
    </div>
</div>

<div class="app-body">

<!-- SIDEBAR -->
<div class="sidebar">
    <div class="menu-title">MENÚ</div>

    <asp:Button runat="server" Text="INICIO" CssClass="menu-btn" OnClick="btnInicio_Click" />

    <div class="sidebar-spacer"></div>

    <asp:Button runat="server" Text="Cerrar sesión" CssClass="menu-logout" OnClick="btnCerrarSesion_Click" />
</div>

<!-- CONTENT -->
<div class="content">

<h1>Mis Datos</h1>
<p class="sub">Actualiza tu información personal</p>

<asp:Label ID="lblMsg" runat="server" CssClass="mensaje-app"></asp:Label>

<div class="card">

<div class="form-row">
    <asp:TextBox ID="txtRut" runat="server" CssClass="textbox" ReadOnly="true" placeholder="RUT"></asp:TextBox>
</div>

<div class="form-row">
    <asp:TextBox ID="txtNombre" runat="server" CssClass="textbox" placeholder="Nombre"></asp:TextBox>
</div>

<div class="form-row">
    <asp:TextBox ID="txtApellido" runat="server" CssClass="textbox" placeholder="Apellido"></asp:TextBox>
</div>

<div class="form-row">
    <asp:TextBox ID="txtTelefono" runat="server" CssClass="textbox" placeholder="Teléfono"></asp:TextBox>
</div>

<div class="form-row">
    <asp:TextBox ID="txtCorreo" runat="server" CssClass="textbox" placeholder="Correo"></asp:TextBox>
</div>

<div class="form-row">
    <asp:TextBox ID="txtPassword" runat="server" CssClass="textbox" TextMode="Password" placeholder="Nueva contraseña (opcional)"></asp:TextBox>
</div>

<asp:Button ID="btnGuardar" runat="server" Text="GUARDAR CAMBIOS" CssClass="btn-confirm" OnClick="btnGuardar_Click" />

</div>

</div>
</div>
</div>

</form>
</body>
</html>
