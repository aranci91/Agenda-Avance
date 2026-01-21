<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegistroCliente.aspx.cs" Inherits="Agenda.RegistroCliente" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Registro de Cliente - Yessi Aranci</title>
<link rel="stylesheet" runat="server" href="~/styles.css" />
<link rel="shortcut icon" runat="server" href="~/imagenes/logo.ico" />

<script type="text/javascript">

    function formatearRut(input) {
        let rut = input.value.replace(/[^0-9kK]/g, '').toUpperCase();
        if (rut.length > 1) {
            input.value = rut.slice(0, -1) + '-' + rut.slice(-1);
        } else {
            input.value = rut;
        }
    }

    // ===== CAMBIO NUEVO =====
    function capitalizarTexto(input) {
        let texto = input.value.toLowerCase();
        texto = texto.replace(/\b\w/g, function (letra) {
            return letra.toUpperCase();
        });
        input.value = texto;
    }

    function mostrarInfoClave() {
        document.getElementById("infoClave").style.display = "block";
    }

    function ocultarInfoClave() {
        document.getElementById("infoClave").style.display = "none";
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
        var claveInput = document.getElementById("<%= txtClave.ClientID %>");
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
        var clave = document.getElementById("<%= txtClave.ClientID %>").value;
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

<div class="barra-superior">
    <asp:Button ID="btnVolver" runat="server"
        Text="← Volver al inicio"
        CssClass="btn btn-light btn-volver"
        OnClick="btnVolver_Click" />
</div>

<div class="contenedor-form">
    <div id="MenuCentral">

        <img src="imagenes/logo.png" class="logo" alt="Yessi Aranci" />

        <h3>Registro de Cliente</h3>
        <p>Complete los datos para crear su cuenta</p>

        <asp:TextBox ID="txtNombres" runat="server"
            CssClass="input" Placeholder="Nombres"
            onblur="capitalizarTexto(this)">
        </asp:TextBox>

        <asp:TextBox ID="txtApellidos" runat="server"
            CssClass="input" Placeholder="Apellidos"
            onblur="capitalizarTexto(this)">
        </asp:TextBox>

        <asp:TextBox ID="txtRut" runat="server"
            CssClass="input"
            Placeholder="RUT (ej: 12345678-9)"
            onkeyup="formatearRut(this)"
            MaxLength="10"
            inputmode="numeric">
        </asp:TextBox>

        <asp:TextBox ID="txtFechaNacimiento" runat="server"
            CssClass="input"
            Placeholder="Fecha de nacimiento"
            onfocus="this.type='date'"
            onblur="if(this.value==''){this.type='text'}"
            type="text">
        </asp:TextBox>

        <asp:TextBox ID="txtCorreo" runat="server"
            CssClass="input" TextMode="Email"
            Placeholder="Correo electrónico"></asp:TextBox>

        <asp:TextBox ID="txtTelefono" runat="server"
            CssClass="input" Placeholder="Teléfono"></asp:TextBox>

        <!-- CONTRASEÑA -->
        <div class="campo-form">
            <asp:TextBox ID="txtClave" runat="server"
                CssClass="input"
                TextMode="Password"
                Placeholder="Contraseña"
                onfocus="mostrarInfoClave()"
                onblur="ocultarInfoClave()"
                onkeyup="validarClave()">
            </asp:TextBox>

            <div class="accion-clave">
                <span onclick="toggleClave('<%= txtClave.ClientID %>', this)">Mostrar</span>
            </div>
        </div>

        <div id="infoClave" class="info-clave" style="display:none;">
            <ul>
                <li id="req-mayus">Debe contener al menos 1 letra mayúscula</li>
                <li id="req-minus">Debe contener al menos 1 letra minúscula</li>
                <li id="req-num">Debe contener al menos 1 número</li>
                <li id="req-especial">Debe contener al menos 1 carácter especial (.,-#$%)</li>
                <li id="req-largo">Debe tener entre 6 y 8 caracteres</li>
            </ul>
        </div>

        <div class="campo-form">
            <asp:TextBox ID="txtConfirmarClave" runat="server"
                CssClass="input"
                TextMode="Password"
                Placeholder="Confirmar contraseña"
                onkeyup="validarCoincidenciaClave()">
            </asp:TextBox>

            <div class="accion-clave">
                <span onclick="toggleClave('<%= txtConfirmarClave.ClientID %>', this)">Mostrar</span>
            </div>

            <div id="mensajeCoincidencia" class="mensaje-validacion" style="display:none;"></div>
        </div>

        <asp:Label ID="lblMensaje" runat="server"
            style="margin-bottom:10px; display:block; color:#b30000;"></asp:Label>

        <asp:Button ID="btnRegistrar" runat="server"
            Text="Crear cuenta"
            CssClass="btn"
            OnClick="btnRegistrar_Click" />
        <asp:Label ID="lblExito" runat="server" CssClass="mensaje-exito" style="display:none;"></asp:Label>


    </div>
</div>

</form>
</body>
</html>
