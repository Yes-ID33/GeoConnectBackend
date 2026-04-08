namespace Services.Interface
{
    public interface IEmailService
    {
        Task EnviarCorreoActivacionAsync(string correoDestino, string nombre, string token);
    }
}