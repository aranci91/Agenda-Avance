<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionColaboradores.aspx.cs" Inherits="Agenda.Administrador.GestionColaboradores" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>Gestión de Colaboradores - Yessi Aranci</title>

<link rel="stylesheet" href="<%= ResolveUrl("~/styles.css") %>" />
<link rel="shortcut icon" href="<%= ResolveUrl("~/imagenes/logo.ico") %>" />

<style>
    .grid-card{
        background:#fff;
        border-radius:18px;
        padding:16px;
        box-shadow:0 6px 18px rgba(0,0,0,0.06);
        margin-top:14px;
    }
    .form-row{
        display:flex;
        gap:10px;
        flex-wrap:wrap;
    }
    .form-row .textbox, .form-row .select{
        flex:1;
        min-width:220px;
    }
    .btn-danger{
        background:#ffd0d0;
        border:none;
        border-radius:999px;
        padding:8px 14px;
        font-weight:800;
        cursor:pointer;
    }
    .btn-mini{
        border:none;
        border-radius:999px;
        padding:7px 12px;
        font-weight:800;
        cursor:pointer;
    }
</style>
</head>

<body>
<form id="form1" runat="server">

<div class="app-shell">
<div class="panel-app">

    <!-- TOP BAR -->
    <div class="app-topbar">
        <div class="brand">
            <img src="<%= ResolveUrl("~/imagenes/logo.png") %>" class="brand-logo" alt="Yessi Aranci" />
            <div class="brand-name">YESSI ARANCI</div>
        </div>
    </div>

    <div class="app-body">

        <!-- SIDEBAR -->
        <div class="sidebar">
            <div class="menu-title">MENÚ</div>

            <asp:Button runat="server"
                Text="VOLVER"
                CssClass="menu-btn"
                PostBackUrl="~/Admin/InicioAdmin.aspx" />

            <div class="sidebar-spacer"></div>

            <asp:Button ID="btnCerrarSesion" runat="server"
                Text="Cerrar sesión"
                CssClass="menu-logout"
                OnClick="btnCerrarSesion_Click" />
        </div>

        <!-- CONTENT -->
        <div class="content">
            <h1>Gestión de Colaboradores</h1>
            <p class="sub">Crea, edita o elimina colaboradores del sistema.</p>

            <asp:Label ID="lblMsg" runat="server" CssClass="mensaje-app"></asp:Label>

            <!-- CREAR -->
            <div class="card">
                <div class="card-title">Crear colaborador</div>

                <div class="form-row">
                    <asp:TextBox ID="txtRut" runat="server" CssClass="textbox" placeholder="RUT (ej: 12345678-9)" />
                    <asp:TextBox ID="txtCorreo" runat="server" CssClass="textbox" placeholder="Correo" />
                </div>

                <div class="form-row" style="margin-top:10px;">
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="textbox" placeholder="Nombre colaborador" />
                    <asp:TextBox ID="txtEspecialidad" runat="server" CssClass="textbox" placeholder="Especialidad (ej: Lifting de pestañas)" />
                </div>

                <div class="form-row" style="margin-top:10px;">
                    <asp:TextBox ID="txtClaveTemp" runat="server" CssClass="textbox" placeholder="Clave temporal (ej: 1234)" />
                </div>

                <div style="margin-top:12px;">
                    <asp:Button ID="btnCrear" runat="server"
                        Text="CREAR COLABORADOR"
                        CssClass="btn-confirm"
                        OnClick="btnCrear_Click" />
                </div>
            </div>

            <!-- LISTADO -->
            <div class="grid-card">
                <div class="card-title">Colaboradores registrados</div>

                <asp:Repeater ID="rptColaboradores" runat="server" OnItemCommand="rptColaboradores_ItemCommand">
                    <HeaderTemplate>
                        <table style="width:100%; border-collapse:collapse; margin-top:10px;">
                            <thead>
                                <tr style="text-align:left; opacity:0.9;">
                                    <th style="padding:8px;">UsuarioID</th>
                                    <th style="padding:8px;">RUT</th>
                                    <th style="padding:8px;">Correo</th>
                                    <th style="padding:8px;">Nombre</th>
                                    <th style="padding:8px;">Especialidad</th>
                                    <th style="padding:8px;">Acciones</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>

                    <ItemTemplate>
                        <tr style="border-top:1px solid #eee;">
                            <td style="padding:8px;"><%# Eval("UsuarioID") %></td>
                            <td style="padding:8px;"><%# Eval("Rut") %></td>
                            <td style="padding:8px;"><%# Eval("Correo") %></td>

                            <td style="padding:8px;">
                                <asp:TextBox ID="txtNombreRow" runat="server" CssClass="textbox"
                                    Text='<%# Eval("Nombre") %>' />
                            </td>

                            <td style="padding:8px;">
                                <asp:TextBox ID="txtEspecialidadRow" runat="server" CssClass="textbox"
                                    Text='<%# Eval("Especialidad") %>' />
                            </td>

                            <td style="padding:8px; white-space:nowrap;">
                                <asp:Button runat="server"
                                    Text="Guardar"
                                    CssClass="btn-mini"
                                    CommandName="editar"
                                    CommandArgument='<%# Eval("UsuarioID") %>' />

                                <asp:Button runat="server"
                                    Text="Eliminar"
                                    CssClass="btn-danger"
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

                <asp:Label ID="lblVacio" runat="server" Text=""
                    style="display:block; margin-top:10px; font-weight:700; opacity:0.9;"></asp:Label>
            </div>

        </div>
    </div>

</div>
</div>

</form>
</body>
</html>