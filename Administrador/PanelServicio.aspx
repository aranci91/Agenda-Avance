<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PanelServicio.aspx.cs" Inherits="Agenda.Administrador.PanelServicio" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>Panel de Servicio - Yessi Aranci</title>

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
                Text="GESTIÓN DE SERVICIOS"
                CssClass="menu-btn"
                OnClick="btnVolver_Click" />

            <div class="sidebar-spacer"></div>

            <asp:Button runat="server"
                Text="Cerrar sesión"
                CssClass="menu-logout"
                OnClick="btnCerrarSesion_Click" />
        </div>

        <!-- CONTENT -->
        <div class="content">

            <div class="hola">
                <asp:Label ID="lblTitulo" runat="server" Text="Nuevo servicio" />
            </div>

            <!-- SERVICIO -->
            <div class="card">
                <div class="card-title">Datos del servicio</div>

                <div class="form-col">

                    <asp:HiddenField ID="hfServicioID" runat="server" />

                    <asp:TextBox ID="txtNombre" runat="server"
                        CssClass="textbox"
                        placeholder="Nombre del servicio" />

                    <asp:TextBox ID="txtDuracion" runat="server"
                        CssClass="textbox"
                        placeholder="Duración (minutos)" />

                    <asp:TextBox ID="txtPrecio" runat="server"
                        CssClass="textbox"
                        placeholder="Precio" />

                    <div class="check-row">
                        <asp:CheckBox ID="chkActivo"
                            runat="server"
                            Checked="true" />
                        <span>Servicio activo</span>
                    </div>

                </div>
            </div>

            <!-- COLABORADOR -->
            <div class="card">
                <div class="card-title">Asignar colaborador</div>

                <div class="form-col">

                    <asp:DropDownList ID="ddlColaboradores"
                        runat="server"
                        CssClass="select" />

                   <div class="check-row">
                            <asp:CheckBox ID="chkColabActivo"
                                runat="server"
                                Checked="true" />
                            <span>Activo para este servicio</span>
                        </div>

                </div>
            </div>

            <!-- GUARDAR -->
            <div class="card">
                <asp:Button ID="btnGuardar"
                    runat="server"
                    Text="GUARDAR SERVICIO"
                    CssClass="btn-confirm"
                    OnClick="btnGuardar_Click" />

                <asp:Label ID="lblMensaje"
                    runat="server"
                    CssClass="mensaje-app" />
            </div>

        </div>
    </div>
</div>
</div>

</form>
</body>
</html>