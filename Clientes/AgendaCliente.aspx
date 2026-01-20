<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AgendaCliente.aspx.cs" Inherits="Agenda.Clientes.AgendaCliente" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Agenda tu hora</title>

<link href="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.11/index.global.min.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.11/index.global.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.11/locales/es.global.min.js"></script>

<link rel="stylesheet" href="../styles.css" />
</head>

<body>
<form runat="server">

<div class="content">
    <h1>Agenda tu hora</h1>
    <p class="sub">Selecciona un horario disponible</p>

    <div class="card">
        <div id="calendar"></div>
    </div>
</div>

</form>

<script>
    document.addEventListener('DOMContentLoaded', function () {

        var calendar = new FullCalendar.Calendar(
            document.getElementById('calendar'), {

            locale: 'es',
            initialView: 'timeGridWeek',
            height: 'auto',

            selectable: false,
            editable: false,
            allDaySlot: false,

            slotMinTime: '00:00:00',
            slotMaxTime: '24:00:00',
            slotDuration: '01:00:00',

            slotLabelFormat: {
                hour: '2-digit',
                minute: '2-digit',
                hour12: false
            },

            headerToolbar: {
                left: 'prev,next',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek'
            },

            events: '/controladores/HorariosHandler.ashx?accion=listar_cliente',

            eventClick: function (info) {

                if (!confirm(
                    '¿Confirmar hora?\n\n' +
                    info.event.start.toLocaleString()
                )) {
                    return;
                }

                fetch('/controladores/HorariosHandler.ashx?accion=reservar', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    body: 'id=' + info.event.id
                })
                    .then(r => r.json())
                    .then(() => {
                        alert('Hora reservada con éxito 💖');
                        calendar.refetchEvents();
                    });
            }
        });

        calendar.render();
    });
</script>

</body>
</html>