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
</script>
</head>

<body>
<form id="form1" runat="server">

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
<p>Ingrese su RUT para recibir un código en su correo</p>

<asp:TextBox ID="txtRut" runat="server"
    CssClass="input"
    Placeholder="RUT (ej: 12345678-9)"
    onkeyup="formatearRut(this)"
    MaxLength="10" />

<asp:Button ID="btnEnviarCodigo" runat="server"
    Text="Enviar código al correo"
    CssClass="btn"
    OnClick="btnEnviarCodigo_Click" />

<asp:Panel ID="pnlCodigo" runat="server" Visible="false">

<p>Ingrese el código recibido por correo</p>

<asp:TextBox ID="txtCodigo" runat="server"
    CssClass="input"
    Placeholder="Código de verificación" />

<asp:Button ID="btnValidarCodigo" runat="server"
    Text="Validar código"
    CssClass="btn"
    OnClick="btnValidarCodigo_Click" />

<asp:Label ID="lblMensaje" runat="server"
    style="display:block; margin-top:10px; color:#b30000;"></asp:Label>

</asp:Panel>

<asp:Panel ID="pnlNuevaClave" runat="server" Visible="false">

<asp:TextBox ID="txtNuevaClave" runat="server"
    CssClass="input"
    TextMode="Password"
    Placeholder="Nueva contraseña" />

<asp:TextBox ID="txtConfirmarClave" runat="server"
    CssClass="input"
    TextMode="Password"
    Placeholder="Confirmar contraseña" />

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
