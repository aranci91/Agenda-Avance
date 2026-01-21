using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace Agenda.Servicios
{
    public class EmailService
    {
        public static void EnviarCodigoVerificacion(string destinatarioEmail, string codigo)
        {
            if (string.IsNullOrWhiteSpace(destinatarioEmail))
                throw new Exception("El correo del destinatario está vacío.");

            string gmailUser = ConfigurationManager.AppSettings["GmailUser"];
            string gmailAppPass = ConfigurationManager.AppSettings["GmailAppPassword"];
            string fromName = ConfigurationManager.AppSettings["GmailFromName"] ?? "Yessi Aranci";

            if (string.IsNullOrWhiteSpace(gmailUser) || string.IsNullOrWhiteSpace(gmailAppPass))
                throw new Exception("Faltan credenciales SMTP en Web.config (GmailUser / GmailAppPassword).");

            gmailAppPass = gmailAppPass.Replace(" ", "");

            string asunto = "Código de verificación - Yessi Aranci";
            string cuerpoHtml = $@"
<div style='font-family: Arial, sans-serif; line-height:1.5;'>
  <h2 style='margin:0;'>Hola 👋</h2>
  <p>Tu código de verificación para ingresar es:</p>
  <div style='font-size:28px; font-weight:bold; letter-spacing:3px; 
              background:#ffe3ef; padding:14px 18px; display:inline-block; border-radius:12px;'>
    {codigo}
  </div>
  <p style='margin-top:18px; color:#444;'>
    Si tú no solicitaste este código, puedes ignorar este correo.
  </p>
  <hr style='border:none; border-top:1px solid #eee; margin:18px 0;' />
  <p style='font-size:12px; color:#777; margin:0;'>Yessi Aranci</p>
</div>";

            EnviarHtmlSimple(destinatarioEmail, asunto, cuerpoHtml, gmailUser, gmailAppPass, fromName);
        }

        public static void EnviarConfirmacionCita(
            string destinatarioEmail,
            string nombreCliente,
            string servicioNombre,
            int duracionMin,
            decimal precio,
            DateTime fecha,
            string hora,
            string reglas,
            string datosTransferencia)
        {
            if (string.IsNullOrWhiteSpace(destinatarioEmail))
                throw new Exception("El correo del destinatario está vacío.");

            string gmailUser = ConfigurationManager.AppSettings["GmailUser"];
            string gmailAppPass = ConfigurationManager.AppSettings["GmailAppPassword"];
            string fromName = ConfigurationManager.AppSettings["GmailFromName"] ?? "Yessi Aranci";

            if (string.IsNullOrWhiteSpace(gmailUser) || string.IsNullOrWhiteSpace(gmailAppPass))
                throw new Exception("Faltan credenciales SMTP en Web.config (GmailUser / GmailAppPassword).");

            gmailAppPass = gmailAppPass.Replace(" ", "");

            string asunto = "Confirmación de cita - Yessi Aranci";

            string precioCl = precio.ToString("C0", new CultureInfo("es-CL"));
            string fechaStr = fecha.ToString("dd-MM-yyyy");

            string reglasHtml = (reglas ?? "").Replace("\n", "<br/>");
            string transferenciaHtml = (datosTransferencia ?? "").Replace("\n", "<br/>");

            string cuerpoHtml = $@"
<div style='font-family: Arial, sans-serif; line-height:1.6; color:#222;'>
  <h2 style='margin:0;'>Hola {System.Net.WebUtility.HtmlEncode(nombreCliente)} 👋</h2>
  <p style='margin-top:10px;'>Tu cita fue solicitada con éxito. Aquí tienes el detalle:</p>

  <div style='background:#fff1f7; border:1px solid #ffd0e3; border-radius:14px; padding:14px 16px;'>
    <p style='margin:0;'><strong>Servicio:</strong> {System.Net.WebUtility.HtmlEncode(servicioNombre)}</p>
    <p style='margin:0;'><strong>Duración:</strong> {duracionMin} min</p>
    <p style='margin:0;'><strong>Fecha:</strong> {fechaStr}</p>
    <p style='margin:0;'><strong>Hora:</strong> {System.Net.WebUtility.HtmlEncode(hora)}</p>
    <p style='margin:0;'><strong>Valor total:</strong> {precioCl}</p>
  </div>

  <h3 style='margin:18px 0 6px;'>Reglas</h3>
  <div style='background:#ffffff; border:1px solid #eee; border-radius:12px; padding:12px 14px;'>
    {reglasHtml}
  </div>

  <h3 style='margin:18px 0 6px;'>Pago para asegurar tu hora</h3>
  <p style='margin-top:0;'>
    Recuerda realizar el pago <strong>antes de las 00:00 de hoy</strong> para no perder tu cita.
  </p>

  <div style='background:#ffffff; border:1px solid #eee; border-radius:12px; padding:12px 14px;'>
    {transferenciaHtml}
  </div>

  <hr style='border:none; border-top:1px solid #eee; margin:18px 0;' />
  <p style='font-size:12px; color:#777; margin:0;'>Yessi Aranci</p>
</div>";

            EnviarHtmlSimple(destinatarioEmail, asunto, cuerpoHtml, gmailUser, gmailAppPass, fromName);
        }

        //  NUEVO: ENVÍO MASIVO (B: uno por uno) + logo embebido
      
        public static int EnviarMensajeMasivoAClientes(List<string> correosClientes, string asunto, string cuerpoHtmlEditor, string logoPathFisico)
        {
            if (correosClientes == null || correosClientes.Count == 0)
                return 0;

            string gmailUser = ConfigurationManager.AppSettings["GmailUser"];
            string gmailAppPass = ConfigurationManager.AppSettings["GmailAppPassword"];
            string fromName = ConfigurationManager.AppSettings["GmailFromName"] ?? "Yessi Aranci";

            if (string.IsNullOrWhiteSpace(gmailUser) || string.IsNullOrWhiteSpace(gmailAppPass))
                throw new Exception("Faltan credenciales SMTP en Web.config (GmailUser / GmailAppPassword).");

            gmailAppPass = gmailAppPass.Replace(" ", "");

            int enviados = 0;

            foreach (string correo in correosClientes.Distinct())
            {
                if (string.IsNullOrWhiteSpace(correo)) continue;

                // Plantilla rosadita con firma
                string htmlFinal = ConstruirPlantillaMasiva(cuerpoHtmlEditor);

                // Envío con logo embebido (cid)
                EnviarHtmlConLogoEmbebido(correo.Trim(), asunto, htmlFinal, gmailUser, gmailAppPass, fromName, logoPathFisico);

                enviados++;
            }

            return enviados;
        }

        private static string ConstruirPlantillaMasiva(string cuerpoHtmlEditor)
        {
            string contenido = (cuerpoHtmlEditor ?? "").Trim();
            if (string.IsNullOrWhiteSpace(contenido))
                contenido = "<p>Hola 💖</p>";

            // Logo embebido: cid:logoYA
            return $@"
<div style='font-family: Arial, sans-serif; line-height:1.6; color:#222; background:#fff7fb; padding:18px;'>
  <div style='max-width:700px; margin:0 auto;'>
    <div style='background:#ffe3ef; border:1px solid #ffd0e3; border-radius:18px; padding:14px 16px;'>
      <div style='display:flex; gap:12px; align-items:center;'>
        <img src='cid:logoYA' alt='Yessi Aranci' style='width:44px; height:44px; border-radius:12px;'/>
        <div style='font-weight:900; font-size:16px;'>YESSI ARANCI</div>
      </div>
    </div>

    <div style='background:#ffffff; border:1px solid #ffd0e3; border-radius:18px; padding:16px 18px; margin-top:12px;'>
      {contenido}
    </div>

    <div style='margin-top:12px; background:#ffe3ef; border:1px solid #ffd0e3; border-radius:18px; padding:14px 16px;'>
      <div style='display:flex; gap:12px; align-items:center;'>
        <img src='cid:logoYA' alt='Yessi Aranci' style='width:36px; height:36px; border-radius:10px;'/>
        <div>
          <div style='font-weight:900;'>Yessi Aranci</div>
          <div style='font-size:12px; opacity:0.85;'>Estudio de estética</div>
        </div>
      </div>
    </div>

    <p style='font-size:11px; color:#777; margin:12px 0 0 0;'>
      Si no deseas recibir este tipo de mensajes, responde este correo solicitando la baja.
    </p>
  </div>
</div>";
        }

        // Envía HTML simple (lo que ya tenías)
        private static void EnviarHtmlSimple(string destinatarioEmail, string asunto, string cuerpoHtml, string gmailUser, string gmailAppPass, string fromName)
        {
            using (var msg = new MailMessage())
            {
                msg.From = new MailAddress(gmailUser, fromName);
                msg.To.Add(destinatarioEmail);
                msg.Subject = asunto;
                msg.Body = cuerpoHtml;
                msg.IsBodyHtml = true;

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(gmailUser, gmailAppPass);
                    smtp.Send(msg);
                }
            }
        }

        // Envía HTML + logo embebido por CID
        private static void EnviarHtmlConLogoEmbebido(
            string destinatarioEmail,
            string asunto,
            string cuerpoHtml,
            string gmailUser,
            string gmailAppPass,
            string fromName,
            string logoPathFisico)
        {
            using (var msg = new MailMessage())
            {
                msg.From = new MailAddress(gmailUser, fromName);
                msg.To.Add(destinatarioEmail);
                msg.Subject = asunto;
                msg.IsBodyHtml = true;

                // AlternateView con recurso embebido
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString(cuerpoHtml, null, MediaTypeNames.Text.Html);

                try
                {
                    if (!string.IsNullOrWhiteSpace(logoPathFisico) && System.IO.File.Exists(logoPathFisico))
                    {
                        LinkedResource logo = new LinkedResource(logoPathFisico);
                        logo.ContentId = "logoYA";
                        logo.TransferEncoding = TransferEncoding.Base64;
                        avHtml.LinkedResources.Add(logo);
                    }
                }
                catch
                {
                    // Si falla el logo, no rompe el envío
                }

                msg.AlternateViews.Add(avHtml);

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(gmailUser, gmailAppPass);
                    smtp.Send(msg);
                }
            }
        }

        public static void EnviarMasivoClientes(List<string> destinatarios, string asunto, string cuerpoHtml)
        {
            if (destinatarios == null || destinatarios.Count == 0)
                throw new Exception("No hay destinatarios para el envío masivo.");

            if (string.IsNullOrWhiteSpace(asunto))
                throw new Exception("El asunto está vacío.");

            string gmailUser = System.Configuration.ConfigurationManager.AppSettings["GmailUser"];
            string gmailAppPass = System.Configuration.ConfigurationManager.AppSettings["GmailAppPassword"];
            string fromName = System.Configuration.ConfigurationManager.AppSettings["GmailFromName"] ?? "Yessi Aranci";

            if (string.IsNullOrWhiteSpace(gmailUser) || string.IsNullOrWhiteSpace(gmailAppPass))
                throw new Exception("Faltan credenciales SMTP en Web.config (GmailUser / GmailAppPassword).");

            gmailAppPass = gmailAppPass.Replace(" ", "");

            using (var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587))
            {
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(gmailUser, gmailAppPass);

                foreach (string mail in destinatarios.Distinct())
                {
                    if (string.IsNullOrWhiteSpace(mail)) continue;

                    using (var msg = new System.Net.Mail.MailMessage())
                    {
                        msg.From = new System.Net.Mail.MailAddress(gmailUser, fromName);
                        msg.To.Add(mail.Trim());
                        msg.Subject = asunto;
                        msg.Body = cuerpoHtml;
                        msg.IsBodyHtml = true;

                        smtp.Send(msg);
                    }
                }
            }
        }

    }
}
