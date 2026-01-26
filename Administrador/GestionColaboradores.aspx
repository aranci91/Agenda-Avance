<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionColaboradores.aspx.cs" Inherits="Agenda.Administrador.GestionColaboradores" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

  <title>Gestión de Colaboradores - Yessi Aranci</title>

<link rel="stylesheet" href="<%= ResolveUrl("~/styles.css") %>" />
<link rel="shortcut icon" href="<%= ResolveUrl("~/imagenes/logo.ico") %>" />
</head>

<body>
<form id="form1" runat="server">

<asp:HiddenField ID="hfUsuarioID" runat="server" />

<div class="app-shell">
<div class="panel-app">

<div class="app-topbar">
    <div class="brand">
        <img src="<%= ResolveUrl("~/imagenes/logo.png") %>" class="brand-logo" />
        <div class="brand-name">YESSI ARANCI</div>
    </div>
</div>

<div class="app-body">

<div class="sidebar">
    <div class="menu-title">MENÚ</div>

    <asp:Button runat="server" Text="INICIO" CssClass="menu-btn" OnClick="btnInicio_Click" />

    <div class="sidebar-spacer"></div>

    <asp:Button ID="btnCerrarSesion" runat="server" Text="Cerrar sesión" CssClass="menu-logout" OnClick="btnCerrarSesion_Click" />
</div>

<div class="content">
    <asp:HiddenField ID="HiddenField1" runat="server" />

<h1>Gestión de Colaboradores</h1>
<p class="sub">Crea, edita o elimina colaboradores del sistema.</p>

<asp:Label ID="lblMsg" runat="server" CssClass="mensaje-app"></asp:Label>

<div class="card">
<div class="card-title">Crear / Editar colaborador</div>

<div class="form-grid">
    <asp:TextBox ID="txtRut" runat="server" CssClass="textbox" placeholder="RUT (ej: 12345678-9)" />
    <asp:TextBox ID="txtTelefono" runat="server" CssClass="textbox" placeholder="Teléfono" />
    <asp:TextBox ID="txtCorreo" runat="server" CssClass="textbox" placeholder="Correo" />
    <asp:TextBox ID="txtNombre" runat="server" CssClass="textbox" placeholder="Nombre completo" />
    <asp:TextBox ID="txtEspecialidad" runat="server" CssClass="textbox" placeholder="Especialidad" />
    <asp:TextBox ID="txtClaveTemp" runat="server" CssClass="textbox" placeholder="Clave temporal" TextMode="Password" />
</div>

<asp:Button ID="btnCrear" runat="server" Text="CREAR COLABORADOR" CssClass="btn-confirm" OnClick="btnCrear_Click" />

</div>

<div class="grid-card">
<div class="card-title">Colaboradores registrados</div>

<div style="overflow-x:auto;">

<asp:Repeater ID="rptColaboradores" runat="server" OnItemCommand="rptColaboradores_ItemCommand">
<HeaderTemplate>
<table style="width:100%; border-collapse:collapse;">
<thead>
<tr>
<th>RUT</th>
<th>Teléfono</th>
<th>Correo</th>
<th>Nombre</th>
<th>Especialidad</th>
<th></th>
</tr>
</thead>
<tbody>
</HeaderTemplate>

<ItemTemplate>
<tr style="border-top:1px solid #eee;">
<td><%# Eval("Rut") %></td>
<td><%# Eval("Telefono") %></td>
<td><%# Eval("Correo") %></td>
<td><%# Eval("Nombre") %></td>
<td><%# Eval("Especialidad") %></td>
<td style="text-align:center;">
<div style="display:flex; flex-direction:column; gap:6px;">
<asp:Button runat="server" Text="✏️" CssClass="icon-btn edit"
CommandName="editar" CommandArgument='<%# Eval("UsuarioID") %>' />
<asp:Button runat="server" Text="🗑️" CssClass="icon-btn delete"
CommandName="eliminar" CommandArgument='<%# Eval("UsuarioID") %>' />
</div>
</td>
</tr>
</ItemTemplate>

<FooterTemplate>
</tbody>
</table>
</FooterTemplate>
</asp:Repeater>

</div>

<asp:Label ID="lblVacio" runat="server"></asp:Label>

</div>

</div>
</div>
</div>
</div>

</form>
</body>
</html>