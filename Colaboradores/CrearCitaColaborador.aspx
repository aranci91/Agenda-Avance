<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CrearCitaColaborador.aspx.cs" Inherits="Agenda.Colaboradores.CrearCitaColaborador" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>Crear Cita - Colaborador - Yessi Aranci</title>

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
            <img src="<%= ResolveUrl("~/imagenes/logo.png") %>" class="brand-logo" alt="Yessi Aranci" />
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
                PostBackUrl="~/Colaboradores/InicioColaborador.aspx" />

            <asp:Button runat="server"
                Text="MI AGENDA"
                CssClass="menu-btn"
                PostBackUrl="~/Colaboradores/AgendaColaborador.aspx" />

            <asp:Button runat="server"
                Text="CREAR CITA"
                CssClass="menu-btn"
                Enabled="false" />

            <div class="sidebar-spacer"></div>

            <asp:Button ID="btnCerrarSesion" runat="server"
                Text="Cerrar sesión"
                CssClass="menu-logout"
                OnClick="btnCerrarSesion_Click" />
        </div>

        <!-- CONTENT -->
        <div class="content">

            <div class="hola">Crear cita</div>
            <p class="sub">Crea una cita para un cliente ya registrado (desde hoy en adelante).</p>

            <!-- CARD: BUSCAR CLIENTE -->
            <div class="card">
                <div class="card-title">Buscar cliente</div>

                <div class="form-col">

                    <asp:TextBox ID="txtBuscarCliente" runat="server"
                        CssClass="textbox"
                        placeholder="Buscar por RUT o nombre (ej: 12345678-9 o María)"
                        />

                    <asp:Button ID="btnBuscarCliente" runat="server"
                        Text="BUSCAR"
                        CssClass="btn-confirm"
                        OnClick="btnBuscarCliente_Click" />

                    <asp:Label ID="lblMsgBusqueda" runat="server" Text=""
                        style="display:block; margin-top:8px; color:#b00020; font-weight:600;"></asp:Label>

                    <asp:DropDownList ID="ddlClientes" runat="server"
                        CssClass="select">
                    </asp:DropDownList>

                    <asp:Label ID="lblClienteSeleccionado" runat="server" Text=""
                        style="display:block; margin-top:6px; color:#444; opacity:0.9;"></asp:Label>

                </div>
            </div>

            <!-- CARD: DATOS CITA -->
            <div class="card">
                <div class="card-title">Datos de la cita</div>

                <div class="form-col">

                    <asp:DropDownList ID="ddlServicios" runat="server"
                        CssClass="select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlServicios_SelectedIndexChanged">
                    </asp:DropDownList>

                    <!-- Resumen del servicio -->
                    <div style="margin-top:-4px; margin-left:6px; font-size:13px; color:#333; opacity:0.9;">
                        <div>
                            <strong>Duración:</strong>
                            <asp:Label ID="lblDuracionServicio" runat="server" Text="-" />
                        </div>
                        <div>
                            <strong>Valor:</strong>
                            <asp:Label ID="lblPrecioServicio" runat="server" Text="-" />
                        </div>
                    </div>

                    <!-- 🔴 CAMBIO CLAVE: fecha solo desde HOY (min se setea en code-behind)
                         + placeholder estilo bonito como tu RegistroCliente -->
                    <asp:TextBox ID="txtFecha" runat="server"
                        CssClass="textbox"
                        placeholder="Fecha de la cita"
                        AutoPostBack="true"
                        OnTextChanged="txtFecha_TextChanged"
                        onfocus="this.type='date'"
                        onblur="if(this.value==''){this.type='text'}"
                        type="text">
                    </asp:TextBox>

                    <asp:DropDownList ID="ddlHora" runat="server" CssClass="select"></asp:DropDownList>

                    <asp:Label ID="lblSinHoras" runat="server" Text=""
                        style="display:block; margin-top:8px; color:#b00020; font-weight:600;"></asp:Label>

                    <asp:Button ID="btnConfirmar" runat="server"
                        Text="CONFIRMAR CITA"
                        CssClass="btn-confirm"
                        OnClick="btnConfirmar_Click" />

                    <asp:Label ID="lblMensajeCita" runat="server" Text="" CssClass="mensaje-app"></asp:Label>
                </div>
            </div>

        </div>
    </div>
</div>
</div>

</form>
</body>
</html>
