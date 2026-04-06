namespace Services.Interface
{
    public interface IAuthService
    {
        Task<(bool Exito, string Mensaje, string? Token)> LoginAsync(LoginDto dto);
        Task<(bool Exito, string Mensaje, string? Token)> VerificarCuentaAsync(VerificarCuentaDto dto);
    }

    public class LoginDto
    {
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }

    public class VerificarCuentaDto
    {
        public string Correo { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}