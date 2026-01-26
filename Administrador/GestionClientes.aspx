<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionClientes.aspx.cs" Inherits="Agenda.Administrador.GestionClientes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta name="viewport" content="width=device-width, initial-scale=1.0" />

<title>Gestión de Clientes - Yessi Aranci</title>

<link rel="stylesheet" href="<%= ResolveUrl("~/styles.css") %>" />
<link rel="shortcut icon" href="<%= ResolveUrl("~/imagenes/logo.ico") %>" />
</head>

<body>
<form id="form1" runat="server">

<div class="app-shell">
<div class="panel-app">

<!-- TOP BAR -->
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

    <asp:Button runat="server"
        Text="INICIO"
        CssClass="menu-btn"
        OnClick="btnInicio_Click" />

    <div class="sidebar-spacer"></div>

    <asp:Button ID="btnCerrarSesion" runat="server"
        Text="Cerrar sesión"
        CssClass="menu-logout"
        OnClick="btnCerrarSesion_Click" />
</div>

<!-- CONTENT -->
<div class="content">

<h1>Gestión de Clientes</h1>
<p class="sub">Listado de clientes registrados en el sistema.</p>

<asp:Label ID="lblMsg" runat="server" CssClass="mensaje-app"></asp:Label>

<div class="grid-card">

<div class="card-title">Clientes registrados</div>

<div style="overflow-x:auto;">

    <div class="card" style="margin-bottom:16px;">

    <div class="card-title">Buscar cliente</div>

    <div class="form-row">
        <asp:TextBox ID="txtBuscar" runat="server" CssClass="textbox"
            placeholder="Buscar por nombre o apellido..." />

        <asp:Button ID="btnBuscar" runat="server"
            Text="Buscar"
            CssClass="btn-confirm"
            OnClick="btnBuscar_Click" />
    </div>

</div>


<asp:Repeater ID="rptClientes" runat="server" OnItemCommand="rptClientes_ItemCommand">
    <HeaderTemplate>
        <table style="width:100%; border-collapse:collapse; margin-top:10px;">
            <thead>
                <tr>
                    <th>RUT</th>
                    <th>Teléfono</th>
                    <th>Correo</th>
                    <th>Nombre</th>
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

            <td style="text-align:center;">
                <asp:Button runat="server"
                    Text="🗑️"
                    CssClass="icon-btn delete"
                    CommandName="eliminar"
                    CommandArgument='<%# Eval("UsuarioID") %>' />
            </td>
        </tr>
    </ItemTemplate>

    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
</asp:Repeater>

</div>

<asp:Label ID="lblVacio" runat="server" CssClass="mensaje-app"></asp:Label>

</div>

</div>
</div>

</div>
</div>

</form>
</body>
</html>