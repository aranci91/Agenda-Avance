<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Ingreso.aspx.cs" Inherits="Agenda.Ingreso" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
  <title>Yessi Aranci</title>

<link rel="stylesheet" href="<%= ResolveUrl("~/styles.css") %>" />
<link rel="shortcut icon" href="<%= ResolveUrl("~/imagenes/logo.ico") %>" />

<script type="text/javascript">

function formatearRut(input) {
    let rut = input.value.replace(/[^0-9kK]/g, '').toUpperCase();
    if (rut.length > 1)
        input.value = rut.slice(0, -1) + '-' + rut.slice(-1);
    else
        input.value = rut;
}

function limpiarRut(rut) {
    return (rut || "").replace(/\./g, "").replace(/\s/g, "").toUpperCase();
}

function rutValido(rutConDv) {
    rutConDv = limpiarRut(rutConDv);
    if (!rutConDv || rutConDv.indexOf("-") === -1) return false;

    var partes = rutConDv.split("-");
    var cuerpo = partes[0];
    var dv = partes[1].toUpperCase();

    var suma = 0;
    var multiplo = 2;

    for (var i = cuerpo.length - 1; i >= 0; i--) {
        suma += parseInt(cuerpo.charAt(i)) * multiplo;
        multiplo = multiplo < 7 ? multiplo + 1 : 2;
    }

    var dvEsperado = 11 - (suma % 11);
    var dvCalc = dvEsperado == 11 ? "0" : dvEsperado == 10 ? "K" : dvEsperado.toString();

    return dvCalc === dv;
}

function validarRutUI() {
    var input = document.getElementById('<%= txtRut.ClientID %>');
    var lbl = document.getElementById('<%= lblErrorRut.ClientID %>');

        if (!input || !lbl) return true;

        var rut = input.value.trim();

        if (rut.length === 0) {
            lbl.innerHTML = "";
            return true;
        }

        if (!rutValido(rut)) {
            lbl.innerHTML = "RUT inválido.";
            lbl.className = "mensaje-validacion error";
            return false;
        }

        lbl.innerHTML = "RUT válido ✅";
        lbl.className = "mensaje-validacion ok";
        return true;
    }

</script>

<style>
.mensaje-validacion.ok {
    color: #1b7f3a;
    font-weight: 700;
}
.mensaje-validacion.error {
    color: #b30000;
    font-weight: 700;
}
</style>

</head>

<body>

<form id="form1" runat="server" DefaultButton="btnIngresar">

<!-- Barra superior -->
<div class="barra-superior">
    <asp:Button ID="btnVolver" runat="server"
        Text="← Volver al inicio"
        CssClass="btn-volver"
        OnClick="btnVolver_Click" />
</div>

<div class="contenedor">

<div id="MenuCentral">

<img src="imagenes/logo.png" class="logo" alt="Yessi Aranci" />

<h3>Ingreso de Sesión</h3>
<p>Ingrese su RUT y contraseña para continuar</p>

<asp:TextBox ID="txtRut" runat="server"
    CssClass="input"
    Placeholder="RUT (ej: 12345678-9)"
    onkeyup="formatearRut(this); validarRutUI();">
</asp:TextBox>

<asp:Label ID="lblErrorRut" runat="server" CssClass="mensaje-validacion"></asp:Label>

<asp:TextBox ID="txtClave" runat="server"
    CssClass="input"
    TextMode="Password"
    Placeholder="Ingrese su contraseña">
</asp:TextBox>

<asp:Label ID="lblErrorClave" runat="server" CssClass="mensaje-validacion error"></asp:Label>

<asp:Button ID="btnIngresar" runat="server"
    Text="Ingresar"
    CssClass="btn"
    OnClick="btnIngresar_Click" />

<div style="width:100%; text-align:right;">
    <asp:LinkButton ID="lnkRecuperarClave" runat="server"
        OnClick="lnkRecuperarClave_Click"
        style="color:#e46aa3; font-size:13px;">
        ¿Olvidaste tu contraseña?
    </asp:LinkButton>
</div>

<!-- PANEL VERIFICACION -->
<asp:Panel ID="pnlVerificacion" runat="server" Visible="false" style="margin-top:20px; width:100%;">

    <p>Se ha enviado un código de verificación a su correo electrónico</p>

    <asp:TextBox ID="txtCodigo" runat="server"
        CssClass="input"
        Placeholder="Ingrese el código">
    </asp:TextBox>

    <div class="grupo-botones">

        <asp:Button ID="btnVerificar" runat="server"
            Text="Verificar código"
            CssClass="btn"
            OnClick="btnVerificar_Click" />

        <asp:Button ID="btnReenviarCodigo" runat="server"
            Text="Reenviar código"
            CssClass="btn btn-light"
            OnClick="btnReenviarCodigo_Click" />

    </div>

    <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje-validacion"></asp:Label>

</asp:Panel>


<p style="margin-top:20px;">
¿No tienes cuenta?
<asp:LinkButton ID="lnkCrearCuenta" runat="server" OnClick="lnkCrearCuenta_Click">
Crear cuenta
</asp:LinkButton>
</p>

</div>
</div>

</form>

</body>
</html>