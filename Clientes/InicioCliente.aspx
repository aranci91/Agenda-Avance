<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InicioCliente.aspx.cs" Inherits="Agenda.InicioCliente" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
  <title>Inicio Cliente - Yessi Aranci</title>
  <link rel="stylesheet" href="<%= ResolveUrl("~/styles.css") %>" />
  <link rel="shortcut icon" href="<%= ResolveUrl("~/imagenes/logo.ico") %>" />

  <style>
      /* Carrusel reseñas */
      .reviews-wrap{
          margin-top:10px;
      }
      .carousel{
          position:relative;
          overflow:hidden;
          border-radius:18px;
          background:#fff;
          box-shadow:0 6px 18px rgba(0,0,0,0.06);
          border:1px solid #ffe0ee;
      }
      .slide{
          display:none;
          padding:14px 16px;
      }
      .slide.active{ display:block; }
      .rev-name{
          font-weight:900;
          font-size:15px;
          margin-bottom:4px;
      }
      .rev-stars{
          font-weight:900;
          opacity:0.95;
          margin-bottom:6px;
      }
      .rev-comment{
          opacity:0.95;
          line-height:1.35;
      }
      .carousel-dots{
          display:flex;
          gap:6px;
          justify-content:center;
          padding:10px 0 12px 0;
          background:#fff7fb;
          border-top:1px solid #ffe0ee;
      }
      .dot{
          width:8px;
          height:8px;
          border-radius:999px;
          background:#ffd0e6;
          cursor:pointer;
      }
      .dot.active{ background:#ff9fc9; }

      .val-panels{
          display:grid;
          grid-template-columns: 1fr;
          gap:14px;
          margin-top:10px;
      }
      .panel-mini{
          background:#fff;
          border-radius:18px;
          padding:14px 16px;
          border:1px solid #ffe0ee;
          box-shadow:0 6px 18px rgba(0,0,0,0.06);
      }
      .panel-title{
          font-weight:900;
          margin-bottom:10px;
      }
      .hint{
          font-size:13px;
          opacity:0.85;
          margin-top:-6px;
          margin-bottom:10px;
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

            <!-- BODY -->
            <div class="app-body">

                <!-- SIDEBAR -->
                <div class="sidebar">
                    <div class="menu-title">MENÚ</div>

                    <asp:Button ID="btnMisDatos" runat="server"
                        Text="EDITAR DATOS"
                        CssClass="menu-btn"
                        OnClick="btnMisDatos_Click" />
                    <div class="menu-subtxt">Actualiza tus datos personales</div>

                    <asp:Button ID="btnHistorial" runat="server"
                        Text="HISTORIAL DE CITAS"
                        CssClass="menu-btn"
                        OnClick="btnHistorial_Click" />

                    <div class="sidebar-spacer"></div>

                    <asp:Button ID="btnCerrarSesion" runat="server"
                        Text="Cerrar sesión"
                        CssClass="menu-logout"
                        OnClick="btnCerrarSesion_Click" />
                </div>

                <!-- CONTENT -->
                <div class="content">

                    <div class="hola">
                        ¡Bienvenid@ <asp:Label ID="lblNombreCliente" runat="server" Text="CLIENTE"></asp:Label>!
                    </div>

                    <p class="sub">
                        Aquí podrás agendar tus citas, revisar tu historial y actualizar tus datos.
                    </p>

                    <!-- VALORACIONES -->
                    <div class="card">
                        <div class="card-title">Valoraciones</div>

                        <div class="val-panels">

                            <!-- PANEL 1: CARRUSEL -->
                            <div class="panel-mini">
                                <div class="panel-title">Últimas reseñas</div>

                                <asp:Label ID="lblSinResenas" runat="server" Text=""
                                    style="display:block; margin-top:6px; color:#444; opacity:0.9;"></asp:Label>

                                <div class="reviews-wrap" id="reviewsWrap" runat="server">
                                    <div class="carousel" id="carouselResenas">
                                        <asp:Repeater ID="rptResenas" runat="server">
                                            <ItemTemplate>
                                                <div class="slide">
                                                    <div class="rev-name"><%# Eval("NombreCliente") %></div>
                                                    <div class="rev-stars">★ <%# Eval("Calificacion") %>/5</div>
                                                    <div class="rev-comment"><%# Eval("Comentario") %></div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                        <div class="carousel-dots" id="dotsResenas"></div>
                                    </div>
                                </div>
                            </div>

                            <!-- PANEL 2: AGREGAR RESEÑA -->
                            <div class="panel-mini">
                                <div class="panel-title">Agregar comentario</div>
                                <div class="hint">Tu reseña se asociará automáticamente a tu última atención realizada.</div>

                                <!-- Calificación -->
                                <asp:DropDownList ID="ddlCalificacion" runat="server" CssClass="select">
                                    <asp:ListItem Text="5" Value="5" />
                                    <asp:ListItem Text="4" Value="4" />
                                    <asp:ListItem Text="3" Value="3" />
                                    <asp:ListItem Text="2" Value="2" />
                                    <asp:ListItem Text="1" Value="1" />
                                </asp:DropDownList>

                                <!-- Comentario -->
                                <asp:TextBox ID="txtComentario" runat="server"
                                    CssClass="textbox"
                                    TextMode="MultiLine"
                                    Rows="3"
                                    placeholder="Escribe tu comentario..." />

                                <!-- BOTÓN PUBLICAR -->
                                <asp:Button ID="btnPublicarValoracion" runat="server"
                                    Text="PUBLICAR VALORACIÓN"
                                    CssClass="btn-confirm"
                                    OnClick="btnPublicarValoracion_Click" />

                                <asp:Label ID="lblMensajeResena" runat="server" Text=""
                                    CssClass="mensaje-app"></asp:Label>
                            </div>

                        </div>
                    </div>

                    <!-- AGENDAR -->
                    <div class="card">
                        <div class="card-title">AGENDAR NUEVA CITA</div>

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

                            <asp:TextBox ID="txtFecha" runat="server"
                                CssClass="textbox"
                                TextMode="Date"
                                AutoPostBack="true"
                                OnTextChanged="txtFecha_TextChanged" />

                            <asp:DropDownList ID="ddlHora" runat="server" CssClass="select"></asp:DropDownList>

                            <asp:Label ID="lblSinHoras" runat="server" Text=""
                                style="display:block; margin-top:8px; color:#b00020; font-weight:600;"></asp:Label>

                            <asp:Button ID="btnConfirmar" runat="server"
                                Text="CONFIRMAR"
                                CssClass="btn-confirm"
                                OnClick="btnConfirmar_Click" />
                        </div>

                        <asp:Label ID="lblMensajeCita" runat="server" Text="" CssClass="mensaje-app"></asp:Label>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" Text="" CssClass="mensaje-app"></asp:Label>

                </div>
            </div>
        </div>
    </div>

</form>

<script>
    // Carrusel automático 1 reseña por vez
    (function () {
        function initCarousel() {
            const carousel = document.getElementById('carouselResenas');
            if (!carousel) return;

            const slides = carousel.querySelectorAll('.slide');
            const dotsWrap = document.getElementById('dotsResenas');

            if (!slides || slides.length === 0) return;

            // construir dots
            dotsWrap.innerHTML = '';
            slides.forEach((_, i) => {
                const d = document.createElement('div');
                d.className = 'dot' + (i === 0 ? ' active' : '');
                d.addEventListener('click', () => goTo(i));
                dotsWrap.appendChild(d);
            });

            let idx = 0;
            let timer = null;

            function render() {
                slides.forEach((s, i) => s.classList.toggle('active', i === idx));
                const dots = dotsWrap.querySelectorAll('.dot');
                dots.forEach((d, i) => d.classList.toggle('active', i === idx));
            }

            function goTo(i) {
                idx = i;
                render();
                restart();
            }

            function next() {
                idx = (idx + 1) % slides.length;
                render();
            }

            function restart() {
                if (timer) clearInterval(timer);
                timer = setInterval(next, 3500); // velocidad del carrusel
            }

            // init
            idx = 0;
            render();
            restart();
        }

        document.addEventListener('DOMContentLoaded', initCarousel);
    })();
</script>

</body>
</html>
