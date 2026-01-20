<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Ingreso.aspx.cs" Inherits="Agenda.Ingreso" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>Yessi Aranci</title>
<link rel="stylesheet" href="styles.css" />
<link rel="shortcut icon" href="imagenes/logo.ico" />
<script type="text/javascript">
    function formatearRut(input) {
        let rut = input.value.replace(/[^0-9kK]/g, '').toUpperCase();

        if (rut.length > 1) {
            input.value = rut.slice(0, -1) + '-' + rut.slice(-1);
        } else {
            input.value = rut;
        }
    }
</script>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Botón volver -->
        <div class="barra-superior">
    <asp:Button ID="btnVolver" runat="server"
        Text="← Volver al inicio"
        CssClass="btn btn-light btn-volver"
        OnClick="btnVolver_Click" />
</div>

            <!-- Contenedor central -->
        <div class="contenedor">
            <div id="MenuCentral">

                <img src="imagenes/logo.png" class="logo" alt="Yessi Aranci" />

                <h3>Ingreso de Sesión</h3>
                <p>Ingrese su RUT y contraseña para continuar</p>

                <!-- RUT -->
                <asp:TextBox ID="txtRut" runat="server"
                    CssClass="input"
                    Placeholder="RUT (ej: 12345678-9)"
                    onkeyup="formatearRut(this)"
                    MaxLength="10"
                    inputmode="numeric">
                </asp:TextBox>

                <!-- Contraseña -->
                <asp:TextBox ID="txtClave" runat="server"
                    CssClass="input"
                    TextMode="Password"
                    Placeholder="Ingrese su contraseña"></asp:TextBox>

                <asp:Label ID="lblErrorClave" runat="server"
                    Text=""
                    CssClass="mensaje-validacion error"
                    style="display:block; margin-top:-6px;">
                </asp:Label>


                <!-- Botón ingresar -->
                <asp:Button ID="btnIngresar" runat="server"
                    Text="Ingresar"
                    CssClass="btn"
                    OnClick="btnIngresar_Click" />

                <div style="width:100%; max-width:420px; text-align:right; margin-top:-6px;">

                <asp:LinkButton ID="lnkRecuperarClave" runat="server"
                    OnClick="lnkRecuperarClave_Click"
                    style="color:#e46aa3; font-size:13px; text-decoration:none;">
                    ¿Olvidaste tu contraseña?
                </asp:LinkButton>

                </div>


                <!-- Panel de verificación (oculto al inicio) -->
<asp:Panel ID="pnlVerificacion" runat="server" Visible="false" style="margin-top:20px; width:100%; max-width:420px;">

    <p style="font-size:14px; color:#111; margin-bottom:10px;">
        Se ha enviado un código de verificación a su teléfono.
    </p>

    <asp:TextBox ID="txtCodigo" runat="server"
        CssClass="input"
        Placeholder="Ingrese el código"></asp:TextBox>

    <asp:Button ID="btnVerificar" runat="server"
        Text="Verificar código"
        CssClass="btn"
        OnClick="btnVerificar_Click" />

    <asp:Button ID="btnEnviarCorreo" runat="server"
        Text="Enviar por correo"
        CssClass="btn btn-light"
        OnClick="btnEnviarCorreo_Click" />

    <asp:Label ID="lblMensaje" runat="server" Text="" style="display:block; margin-top:10px; color:#b30000;"></asp:Label>

</asp:Panel>


                <!-- Crear cuenta -->
                <p style="margin-top:20px; font-size:14px;">
                    ¿No tienes cuenta?
                    <asp:LinkButton ID="lnkCrearCuenta" runat="server"
                        OnClick="lnkCrearCuenta_Click">
                        Crear cuenta
                    </asp:LinkButton>
                </p>

            </div>
        </div>
    </form>
</body>
</html>
