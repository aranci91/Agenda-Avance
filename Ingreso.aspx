<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Ingreso.aspx.cs" Inherits="Agenda.Ingreso" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>Yessi Aranci</title>

<link rel="stylesheet" href="styles.css" />
<link rel="shortcut icon" href="imagenes/logo.ico" />

<script type="text/javascript">

    //  formateo rut 
    function formatearRut(input) {
        let rut = input.value.replace(/[^0-9kK]/g, '').toUpperCase();
        if (rut.length > 1) {
            input.value = rut.slice(0, -1) + '-' + rut.slice(-1);
        } else {
            input.value = rut;
        }
    }

    // Validación REAL de RUT  en JS
    function limpiarRut(rut) {
        return (rut || "").replace(/\./g, "").replace(/\s/g, "").toUpperCase();
    }

    function rutValido(rutConDv) {
        rutConDv = limpiarRut(rutConDv);
        if (!rutConDv || rutConDv.indexOf("-") === -1) return false;

        var partes = rutConDv.split("-");
        var cuerpo = partes[0];
        var dv = (partes[1] || "").toUpperCase();

        if (cuerpo.length < 7 || cuerpo.length > 8) return false;
        if (!/^\d+$/.test(cuerpo)) return false;
        if (!/^[0-9K]$/.test(dv)) return false;

        var suma = 0;
        var multiplo = 2;

        for (var i = cuerpo.length - 1; i >= 0; i--) {
            suma += parseInt(cuerpo.charAt(i), 10) * multiplo;
            multiplo++;
            if (multiplo > 7) multiplo = 2;
        }

        var resto = suma % 11;
        var dvEsperado = 11 - resto;

        var dvCalc = "";
        if (dvEsperado === 11) dvCalc = "0";
        else if (dvEsperado === 10) dvCalc = "K";
        else dvCalc = String(dvEsperado);

        return dvCalc === dv;
    }

    // Mensaje visual bajo el RUT (igual estilo a clave)
    function validarRutUI() {
        var input = document.getElementById('<%= txtRut.ClientID %>');
        var lbl = document.getElementById('<%= lblErrorRut.ClientID %>');

        if (!input || !lbl) return true;

        var rut = (input.value || "").trim();

        // si está vacío no mostramos error (se valida en server también)
        if (rut.length === 0) {
            lbl.innerHTML = "";
            lbl.className = "mensaje-validacion";
            return true;
        }

        // exige guión para evaluar (porque tú lo formateas)
        if (rut.indexOf("-") === -1) {
            lbl.innerHTML = "RUT incompleto.";
            lbl.className = "mensaje-validacion error";
            return false;
        }

        if (!rutValido(rut)) {
            lbl.innerHTML = "RUT inválido. Revisa el dígito verificador.";
            lbl.className = "mensaje-validacion error";
            return false;
        }

        lbl.innerHTML = "RUT válido ✅";
        lbl.className = "mensaje-validacion ok";
        return true;
    }

    // Evitar que Enter “borre todo” y hacer que Enter dispare Ingresar
    document.addEventListener("DOMContentLoaded", function () {
        var form = document.getElementById('<%= form1.ClientID %>');
        if (!form) return;

        form.addEventListener("keydown", function (e) {
            if (e.key === "Enter") {
                var tag = (e.target && e.target.tagName) ? e.target.tagName.toLowerCase() : "";

                // si estás escribiendo en un textarea, sí permitimos enter
                if (tag === "textarea") return;

                e.preventDefault();

                // validar rut en UI antes de enviar
                if (!validarRutUI()) return;

                // dispara el botón ingresar
                var btn = document.getElementById('<%= btnIngresar.ClientID %>');
                if (btn) btn.click();
            }
        });
    });

</script>

<style>
    /*  clases para el mensaje del RUT */
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
    <!--  DefaultButton para que Enter use btnIngresar -->
    <form id="form1" runat="server" DefaultButton="btnIngresar">

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
                    onkeyup="formatearRut(this); validarRutUI();"
                    onblur="validarRutUI();"
                    MaxLength="10"
                    inputmode="numeric">
                </asp:TextBox>

                <!-- Label de validación para RUT -->
                <asp:Label ID="lblErrorRut" runat="server"
                    Text=""
                    CssClass="mensaje-validacion"
                    style="display:block; margin-top:-6px;">
                </asp:Label>

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

                    <asp:Label ID="lblMensaje" runat="server" Text=""
                        style="display:block; margin-top:10px; color:#b30000;"></asp:Label>

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