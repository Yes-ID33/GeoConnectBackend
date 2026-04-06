using System.Diagnostics;

namespace Services.Interface
{
    public interface IEmailService
    {
        Task EnviarCorreoActivacionAsync(string correoDestino, string nombre, string token) 
        {
            // Por ahora, lo imprimimos en la consola de depuración para poder probar la API.
            Debug.WriteLine("==================================================");
            Debug.WriteLine($"MOCK EMAIL ENVIADO A: {correoDestino}");
            Debug.WriteLine($"ASUNTO: Código de verificación GeoConnect");
            Debug.WriteLine($"HOLA {nombre}, TU CÓDIGO ES: {token}");
            Debug.WriteLine("==================================================");

            return Task.CompletedTask;
        }
    }
}
