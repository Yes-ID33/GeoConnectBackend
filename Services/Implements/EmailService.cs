using Microsoft.Extensions.Configuration;
using Services.Interface;
using System.Net;
using System.Net.Mail;

namespace Services.Implements
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoActivacionAsync(string correoDestino, string nombre, string token)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            string senderEmail = emailSettings["SenderEmail"]!;
            string senderPassword = emailSettings["Password"]!;
            string senderName = emailSettings["SenderName"]!;
            string smtpServer = emailSettings["SmtpServer"]!;
            int port = int.Parse(emailSettings["Port"]!);

            var fromAddress = new MailAddress(senderEmail, senderName);
            var toAddress = new MailAddress(correoDestino, nombre);

            var smtp = new SmtpClient
            {
                Host = smtpServer,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, senderPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "¡Activa tu cuenta de GeoConnect! 🌍",
                Body = $@"
                    <div style='font-family: Arial, sans-serif; text-align: center; padding: 20px; background-color: #f4f4f4; border-radius: 10px;'>
                        <h2 style='color: #333;'>¡Hola {nombre}! Bienvenido a GeoConnect</h2>
                        <p style='color: #555;'>Para activar tu cuenta y empezar a explorar, ingresa el siguiente código de verificación:</p>
                        <div style='background-color: #fff; padding: 15px; display: inline-block; border-radius: 5px; border: 2px dashed #2E86C1; margin: 20px 0;'>
                            <h1 style='color: #2E86C1; letter-spacing: 5px; margin: 0;'>{token}</h1>
                        </div>
                        <p style='color: #777; font-size: 12px;'>Este código expirará en 15 minutos.</p>
                    </div>",
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
        }

        public async Task EnviarReporteConsumoCoordsAsync(string correoDesarrollador, List<string> lugaresFallidos)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            string senderEmail = emailSettings["SenderEmail"]!;
            string senderPassword = emailSettings["Password"]!;
            string senderName = emailSettings["SenderName"]!;
            string smtpServer = emailSettings["SmtpServer"]!;
            int port = int.Parse(emailSettings["Port"]!);

            var fromAddress = new MailAddress(senderEmail, senderName);
            var toAddress = new MailAddress(correoDesarrollador, "Equipo de Desarrollo");

            var smtp = new SmtpClient
            {
                Host = smtpServer,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, senderPassword)
            };

            // Construimos la lista HTML con los lugares
            string listaHtml = "";
            foreach (var lugar in lugaresFallidos)
            {
                listaHtml += $"<li style='color: #d9534f; margin-bottom: 5px;'><strong>{lugar}</strong></li>";
            }

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "⚠️ Reporte del Worker: Lugares no encontrados en Nominatim",
                Body = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; background-color: #f9f9f9; border-radius: 10px; border-left: 5px solid #d9534f;'>
                <h2 style='color: #333;'>¡Atención Devs! 🛠️</h2>
                <p style='color: #555;'>El Worker de Geocodificación ha terminado su ciclo, pero no pudo encontrar las coordenadas de los siguientes lugares. Revisen el <strong>DataSeeder</strong> para afinar los nombres y direcciones:</p>
                <ul style='background-color: #fff; padding: 15px 15px 15px 40px; border-radius: 5px; border: 1px solid #ddd;'>
                    {listaHtml}
                </ul>
                <p style='color: #777; font-size: 12px; margin-top: 20px;'>Este es un mensaje automático de tu GeoConnectBackend.</p>
            </div>",
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}