namespace Services.Interface
{
    public interface ILugarService
    {
        Task<IEnumerable<LugarCercanoResponseDto>> GetLugaresCercanos(double lat, double lon, double radioEnMetros);
        Task<IEnumerable<LugarPopularResponseDto>> GetLugaresPopulares(bool ascendente = false);
    }

    public class LugarCercanoResponseDto
    {
        public int IdLugar { get; set; }
        public string? GooglePlaceId { get; set; }
        public string NombreLugar { get; set; } = string.Empty;
        public double DistanciaMetros { get; set; }
        public int TotalComentarios { get; set; }
    }

    public class LugarPopularResponseDto
    {
        public int IdLugar { get; set; }
        public string? GooglePlaceId { get; set; }
        public string NombreLugar { get; set; } = string.Empty;
        public int CantidadComentarios { get; set; }

        // ¡AGREGADO! Promedio de estrellas
        public double CalificacionPromedio { get; set; }
    }
}