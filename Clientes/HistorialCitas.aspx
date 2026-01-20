<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HistorialCitas.aspx.cs" Inherits="Agenda.Clientes.HistorialCitas" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>Historial de Citas - Yessi Aranci</title>

<link rel="stylesheet" href="<%= ResolveUrl("~/styles.css") %>" />
<link rel="shortcut icon" href="<%= ResolveUrl("~/imagenes/logo.ico") %>" />

<style>
    .hist-item{
        background:#fff;
        border-radius:18px;
        padding:14px 16px;
        margin-bottom:12px;
        box-shadow:0 6px 18px rgba(0,0,0,0.06);
    }
    .hist-top{
        display:flex;
        justify-content:space-between;
        align-items:center;
        gap:12px;
        flex-wrap:wrap;
    }
    .hist-servicio{
        font-weight:900;
        font-size:16px;
    }
    .hist-fecha{
        font-weight:700;
        opacity:0.9;
    }
    .hist-sub{
        margin-top:6px;
        display:flex;
        gap:10px;
        flex-wrap:wrap;
        font-size:13px;
        opacity:0.9;
    }
    .badge{
        padding:4px 10px;
        border-radius:999px;
        font-weight:800;
        font-size:12px;
        display:inline-block;
    }
    .b-pendiente{ background:#fff1b8; }
    .b-confirmada{ background:#c8f7d2; }
    .b-cancelada{ background:#ffd0d0; }
    .b-atendida{ background:#d6e7ff; }

    .btn-cancelar{
        background:#ff9fc9;
        border:none;
        border-radius:999px;
        padding:8px 14px;
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
                PostBackUrl="~/Clientes/InicioCliente.aspx" />

            <div class="sidebar-spacer"></div>

            <asp:Button ID="btnCerrarSesion" runat="server"
                Text="Cerrar sesión"
                CssClass="menu-logout"
                OnClick="btnCerrarSesion_Click" />
        </div>

        <!-- CONTENT -->
        <div class="content">
            <h1>Historial de Citas</h1>
            <p class="sub">Revisa tus horas agendadas y su estado.</p>

            <asp:Label ID="lblMsg" runat="server" CssClass="mensaje-app"></asp:Label>

            <asp:Repeater ID="rptCitas" runat="server" OnItemCommand="rptCitas_ItemCommand">
                <ItemTemplate>
                    <div class="hist-item">
                        <div class="hist-top">
                            <div>
                                <div class="hist-servicio"><%# Eval("Servicio") %></div>
                                <div class="hist-fecha"><%# Eval("FechaHoraTexto") %></div>
                            </div>

                            <div>
                                <span class='badge <%# Eval("BadgeClass") %>'>
                                    <%# Eval("Estado") %>
                                </span>
                            </div>
                        </div>

                        <div class="hist-sub">
                            <div><strong>Colaborador:</strong> <%# Eval("Colaborador") %></div>
                        </div>

                        <asp:Panel runat="server" Visible='<%# (bool)Eval("PuedeCancelar") %>' style="margin-top:10px;">
                            <asp:Button runat="server"
                                Text="Cancelar cita"
                                CssClass="btn-cancelar"
                                CommandName="cancelar"
                                CommandArgument='<%# Eval("CitaID") %>' />
                        </asp:Panel>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Label ID="lblVacio" runat="server" Text=""
                style="display:block; margin-top:10px; font-weight:700; opacity:0.9;"></asp:Label>

        </div>
    </div>

</div>
</div>

</form>
</body>
</html>
