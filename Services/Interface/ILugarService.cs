namespace Services.Interface
{
    public interface ILugarService
    {
        Task<IEnumerable<object>> GetLugaresCercanos(double lat, double lon, double radioEnMetros);
        Task<IEnumerable<object>> GetLugaresPopulares(bool ascendente = false);
    }
}