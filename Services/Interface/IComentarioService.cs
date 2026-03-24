namespace Services.Interface
{
    public interface IComentarioService
    {
        // 1. Cambiamos string googlePlaceId por int idLugar
        Task<IEnumerable<ComentarioLugarResponseDto>> GetComentariosPorLugar(int idLugar);

        Task<IEnumerable<ComentarioUsuarioResponseDto>> GetComentariosPorUsuario(int idUsuario);

        Task<(bool Exito, string Mensaje, object? Datos)> CrearComentario(CrearComentarioDto dto);
    }

    // DTO DE ENTRADA
    public class CrearComentarioDto
    {
        public int IdUsuario { get; set; }

        // ¡CAMBIO CLAVE! Usamos el ID interno
        public int IdLugar { get; set; }

        public string TextoComentario { get; set; } = string.Empty;

        // ¡AGREGADO! Necesario para cumplir la regla de la BD
        public float Calificacion { get; set; }
    }

    // DTO DE SALIDA (Para la vista del Lugar)
    public class ComentarioLugarResponseDto
    {
        public int IdComentario { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public float Calificacion { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public string Autor { get; set; } = string.Empty;
    }

    // DTO DE SALIDA (Para el perfil del Usuario)
    public class ComentarioUsuarioResponseDto
    {
        public int IdComentario { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public float Calificacion { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public int IdLugar { get; set; }
        public string NombreLugar { get; set; } = string.Empty;
    }
}