namespace Services.Interface
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioResponseDto>> ListarUsuarios();

        // Unificamos el patrón de respuesta con Tupla y usamos DTO de entrada
        Task<(bool Exito, string Mensaje, UsuarioResponseDto? Datos)> CrearUsuario(CrearUsuarioDto dto);

        // Usamos DTO para evitar que el front envíe datos que no debe tocar
        Task<(bool Exito, string Mensaje)> ActualizarUsuario(int id, ActualizarUsuarioDto dto);

        Task<(bool Exito, string Mensaje)> DesactivarCuenta(int id);
        Task<(bool Exito, string Mensaje)> ReactivarCuenta(int id, string nuevoCorreo, string nuevoNombre);
    }

    // DTO DE SALIDA (Oculta la contraseña y datos sensibles)
    public class UsuarioResponseDto
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    // DTO DE ENTRADA (Solo lo necesario para registrarse)
    public class CrearUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }

    // DTO DE ENTRADA (Para editar perfil)
    public class ActualizarUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}