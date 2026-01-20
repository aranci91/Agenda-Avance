<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inicio.aspx.cs" Inherits="Agenda.Inicio" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
  <title>Yessi Aranci</title>
  <link rel="stylesheet" href="styles.css" />
  <link rel="shortcut icon" href="imagenes/logo.ico" />
  <script type="text/javascript">
  </script>
</head>
<body>
  <form id="form1" runat="server">
    <div class="contenedor">
    <div id="MenuCentral">

        <img src="imagenes/logo.png" class="logo" alt="Yessi Aranci" />

        <h3>¡Bienvenida a Yessi Aranci!</h3>
        <p>Ingresa a tu sesión y agenda tu cita</p>

        <!-- BOTÓN ÚNICO (ANTES HABÍAN 3) -->
            <asp:Button ID="btnIngresar" runat="server"
                Text="INGRESAR"
                CssClass="btn"
                OnClick="btnIngresar_Click" />

    </div>
</div>

    </form>
</body>
</html>
