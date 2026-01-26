<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionHorario.aspx.cs" Inherits="Agenda.Administrador.GestionHorario" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>Gestión de Horarios - Yessi Aranci</title>

<!-- FullCalendar -->
<link href="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.11/index.global.min.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.11/index.global.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.11/locales/es.global.min.js"></script>

<link rel="stylesheet" href="../styles.css" />

<style>
    .fc .fc-button {
        background-color: #ff9fc9 !important;
        border: none !important;
        color: #000 !important;
        border-radius: 18px !important;
        font-weight: 700;
        padding: 6px 14px;
    }
    .fc .fc-button-active {
        background-color: #e46aa3 !important;
        color: #fff !important;
    }
    .fc-timegrid-slot-label {
        font-size: 13px;
        color: #444;
    }
    .fc-event {
        background-color: #ffc1dc !important;
        border: none !important;
        color: #000 !important;
        border-radius: 12px;
        font-size: 12px;
        padding: 2px 6px;
        cursor: pointer;
    }
    .fc-timegrid-all-day { display:none; }

    .top-tools{
        display:flex;
        gap:12px;
        flex-wrap:wrap;
        align-items:center;
        margin-bottom:12px;
    }
    .lbl-soft{
        font-weight:900;
        opacity:.9;
    }
</style>
</head>

<body>
<form id="form1" runat="server">

<div class="app-shell">
<div class="panel-app">

    <div class="app-topbar">
        <div class="brand">
            <img src="../imagenes/logo.png" class="brand-logo" />
            <div class="brand-name">YESSI ARANCI</div>
        </div>
    </div>

    <div class="app-body">

        <div class="sidebar">
    <div class="menu-title">MENÚ</div>

            <asp:Button runat="server"
                Text="INICIO"
                CssClass="menu-btn"
                OnClick="btnInicio_Click" />

            <asp:Button runat="server"
                Text="GESTIÓN DE SERVICIOS"
                CssClass="menu-btn"
                PostBackUrl="GestionServicios.aspx" />

            <asp:Button runat="server"
                Text="GESTIÓN DE HORARIOS"
                CssClass="menu-btn"
                Enabled="false" />

            <div class="sidebar-spacer"></div>

            <asp:Button runat="server"
                Text="Cerrar sesión"
                CssClass="menu-logout"
                OnClick="btnCerrarSesion_Click" />
        </div>

        <div class="content">
            <h1>Gestión de Horarios</h1>
            <p class="sub">Define horarios disponibles por colaborador.</p>

            <div class="card">

                <div class="top-tools">
                    <span class="lbl-soft">Colaborador</span>

                    <asp:DropDownList ID="ddlColaboradores"
                        runat="server"
                        CssClass="select" />

                    <asp:Label ID="lblMsg" runat="server" CssClass="mensaje-app"></asp:Label>
                </div>

                <div id="calendar"></div>
            </div>

        </div>
    </div>
</div>
</div>

</form>

<script>
    document.addEventListener('DOMContentLoaded', function () {

        const ddl = document.getElementById('<%= ddlColaboradores.ClientID %>');
        const lblMsg = document.getElementById('<%= lblMsg.ClientID %>');
        const calendarEl = document.getElementById('calendar');

        function getColaboradorId() {
            const v = (ddl && ddl.value) ? ddl.value : "";
            return v;
        }

        function setMsg(texto) {
            if (!lblMsg) return;
            lblMsg.innerText = texto || "";
        }

        const calendar = new FullCalendar.Calendar(calendarEl, {

            locale: 'es',

            buttonText: { today: 'Hoy', month: 'Mes', week: 'Semana', day: 'Día' },
            allDaySlot: false,
            initialView: 'timeGridWeek',
            height: 'auto',

            selectable: true,
            selectMirror: true,

            slotMinTime: '00:00:00',
            slotMaxTime: '24:00:00',
            slotDuration: '01:00:00',

            slotLabelFormat: { hour: '2-digit', minute: '2-digit', hour12: false },

            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
            },

            events: function (fetchInfo, successCallback, failureCallback) {
                const colabId = getColaboradorId();
                if (!colabId) {
                    successCallback([]);
                    return;
                }

                const url = '/controladores/HorariosHandler.ashx?accion=listar&colaboradorId=' + encodeURIComponent(colabId);
                fetch(url)
                    .then(r => r.json())
                    .then(data => successCallback(data))
                    .catch(err => failureCallback(err));
            },

            select: function (info) {

                const colabId = getColaboradorId();
                if (!colabId) {
                    setMsg("Selecciona un colaborador primero 💖");
                    calendar.unselect();
                    return;
                }

                setMsg("");

                fetch('/controladores/HorariosHandler.ashx?accion=crear', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    body:
                        'colaboradorId=' + encodeURIComponent(colabId) +
                        '&fecha=' + encodeURIComponent(info.startStr.substring(0, 10)) +
                        '&inicio=' + encodeURIComponent(info.startStr.substring(11, 16)) +
                        '&fin=' + encodeURIComponent(info.endStr.substring(11, 16))
                })
                    .then(r => r.json())
                    .then(res => {
                        if (res && res.msg) setMsg(res.msg);
                        calendar.refetchEvents();
                    });

                calendar.unselect();
            },

            eventClick: function (info) {

                const colabId = getColaboradorId();
                if (!colabId) return;

                info.el.style.opacity = '0.5';

                fetch('/controladores/HorariosHandler.ashx?accion=eliminar', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    body: 'id=' + encodeURIComponent(info.event.id) + '&colaboradorId=' + encodeURIComponent(colabId)
                })
                    .then(r => r.json())
                    .then(res => {
                        if (res && res.msg) setMsg(res.msg);
                        calendar.refetchEvents();
                    });
            }
        });

        calendar.render();

        // Cuando cambio colaborador, refresco
        if (ddl) {
            ddl.addEventListener('change', function () {
                setMsg("");
                calendar.refetchEvents();
            });
        }
    });
</script>

</body>
</html>