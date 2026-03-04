namespace Services.Interface
{
    public interface IMunicipioService
    {
        Task<IEnumerable<object>> GetEstadisticasMunicipios(bool porComentarios = true, bool ascendente = false);
    }
}