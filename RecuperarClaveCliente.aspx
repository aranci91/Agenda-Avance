<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecuperarClaveCliente.aspx.cs" Inherits="Agenda.RecuperarClaveCliente" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Recuperar contraseña - Yessi Aranci</title>
    <link rel="stylesheet" href="styles.css" />
    <link rel="shortcut icon" href="imagenes/logo.ico" />
    <script type="text/javascript">
    function formatearRut(input) {
        let rut = input.value.replace(/[^0-9kK]/g, '').toUpperCase();
        if (rut.length > 1) input.value = rut.slice(0, -1) + '-' + rut.slice(-1);
        else input.value = rut;
    }

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

    function validar(id, cumple) {
        var item = document.getElementById(id);
        if (!item) return;
        if (cumple) item.classList.add("ok");
        else item.classList.remove("ok");
    }

    function validarClave() {
        var claveInput = document.getElementById("<%= txtNuevaClave.ClientID %>");
        if (!claveInput) return;

        var clave = claveInput.value;

        validar("req-mayus", /[A-Z]/.test(clave));
        validar("req-minus", /[a-z]/.test(clave));
        validar("req-num", /[0-9]/.test(clave));
        validar("req-especial", /[.\-#$%]/.test(clave));
        validar("req-largo", clave.length >= 6 && clave.length <= 8);

        validarCoincidenciaClave();
    }

    function validarCoincidenciaClave() {
        var clave = document.getElementById("<%= txtNuevaClave.ClientID %>").value;
        var confirmar = document.getElementById("<%= txtConfirmarClave.ClientID %>").value;

        var mensaje = document.getElementById("mensajeCoincidencia");
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
</head>
<body>
    <form id="form1" runat="server">
        <!-- Botón volver -->
    <div class="barra-superior">
        <asp:Button ID="btnVolver" runat="server"
            Text="← Volver al ingreso"
            CssClass="btn btn-light btn-volver"
            OnClick="btnVolver_Click" />
    </div>

    <div class="contenedor-form">
        <div id="MenuCentral">

            <img src="imagenes/logo.png" class="logo" alt="Yessi Aranci" />

            <h3>Recuperar contraseña</h3>
            <p>Ingrese su RUT para recibir un código de verificación</p>

            <asp:TextBox ID="txtRut" runat="server"
                CssClass="input"
                Placeholder="RUT (ej: 12345678-9)"
                onkeyup="formatearRut(this)"
                MaxLength="10"
                inputmode="numeric">
            </asp:TextBox>
            <!-- Método de envío -->
                <div style="width:100%; max-width:420px; text-align:left; margin: 6px 0 14px;">
                <asp:RadioButtonList ID="rblMetodoEnvio" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Text="Enviar por SMS" Value="SMS" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Enviar por correo" Value="CORREO"></asp:ListItem>
                </asp:RadioButtonList>
                </div>
            <asp:Button ID="btnEnviarCodigo" runat="server"
                Text="Enviar código"
                CssClass="btn"
                OnClick="btnEnviarCodigo_Click" />


            <asp:Panel ID="pnlCodigo" runat="server" Visible="false" style="width:100%; max-width:420px; margin-top:10px;">
                <p style="font-size:14px; color:#111; margin:10px 0 10px;">
                    Se envió un código (simulado). Ingréselo para continuar.
                </p>

                <asp:TextBox ID="txtCodigo" runat="server"
                    CssClass="input"
                    Placeholder="Código de verificación">
                </asp:TextBox>

                <asp:Button ID="btnValidarCodigo" runat="server"
                    Text="Validar código"
                    CssClass="btn"
                    OnClick="btnValidarCodigo_Click" />

                <asp:Label ID="lblMensaje" runat="server" Text=""
                    style="display:block; margin-top:10px; color:#b30000;"></asp:Label>
            </asp:Panel>

            <asp:Panel ID="pnlNuevaClave" runat="server" Visible="false" style="width:100%; max-width:420px; margin-top:10px;">

                <asp:TextBox ID="txtNuevaClave" runat="server"
                    CssClass="input"
                    TextMode="Password"
                    Placeholder="Nueva contraseña"
                    onkeyup="validarClave()">
                </asp:TextBox>

                <div class="accion-clave">
                    <span onclick="toggleClave('<%= txtNuevaClave.ClientID %>', this)">Mostrar</span>
                </div>

                <div class="info-clave">
                    <ul>
                        <li id="req-mayus">Debe contener al menos 1 letra mayúscula</li>
                        <li id="req-minus">Debe contener al menos 1 letra minúscula</li>
                        <li id="req-num">Debe contener al menos 1 número</li>
                        <li id="req-especial">Debe contener al menos 1 carácter especial (.,-#$%)</li>
                        <li id="req-largo">Debe tener entre 6 y 8 caracteres</li>
                    </ul>
                </div>

                <asp:TextBox ID="txtConfirmarClave" runat="server"
                    CssClass="input"
                    TextMode="Password"
                    Placeholder="Confirmar nueva contraseña"
                    onkeyup="validarCoincidenciaClave()">
                </asp:TextBox>

                <div class="accion-clave">
                    <span onclick="toggleClave('<%= txtConfirmarClave.ClientID %>', this)">Mostrar</span>
                </div>

                <div id="mensajeCoincidencia" class="mensaje-validacion" style="display:none;"></div>

                <asp:Button ID="btnGuardar" runat="server"
                    Text="Guardar nueva contraseña"
                    CssClass="btn"
                    OnClick="btnGuardar_Click" />
            </asp:Panel>

        </div>
      </div>
    </form>
</body>
</html>
