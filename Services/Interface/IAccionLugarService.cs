namespace Services.Interface
{
    public interface IAccionLugarService
    {
        Task<IEnumerable<object>> GetAccionesUsuario(int idUsuario, string? tipoAccion, bool ordenarPorLugar = false);
        Task<(bool Exito, string Mensaje, object? Datos)> ToggleAccion(AccionLugarDto dto);
    }

    public class AccionLugarDto
    {
        public int IdUsuario { get; set; }
        public string GooglePlaceId { get; set; } = string.Empty;
        public string TipoAccion { get; set; } = string.Empty;
    }
}