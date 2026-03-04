namespace Services.Interface
{
    public interface IComentarioService
    {
        Task<IEnumerable<object>> GetComentariosPorLugar(string googlePlaceId);
        Task<IEnumerable<object>> GetComentariosPorUsuario(int IdUsuario);
        // Usamos una tupla para devolver el éxito, el mensaje y los datos creados
        Task<(bool Exito, string Mensaje, object? Datos)> CrearComentario(CrearComentarioDto dto);
    }

    public class CrearComentarioDto
    {
        public int IdUsuario { get; set; }
        public string GooglePlaceId { get; set; } = string.Empty;
        public string TextoComentario { get; set; } = string.Empty;
    }
}