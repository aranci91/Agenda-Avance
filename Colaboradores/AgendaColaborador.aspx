<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AgendaColaborador.aspx.cs" Inherits="Agenda.Colaboradores.AgendaColaborador" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Agenda Colaborador - Yessi Aranci</title>

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
        opacity:0.95;
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

    .btn-acc{
        border:none;
        border-radius:999px;
        padding:8px 14px;
        font-weight:800;
        cursor:pointer;
        margin-right:8px;
        margin-top:8px;
    }
    .btn-confirmar{ background:#bff3cc; }
    .btn-cancelar{ background:#ff9fc9; }
    .btn-atender{ background:#b7d7ff; }

    .toolbar-agenda{
        display:flex;
        gap:10px;
        flex-wrap:wrap;
        align-items:center;
        margin:12px 0 16px 0;
    }
    .btn-tab{
        border:none;
        border-radius:999px;
        padding:10px 14px;
        font-weight:900;
        cursor:pointer;
        background:#ffe0ee;
        box-shadow:0 4px 12px rgba(0,0,0,0.06);
    }
    .btn-tab.activo{
        background:#ff9fc9;
    }
    .rango-txt{
        font-weight:800;
        opacity:0.9;
        padding-left:4px;
    }
    .obs-box{
        margin-top:10px;
        background:#fff7fb;
        border:1px solid #ffe0ee;
        border-radius:14px;
        padding:10px 12px;
        font-size:13px;
        line-height:1.35;
    }
    .obs-title{
        font-weight:900;
        margin-bottom:6px;
    }
    .btn-editar-obs{
        border:none;
        border-radius:999px;
        padding:8px 14px;
        font-weight:900;
        cursor:pointer;
        background:#ffd7ea;
        margin-top:10px;
    }
</style>

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
                Text="INICIO"
                CssClass="menu-btn"
                PostBackUrl="~/Colaboradores/InicioColaborador.aspx" />

            <asp:Button runat="server"
                Text="MI AGENDA"
                CssClass="menu-btn"
                Enabled="false" />

            <div class="sidebar-spacer"></div>

            <asp:Button ID="btnCerrarSesion" runat="server"
                Text="Cerrar sesión"
                CssClass="menu-logout"
                OnClick="btnCerrarSesion_Click" />
        </div>

        <div class="content">
            <h1>Mi Agenda</h1>
            <p class="sub">Revisa citas, confirma, cancela o registra atención.</p>

            <asp:Label ID="lblMsg" runat="server" CssClass="mensaje-app"></asp:Label>

            <!-- BOTONES HOY / SEMANA / MES -->
            <div class="toolbar-agenda">
                <asp:Button ID="btnHoy" runat="server" Text="HOY" CssClass="btn-tab" OnClick="btnHoy_Click" />
                <asp:Button ID="btnSemana" runat="server" Text="SEMANA" CssClass="btn-tab" OnClick="btnSemana_Click" />
                <asp:Button ID="btnMes" runat="server" Text="MES" CssClass="btn-tab" OnClick="btnMes_Click" />

                <asp:Label ID="lblRango" runat="server" CssClass="rango-txt"></asp:Label>
            </div>

            <asp:Repeater ID="rptCitas" runat="server" OnItemCommand="rptCitas_ItemCommand">
                <ItemTemplate>
                    <div class="hist-item">

                        <div class="hist-top">
                            <div>
                                <div class="hist-servicio"><%# Eval("Servicio") %></div>
                                <div class="hist-fecha"><%# Eval("FechaHoraTexto") %></div>
                            </div>

                            <div>
                                <span class='badge <%# Eval("BadgeClass") %>'><%# Eval("Estado") %></span>
                            </div>
                        </div>

                        <div class="hist-sub">
                            <div><strong>Cliente:</strong> <%# Eval("Cliente") %></div>
                        </div>

                        <!-- OBSERVACION (solo si hay atención / o si está atendida) -->
                   <asp:Panel runat="server" Visible='<%# Convert.ToInt32(Eval("TieneAtencion")) == 1 %>'>
                        <div class="obs-box">
                            <div class="obs-title">Observación</div>
                            <div><%# Eval("Observacion") %></div>

                            <asp:Button runat="server"
                                Text="✏️ Editar observación"
                                CssClass="btn-editar-obs"
                                CommandName="editarObs"
                                CommandArgument='<%# Eval("CitaID") %>'
                                Visible='<%# Convert.ToBoolean(Eval("PuedeEditarObs")) %>' />
                        </div>
                    </asp:Panel>

                        <!-- ACCIONES -->
                        <asp:Panel runat="server" Visible='<%# (bool)Eval("PuedeConfirmar") %>'>
                            <asp:Button runat="server"
                                Text="Confirmar"
                                CssClass="btn-acc btn-confirmar"
                                CommandName="confirmar"
                                CommandArgument='<%# Eval("CitaID") %>' />
                        </asp:Panel>

                        <asp:Panel runat="server" Visible='<%# (bool)Eval("PuedeCancelar") %>'>
                            <asp:Button runat="server"
                                Text="Cancelar"
                                CssClass="btn-acc btn-cancelar"
                                CommandName="cancelar"
                                CommandArgument='<%# Eval("CitaID") %>' />
                        </asp:Panel>

                        <asp:Panel runat="server" Visible='<%# (bool)Eval("PuedeAtender") %>'>
                            <asp:Button runat="server"
                                Text="Atender"
                                CssClass="btn-acc btn-atender"
                                CommandName="atender"
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