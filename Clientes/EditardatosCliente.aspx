<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditardatosCliente.aspx.cs" Inherits="Agenda.EditardatosCliente" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
     <title>Editar datos - Yessi Aranci</title>
   <link rel="stylesheet" href="<%= ResolveUrl("~/styles.css") %>" />
   <link rel="shortcut icon" href="<%= ResolveUrl("~/imagenes/logo.ico") %>" />
</head>
<body>
<form id="form1" runat="server">

    <div class="app-shell">
        <div class="panel-app">

            <!-- TOPBAR -->
            <div class="app-topbar">
                <div class="brand">
                    <!-- brand-logo ( ya está en CSS) -->
                    <img src="<%= ResolveUrl("~/imagenes/logo.png") %>" class="brand-logo" alt="Yessi Aranci" />
                    <div>
                        <div class="brand-name">YESSI ARANCI</div>
                        <!-- Subtítulo simple -->
                        <div style="font-size:12px; opacity:0.75; font-weight:700; margin-top:2px;">
                            EDITAR DATOS
                        </div>
                    </div>
                </div>
            </div>

            <!-- BODY -->
            <div class="app-body">

                <!-- SIDEBAR -->
                <div class="sidebar">

                    <div class="menu-title">MENÚ</div>

                    <asp:Button ID="btnInicio" runat="server"
                        Text="INICIO"
                        CssClass="menu-btn"
                        OnClick="btnInicio_Click" />

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

                    <div class="hola">Editar mis datos</div>
                    <p class="sub">Puedes actualizar tu correo/teléfono y, si quieres, cambiar tu contraseña.</p>

                    <!-- DATOS CONTACTO -->
                    <div class="card">
                        <div class="card-title">Datos de contacto</div>

                        <div class="form-col">

                            <asp:Label ID="lblCorreoActual" runat="server"
                                CssClass="mini-label" Text="Correo"></asp:Label>
                            <asp:TextBox ID="txtCorreo" runat="server" CssClass="textbox" />

                            <asp:Label ID="lblTelefonoActual" runat="server"
                                CssClass="mini-label" Text="Teléfono"></asp:Label>
                            <asp:TextBox ID="txtTelefono" runat="server" CssClass="textbox" />

                            <asp:Button ID="btnGuardarContacto" runat="server"
                                Text="GUARDAR CAMBIOS"
                                CssClass="btn-confirm"
                                OnClick="btnGuardarContacto_Click" />
                        </div>
                    </div>

                    <!-- CAMBIO CONTRASEÑA -->
                    <div class="card">
                        <div class="card-title">Cambiar contraseña</div>

                        <div class="form-col">

                            <asp:Label ID="lblClaveActual" runat="server"
                                CssClass="mini-label" Text="Contraseña actual"></asp:Label>

                            <div class="campo-form">
                                <asp:TextBox ID="txtClaveActual" runat="server"
                                    CssClass="textbox" TextMode="Password" />
                                <div class="accion-clave">
                                    <span onclick="toggleClave('<%= txtClaveActual.ClientID %>', this)">Mostrar</span>
                                </div>
                            </div>

                            <asp:Label ID="lblClaveNueva" runat="server"
                                CssClass="mini-label" Text="Nueva contraseña"></asp:Label>

                            <div class="campo-form">
                                <asp:TextBox ID="txtClaveNueva" runat="server"
                                    CssClass="textbox" TextMode="Password"
                                    onfocus="mostrarInfoClaveEditar()"
                                    onblur="ocultarInfoClaveEditar()"
                                    onkeyup="validarClaveEditar()" />
                                <div class="accion-clave">
                                    <span onclick="toggleClave('<%= txtClaveNueva.ClientID %>', this)">Mostrar</span>
                                </div>
                            </div>

                            <!-- Requisitos (idéntico estilo que registro) -->
                            <div id="infoClaveEditar" class="info-clave" style="display:none;">
                                <ul>
                                    <li id="reqE-mayus">Debe contener al menos 1 letra mayúscula</li>
                                    <li id="reqE-minus">Debe contener al menos 1 letra minúscula</li>
                                    <li id="reqE-num">Debe contener al menos 1 número</li>
                                    <li id="reqE-especial">Debe contener al menos 1 carácter especial (.,-#$%)</li>
                                    <li id="reqE-largo">Debe tener entre 6 y 8 caracteres</li>
                                </ul>
                            </div>

                            <asp:Label ID="lblClaveConfirmar" runat="server"
                                CssClass="mini-label" Text="Confirmar nueva contraseña"></asp:Label>

                            <div class="campo-form">
                                <asp:TextBox ID="txtClaveConfirmar" runat="server"
                                    CssClass="textbox" TextMode="Password"
                                    onkeyup="validarCoincidenciaClaveEditar()" />
                                <div class="accion-clave">
                                    <span onclick="toggleClave('<%= txtClaveConfirmar.ClientID %>', this)">Mostrar</span>
                                </div>

                                <div id="mensajeCoincidenciaEditar" class="mensaje-validacion" style="display:none;"></div>
                            </div>

                            <asp:Button ID="btnCambiarClave" runat="server"
                                Text="CAMBIAR CONTRASEÑA"
                                CssClass="btn-confirm"
                                OnClick="btnCambiarClave_Click" />
                        </div>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje-app" />

                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function toggleClave(clientId, elemento) {
            var input = document.getElementById(clientId);
            if (!input) return;

            if (input.type === "password") {
                input.type = "text";
                elemento.innerText = "Ocultar";
            } else {
                input.type = "password";
                elemento.innerText = "Mostrar";
            }
        }

        function mostrarInfoClaveEditar() {
            var box = document.getElementById("infoClaveEditar");
            if (box) box.style.display = "block";
        }

        function ocultarInfoClaveEditar() {
            var box = document.getElementById("infoClaveEditar");
            if (box) box.style.display = "none";
        }

        function validar(id, cumple) {
            var item = document.getElementById(id);
            if (!item) return;

            if (cumple) item.classList.add("ok");
            else item.classList.remove("ok");
        }

        function validarClaveEditar() {
            var claveInput = document.getElementById("<%= txtClaveNueva.ClientID %>");
            if (!claveInput) return;

            var clave = claveInput.value;

            validar("reqE-mayus", /[A-Z]/.test(clave));
            validar("reqE-minus", /[a-z]/.test(clave));
            validar("reqE-num", /[0-9]/.test(clave));
            validar("reqE-especial", /[.\-#$%]/.test(clave));
            validar("reqE-largo", clave.length >= 6 && clave.length <= 8);

            validarCoincidenciaClaveEditar();
        }

        function validarCoincidenciaClaveEditar() {
            var clave = document.getElementById("<%= txtClaveNueva.ClientID %>").value;
            var confirmar = document.getElementById("<%= txtClaveConfirmar.ClientID %>").value;

            var mensaje = document.getElementById("mensajeCoincidenciaEditar");
            if (!mensaje) return;

            if (confirmar.length === 0) {
                mensaje.style.display = "none";
                return;
            }

            mensaje.style.display = "block";

            if (clave === confirmar) {
                mensaje.innerHTML = "Las contraseñas coinciden";
                mensaje.className = "mensaje-validacion ok";
            } else {
                mensaje.innerHTML = "Las contraseñas no coinciden";
                mensaje.className = "mensaje-validacion error";
            }
        }
    </script>

</form>
</body>
</html>
