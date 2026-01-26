using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace Agenda.controladores
{
    public class HorariosHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            string accion = (context.Request["accion"] ?? "").ToLower().Trim();

            switch (accion)
            {
                case "listar":
                    // ADMIN: lista horarios de 1 colaborador
                    ListarHorariosAdmin(context);
                    break;

                case "listar_cliente":
                    // CLIENTE: lista horarios del colaborador (para futuro uso con calendario)
                    ListarHorariosCliente(context);
                    break;

                case "crear":
                    // ADMIN: crea horarios para 1 colaborador
                    CrearHorario(context);
                    break;

                case "eliminar":
                    // ADMIN: elimina (solo si está disponible)
                    EliminarHorario(context);
                    break;

                case "reservar":
                    // marca Disponible=0
                    ReservarHorario(context);
                    break;

                default:
                    context.Response.Write("[]");
                    break;
            }
        }

        private bool EsAdmin(HttpContext context)
        {
            if (context.Session == null) return false;
            if (context.Session["Rol"] == null) return false;
            return Convert.ToInt32(context.Session["Rol"]) == 3;
        }


        // ADMIN - LISTAR (por colaborador)
       
        private void ListarHorariosAdmin(HttpContext context)
        {
            if (!EsAdmin(context))
            {
                context.Response.Write("[]");
                return;
            }

            if (!int.TryParse(context.Request["colaboradorId"], out int colaboradorId) || colaboradorId <= 0)
            {
                context.Response.Write("[]");
                return;
            }

            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT HorarioID, Fecha, HoraInicio, HoraFin
FROM dbo.Horarios
WHERE Disponible = 1
  AND UsuarioID_Colaborador = @c
  AND Fecha >= CAST(GETDATE() AS date)
ORDER BY Fecha, HoraInicio;";

            SqlParameter[] p =
            {
                new SqlParameter("@c", colaboradorId)
            };

            DataTable dt = bd.EjecutarConsulta(sql, p);

            var eventos = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                string fecha = Convert.ToDateTime(row["Fecha"]).ToString("yyyy-MM-dd");

                eventos.Add(new
                {
                    id = row["HorarioID"],
                    title = "Disponible",
                    start = fecha + "T" + row["HoraInicio"],
                    end = fecha + "T" + row["HoraFin"]
                });
            }

            context.Response.Write(new JavaScriptSerializer().Serialize(eventos));
        }

        // CLIENTE - LISTAR (por colaborador)
       
        private void ListarHorariosCliente(HttpContext context)
        {
            if (!int.TryParse(context.Request["colaboradorId"], out int colaboradorId) || colaboradorId <= 0)
            {
                context.Response.Write("[]");
                return;
            }

            ConexionBD bd = new ConexionBD();

            string sql = @"
SELECT HorarioID, Fecha, HoraInicio, HoraFin
FROM dbo.Horarios
WHERE Disponible = 1
  AND UsuarioID_Colaborador = @c
  AND Fecha >= CAST(GETDATE() AS date)
ORDER BY Fecha, HoraInicio;";

            SqlParameter[] p =
            {
                new SqlParameter("@c", colaboradorId)
            };

            DataTable dt = bd.EjecutarConsulta(sql, p);

            var eventos = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                string fecha = Convert.ToDateTime(row["Fecha"]).ToString("yyyy-MM-dd");

                eventos.Add(new
                {
                    id = row["HorarioID"],
                    title = "Horario disponible",
                    start = fecha + "T" + row["HoraInicio"],
                    end = fecha + "T" + row["HoraFin"]
                });
            }

            context.Response.Write(new JavaScriptSerializer().Serialize(eventos));
        }

      
        // CREAR HORARIO (ADMIN) - por colaborador + crea slots por hora
      
        private void CrearHorario(HttpContext context)
        {
            if (!EsAdmin(context))
            {
                context.Response.Write("{\"ok\":false,\"msg\":\"Sin permiso.\"}");
                return;
            }

            if (!int.TryParse(context.Request["colaboradorId"], out int colaboradorId) || colaboradorId <= 0)
            {
                context.Response.Write("{\"ok\":false,\"msg\":\"Selecciona un colaborador.\"}");
                return;
            }

            DateTime fecha = Convert.ToDateTime(context.Request["fecha"]);
            TimeSpan inicio = TimeSpan.Parse(context.Request["inicio"]);
            TimeSpan fin = TimeSpan.Parse(context.Request["fin"]);

            if (fecha.Date < DateTime.Today)
            {
                context.Response.Write("{\"ok\":false,\"msg\":\"No puedes crear horarios en el pasado.\"}");
                return;
            }

            // FullCalendar puede seleccionar 1h o varias horas, así que insertamos por bloques de 60
            ConexionBD bd = new ConexionBD();
            int creados = 0;

            TimeSpan actual = inicio;

            while (actual < fin)
            {
                TimeSpan horaInicio = actual;
                TimeSpan horaFin = actual.Add(TimeSpan.FromHours(1));

                string sql = @"
IF NOT EXISTS (
    SELECT 1
    FROM dbo.Horarios
    WHERE UsuarioID_Colaborador = @c
      AND Fecha = @f
      AND HoraInicio = @i
)
BEGIN
    INSERT INTO dbo.Horarios (UsuarioID_Colaborador, Fecha, HoraInicio, HoraFin, Disponible)
    VALUES (@c, @f, @i, @fin, 1);
END";

                SqlParameter[] p =
                {
                    new SqlParameter("@c", colaboradorId),
                    new SqlParameter("@f", fecha.Date),
                    new SqlParameter("@i", horaInicio),
                    new SqlParameter("@fin", horaFin)
                };

                bd.EjecutarComando(sql, p);
                creados++;

                actual = actual.Add(TimeSpan.FromHours(1));
            }

            context.Response.Write("{\"ok\":true,\"msg\":\"Horario(s) guardado(s) ✅\"}");
        }

       
        // ELIMINAR HORARIO (ADMIN) - solo si está disponible
       
        private void EliminarHorario(HttpContext context)
        {
            if (!EsAdmin(context))
            {
                context.Response.Write("{\"ok\":false,\"msg\":\"Sin permiso.\"}");
                return;
            }

            if (!int.TryParse(context.Request["id"], out int id) || id <= 0)
            {
                context.Response.Write("{\"ok\":false,\"msg\":\"ID inválido.\"}");
                return;
            }

            ConexionBD bd = new ConexionBD();

            // Importante: no borrar si ya está reservado (Disponible=0)
            string sql = @"
DELETE FROM dbo.Horarios
WHERE HorarioID = @id
  AND Disponible = 1;";

            SqlParameter[] p =
            {
                new SqlParameter("@id", id)
            };

            bd.EjecutarComando(sql, p);

            context.Response.Write("{\"ok\":true,\"msg\":\"Horario eliminado ✅\"}");
        }


        // RESERVAR HORARIO (bloquear)

        private void ReservarHorario(HttpContext context)
        {
            int horarioId = Convert.ToInt32(context.Request["id"]);
            int clienteId = Convert.ToInt32(context.Session["UsuarioID"]);
            int colaboradorId = Convert.ToInt32(context.Session["UsuarioID_Colaborador"]);
            int servicioId = Convert.ToInt32(context.Session["ServicioID"]);

            ConexionBD bd = new ConexionBD();

            // 1. Bloquear horario
            string sqlUpdate = @"UPDATE Horarios SET Disponible = 0 WHERE HorarioID = @id";
            bd.EjecutarComando(sqlUpdate, new SqlParameter[] {
        new SqlParameter("@id", horarioId)
    });

            // 2. Insertar cita
            string sqlInsert = @"
    INSERT INTO Citas
    (UsuarioID_Cliente, UsuarioID_Colaborador, ServicioID, HorarioID, FechaHora, Estado)
    VALUES
    (@cliente, @colab, @servicio, @horario, GETDATE(), 'Reservada')";

            bd.EjecutarComando(sqlInsert, new SqlParameter[] {
        new SqlParameter("@cliente", clienteId),
        new SqlParameter("@colab", colaboradorId),
        new SqlParameter("@servicio", servicioId),
        new SqlParameter("@horario", horarioId)
    });

            // 3. Obtener datos para correo
            string sqlDatos = @"
    SELECT 
      u.Correo,
      u.Nombre,
      s.NombreServicio,
      s.DuracionMin,
      s.Precio,
      h.Fecha,
      h.HoraInicio,
      s.Reglas,
      s.DatosTransferencia
    FROM Citas c
    JOIN Usuarios u ON u.UsuarioID = c.UsuarioID_Cliente
    JOIN Servicios s ON s.ServicioID = c.ServicioID
    JOIN Horarios h ON h.HorarioID = c.HorarioID
    WHERE c.HorarioID = @id";

            DataTable dt = bd.EjecutarConsulta(sqlDatos, new SqlParameter[] {
        new SqlParameter("@id", horarioId)
    });

            if (dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];

                Agenda.Servicios.EmailService.EnviarConfirmacionCita(
                    r["Correo"].ToString(),
                    r["Nombre"].ToString(),
                    r["NombreServicio"].ToString(),
                    Convert.ToInt32(r["DuracionMin"]),
                    Convert.ToDecimal(r["Precio"]),
                    Convert.ToDateTime(r["Fecha"]),
                    r["HoraInicio"].ToString(),
                    r["Reglas"].ToString(),
                    r["DatosTransferencia"].ToString()
                );
            }

            context.Response.Write("{\"ok\":true}");
        }




        public bool IsReusable => false;
    }
}
