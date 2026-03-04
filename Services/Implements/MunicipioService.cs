using Microsoft.EntityFrameworkCore;
using Models;
using Services.Interface;

namespace Services.Implements
{
    public class MunicipioService : IMunicipioService
    {
        private readonly GeoConnectContext _context;

        public MunicipioService(GeoConnectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetEstadisticasMunicipios(bool porComentarios = true, bool ascendente = false)
        {
            var query = _context.Municipios.Select(m => new {
                m.NombreMunicipio,
                TotalLugares = m.Lugares.Count,
                // Sumamos todos los comentarios de todos los lugares de este municipio
                TotalComentarios = m.Lugares.Sum(l => l.Comentarios.Count)
            });

            // Lógica de ordenamiento
            if (porComentarios)
                query = ascendente ? query.OrderBy(m => m.TotalComentarios) : query.OrderByDescending(m => m.TotalComentarios);
            else
                query = ascendente ? query.OrderBy(m => m.TotalLugares) : query.OrderByDescending(m => m.TotalLugares);

            return await query.Select(x => (object)x).ToListAsync();
        }
    }
}