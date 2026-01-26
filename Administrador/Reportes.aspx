<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reportes.aspx.cs" Inherits="Agenda.Administrador.Reportes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>Reportes - Yessi Aranci</title>

<link rel="stylesheet" href="<%= ResolveUrl("~/styles.css") %>" />
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</head>

<body>
<form id="form1" runat="server">

<div class="app-shell">
<div class="panel-app">

<div class="app-topbar">
    <div class="brand">
        <img src="<%= ResolveUrl("~/imagenes/logo.png") %>" class="brand-logo" />
        <div class="brand-name">YESSI ARANCI</div>
    </div>
</div>

<div class="app-body">

<div class="sidebar">
    <div class="menu-title">MENÚ</div>

    <asp:Button runat="server" Text="INICIO" CssClass="menu-btn" OnClick="btnInicio_Click"/>
    <div class="sidebar-spacer"></div>
    <asp:Button runat="server" Text="Cerrar sesión" CssClass="menu-logout" OnClick="btnCerrarSesion_Click"/>
</div>

<div class="content">

<h1>Reportes</h1>

<div class="card">
    <div class="card-title">Servicios más solicitados</div>
    <canvas id="chartServicios"></canvas>
</div>

<div class="card">
    <div class="card-title">Top 5 clientes con más atenciones</div>
    <canvas id="chartClientes"></canvas>
</div>

<asp:HiddenField ID="hfServicios" runat="server" />
<asp:HiddenField ID="hfClientes" runat="server" />

</div>
</div>
</div>
</div>

</form>

<script>
var servicios = JSON.parse(document.getElementById('<%= hfServicios.ClientID %>').value);
var clientes = JSON.parse(document.getElementById('<%= hfClientes.ClientID %>').value);

    new Chart(document.getElementById("chartServicios"), {
        type: 'bar',
        data: {
            labels: servicios.labels,
            datasets: [{
                label: 'Cantidad',
                data: servicios.data,
                backgroundColor: '#ff9fc9'
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 5,
                        precision: 0
                    }
                }
            }
        }
    });



    new Chart(document.getElementById("chartClientes"), {
        type: 'bar',
        data: {
            labels: clientes.labels,
            datasets: [{
                label: 'Atenciones',
                data: clientes.data,
                backgroundColor: '#ffd0e3'
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 5,
                        precision: 0
                    }
                }
            }
        }
    });


</script>

</body>
</html>