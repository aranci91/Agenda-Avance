<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionServicios.aspx.cs" Inherits="Agenda.Administrador.GestionServicios" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Gestión de Servicios - Yessi Aranci</title>

<link rel="stylesheet" href="../styles.css" />
<link rel="shortcut icon" href="../imagenes/logo.ico" />
</head>

<body>
<form id="form1" runat="server">

<div class="app-shell">
<div class="panel-app">

    <!-- TOPBAR -->
    <div class="app-topbar">
        <div class="brand">
            <img src="../imagenes/logo.png" class="brand-logo" />
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

            <asp:Button runat="server"
                Text="GESTIÓN DE SERVICIOS"
                CssClass="menu-btn"
                Enabled="false" />

            <div class="sidebar-spacer"></div>

            <asp:Button runat="server"
                Text="Cerrar sesión"
                CssClass="menu-logout"
                OnClick="btnCerrarSesion_Click" />
        </div>

        <!-- CONTENT -->
        <div class="content">

            <div class="hola">Gestión de Servicios</div>
            <p class="sub">Administra los servicios disponibles del estudio.</p>

            <!-- LISTADO -->
            <div class="card">
                <div class="card-title">Servicios</div>

                <asp:Repeater ID="rptServicios" runat="server">
                    <ItemTemplate>

                        <div class="resena-item">

                            <div class="resena-top">

                                <!-- INFO -->
                                <div>
                                    <div class="resena-nombre">
                                        <%# Container.ItemIndex + 1 %>. <%# Eval("NombreServicio") %>
                                    </div>

                                    <div class="resena-rating">
                                        <%# Eval("DuracionMin") %> min ·
                                        <%# Eval("Precio", "{0:C0}") %>
                                    </div>
                                </div>

                                <!-- ACCIONES -->
                                <div style="display:flex; gap:12px; align-items:center;">

                                    <!-- SWITCH -->
                                    <asp:CheckBox
                                        runat="server"
                                        Checked='<%# Convert.ToBoolean(Eval("Activo")) %>'
                                        AutoPostBack="true"
                                        OnCheckedChanged="chkActivoServicio_CheckedChanged"
                                        CssClass="switch-grande"
                                        data-id='<%# Eval("ServicioID") %>' />

                                    <!-- EDITAR -->
                                    <asp:LinkButton
                                        runat="server"
                                        CommandArgument='<%# Eval("ServicioID") %>'
                                        OnCommand="EditarServicio"
                                        CssClass="icon-btn"
                                        ToolTip="Editar servicio">
                                        ✏️
                                    </asp:LinkButton>

                                    <!-- ELIMINAR -->
                                    <asp:LinkButton
                                        runat="server"
                                        CommandArgument='<%# Eval("ServicioID") %>'
                                        OnCommand="EliminarServicio"
                                        CssClass="icon-btn"
                                        ToolTip="Eliminar servicio"
                                        OnClientClick="return confirm('¿Seguro que deseas eliminar este servicio?');">
                                        🗑️
                                    </asp:LinkButton>

                                </div>
                            </div>

                        </div>

                    </ItemTemplate>
                </asp:Repeater>

            </div>

        </div>
    </div>
</div>
</div>

</form>
</body>
</html>