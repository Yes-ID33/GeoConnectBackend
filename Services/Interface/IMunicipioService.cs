namespace Services.Interface
{
    public interface IMunicipioService
    {
        Task<IEnumerable<EstadisticaMunicipioResponseDto>> GetEstadisticasMunicipios(bool porComentarios = true, bool ascendente = false);
    }

    public class EstadisticaMunicipioResponseDto
    {
        public string NombreMunicipio { get; set; } = string.Empty;
        public int TotalLugares { get; set; }
        public int TotalComentarios { get; set; }
    }
}