namespace Services.Interface
{
    public interface IAccionLugarService
    {
        // 1. Reemplazamos IEnumerable<object> por IEnumerable<AccionUsuarioResponseDto>
        Task<IEnumerable<AccionUsuarioResponseDto>> GetAccionesUsuario(int idUsuario, string? tipoAccion, bool ordenarPorLugar = false);

        Task<(bool Exito, string Mensaje, object? Datos)> ToggleAccion(AccionLugarDto dto);
    }

    // DTO DE ENTRADA (Lo que recibimos del Frontend)
    public class AccionLugarDto
    {
        public int IdUsuario { get; set; }

        // ¡CAMBIO CLAVE! Ya no recibimos el string de Google, sino tu ID interno
        public int IdLugar { get; set; }

        public string TipoAccion { get; set; } = string.Empty;
    }

    // DTO DE SALIDA (Lo que le enviamos al Frontend)
    public class AccionUsuarioResponseDto
    {
        public int IdAccion { get; set; }
        public string TipoAccion { get; set; } = string.Empty;
        public DateTime? FechaAccion { get; set; }
        public string NombreLugar { get; set; } = string.Empty;
        public int IdLugar { get; set; }

        // Dejamos el de Google opcional por si el Frontend necesita pintar un mapa
        public string? GooglePlaceId { get; set; }
    }
}