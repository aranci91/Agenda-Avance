<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InicioAdministrador.aspx.cs" Inherits="Agenda.Administrador.InicioAdministrador" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>Inicio Administrador - Yessi Aranci</title>

<link rel="stylesheet" href="~/styles.css" />
<link rel="shortcut icon" href="~/imagenes/logo.ico" />

<style>
    /* CARRUSEL RESEÑAS (ADMIN)*/
    .car-wrap{
        position:relative;
        overflow:hidden;
        border-radius:18px;
        background:#fff;
    }
    .car-item{
        display:none;
        padding:14px 16px;
        border-radius:18px;
        background:#fff;
        box-shadow:0 6px 18px rgba(0,0,0,0.06);
    }
    .car-item.activo{ display:block; }

    .resena-top{
        display:flex;
        align-items:center;
        justify-content:space-between;
        gap:10px;
        flex-wrap:wrap;
    }
    .resena-nombre{ font-weight:900; font-size:15px; }
    .resena-rating{ font-weight:900; opacity:.9; }
    .resena-comentario{
        margin-top:8px;
        opacity:.95;
        line-height:1.35;
    }
    .car-dots{
        display:flex;
        gap:6px;
        margin-top:10px;
        justify-content:center;
        align-items:center;
    }
    .car-dot{
        width:8px; height:8px;
        border-radius:99px;
        background:#ffd0e3;
        cursor:pointer;
        opacity:.7;
    }
    .car-dot.activo{ background:#ff9fc9; opacity:1; }

    /*  EDITOR TIPO WORD  */
    .editor-wrap{
        margin-top:12px;
    }
    .toolbar{
        display:flex;
        gap:8px;
        flex-wrap:wrap;
        align-items:center;
        margin-bottom:10px;
    }
    .toolbtn{
        border:none;
        border-radius:999px;
        padding:8px 12px;
        font-weight:900;
        cursor:pointer;
        background:#ffe0ee;
        box-shadow:0 4px 12px rgba(0,0,0,0.06);
    }
    .toolsel, .toolcolor{
        border:none;
        border-radius:999px;
        padding:8px 12px;
        font-weight:900;
        background:#fff;
        box-shadow:0 4px 12px rgba(0,0,0,0.06);
        outline:none;
    }
    .editor{
        min-height:160px;
        background:#fff7fb;
        border:1px solid #ffd0e3;
        border-radius:16px;
        padding:12px 14px;
        outline:none;
        line-height:1.45;
    }
    .editor:focus{
        border-color:#ff9fc9;
        box-shadow:0 0 0 3px rgba(255,159,201,0.22);
    }
    .help-txt{
        font-size:12px;
        opacity:.85;
        margin-top:8px;
    }

    .btn-enviar{
        margin-top:12px;
        width:100%;
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
                <img src="~/imagenes/logo.png" class="brand-logo" alt="Yessi Aranci" runat="server" />
                <div class="brand-name">YESSI ARANCI</div>
            </div>
        </div>

        <!-- BODY -->
        <div class="app-body">

            <!-- SIDEBAR -->
            <div class="sidebar">
                <div class="menu-title">MENÚ</div>

                <asp:Button ID="btnClientes" runat="server"
                    Text="GESTIÓN DE CLIENTES"
                    CssClass="menu-btn" />

                <asp:Button ID="btnGestionServicios" runat="server"
                    Text="GESTIÓN DE SERVICIOS"
                    CssClass="menu-btn"
                    OnClick="btnGestionServicios_Click"/>

                <asp:Button Text="PANEL DE SERVICIOS"
                    CssClass="menu-btn"
                    runat="server"
                    OnClick="btnPanelServicios_Click" />

                <asp:Button ID="btnGestionHorario" runat="server"
                    Text="GESTIÓN DE HORARIOS"
                    CssClass="menu-btn"
                    OnClick="btnGestionHorario_Click"/>

                <asp:Button ID="btnGestionColaboradores" runat="server"
                    Text="GESTIÓN DE COLABORADORES"
                    CssClass="menu-btn"
                    OnClick="btnGestionColaboradores_Click"/>

                <asp:Button ID="btnReportes" runat="server"
                    Text="REPORTES"
                    CssClass="menu-btn" />

                <asp:Button ID="btnMisDatos" runat="server"
                    Text="MIS DATOS"
                    CssClass="menu-btn" />

                <div class="sidebar-spacer"></div>

                <asp:Button ID="btnCerrarSesion" runat="server"
                    Text="Cerrar sesión"
                    CssClass="menu-logout"
                    OnClick="btnCerrarSesion_Click" />
            </div>

            <!-- CONTENT -->
            <div class="content">

                <div class="hola">¡Bienvenida ADMIN!</div>

                <p class="sub">
                    Desde aquí puedes gestionar servicios, horarios, colaboradores y revisar el sistema.
                </p>

                <!-- VALORACIONES (CARRUSEL) -->
                <div class="card">
                    <div class="card-title">Valoraciones recientes</div>

                    <asp:Label ID="lblSinResenas" runat="server"
                        CssClass="mensaje-app"></asp:Label>

                    <div class="car-wrap" id="carWrap" runat="server" visible="false">
                        <asp:Repeater ID="rptResenas" runat="server">
                            <ItemTemplate>
                                <div class="car-item">
                                    <div class="resena-top">
                                        <div class="resena-nombre"><%# Eval("NombreCliente") %></div>
                                        <div class="resena-rating">★ <%# Eval("Calificacion") %>/5</div>
                                    </div>
                                    <div class="resena-comentario"><%# Eval("Comentario") %></div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <div class="car-dots" id="carDots"></div>
                    </div>
                </div>

                <!-- ENVÍO MASIVO -->
                <div class="card">
                    <div class="card-title">Mensaje masivo a clientes</div>
                    <p class="sub" style="margin-top:-4px;">
                        Se enviará a todos los clientes con correo registrado.
                    </p>

                    <asp:Label ID="lblEnvioMsg" runat="server" CssClass="mensaje-app"></asp:Label>

                    <asp:TextBox ID="txtAsunto" runat="server"
                        CssClass="textbox"
                        placeholder="Asunto del correo"></asp:TextBox>

                    <div class="editor-wrap">

                        <!-- TOOLBAR WORD -->
                        <div class="toolbar">
                            <button type="button" class="toolbtn" onclick="cmd('bold')"><b>B</b></button>
                            <button type="button" class="toolbtn" onclick="cmd('italic')"><i>I</i></button>
                            <button type="button" class="toolbtn" onclick="cmd('underline')"><u>U</u></button>

                            <select class="toolsel" onchange="setFontSize(this.value)">
                                <option value="3">Tamaño</option>
                                <option value="2">Pequeño</option>
                                <option value="3">Normal</option>
                                <option value="4">Grande</option>
                                <option value="5">Extra</option>
                            </select>

                            <input class="toolcolor" type="color" title="Color" onchange="setColor(this.value)" />
                            <button type="button" class="toolbtn" onclick="cmd('insertUnorderedList')">• Lista</button>
                            <button type="button" class="toolbtn" onclick="cmd('insertOrderedList')">1. Lista</button>
                            <button type="button" class="toolbtn" onclick="cmd('justifyLeft')">Izq</button>
                            <button type="button" class="toolbtn" onclick="cmd('justifyCenter')">Centro</button>
                            <button type="button" class="toolbtn" onclick="cmd('justifyRight')">Der</button>
                        </div>

                        <!-- EDITOR -->
                        <div id="editor" class="editor" contenteditable="true">
                            <!-- Sin texto de prueba, como pediste -->
                        </div>

                        <div class="help-txt">
                            Tip: Un día a la vez
                        </div>

                        <!-- Hidden que viaja al servidor -->
                        <asp:HiddenField ID="hfHtml" runat="server" />
                    </div>

                    <asp:Button ID="btnEnviar" runat="server"
                        Text="ENVIAR"
                        CssClass="btn-confirm btn-enviar"
                        OnClientClick="return prepararEnvio();"
                        OnClick="btnEnviar_Click" />
                </div>

            </div>
        </div>
    </div>
</div>

</form>

<script>
    
    // CARRUSEL 1 RESEÑA A LA VEZ / 2 SEGUNDOS

    (function(){
        function initCarousel(){
            var wrap = document.getElementById('carWrap');
            if(!wrap) return;

            var items = wrap.querySelectorAll('.car-item');
            if(!items || items.length === 0) return;

            var dotsWrap = document.getElementById('carDots');
            if(!dotsWrap) return;

            dotsWrap.innerHTML = "";
            var idx = 0;

            function setActive(i){
                items.forEach(function(el){ el.classList.remove('activo'); });
                var dots = dotsWrap.querySelectorAll('.car-dot');
                dots.forEach(function(d){ d.classList.remove('activo'); });

                items[i].classList.add('activo');
                if(dots[i]) dots[i].classList.add('activo');
                idx = i;
            }

            // crear dots
            for(let i=0;i<items.length;i++){
                var dot = document.createElement('div');
                dot.className = 'car-dot';
                dot.addEventListener('click', function(){ setActive(i); });
                dotsWrap.appendChild(dot);
            }

            setActive(0);

            setInterval(function(){
                var next = (idx + 1) % items.length;
                setActive(next);
            }, 2000); // cada 2 segundos
        }

        document.addEventListener('DOMContentLoaded', initCarousel);
    })();

   
    // EDITOR TIPO WORD
 
    function cmd(command){
        document.execCommand(command, false, null);
        document.getElementById('editor').focus();
    }

    function setColor(color){
        document.execCommand('foreColor', false, color);
        document.getElementById('editor').focus();
    }

    function setFontSize(size){
        if(!size) return;
        document.execCommand('fontSize', false, size);
        document.getElementById('editor').focus();
    }

    function prepararEnvio(){
        var html = document.getElementById('editor').innerHTML || "";
        document.getElementById('<%= hfHtml.ClientID %>').value = html;
        return true;
    }
</script>

</body>
</html>