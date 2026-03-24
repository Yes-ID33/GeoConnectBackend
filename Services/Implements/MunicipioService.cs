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

        public async Task<IEnumerable<EstadisticaMunicipioResponseDto>> GetEstadisticasMunicipios(bool porComentarios = true, bool ascendente = false)
        {
            var query = _context.Municipios.Select(m => new EstadisticaMunicipioResponseDto{
                NombreMunicipio = m.NombreMunicipio,
                TotalLugares = _context.Lugares.Count(l=>l.IdMunicipio == m.IdMunicipio),
                // Sumamos todos los comentarios de todos los lugares de este municipio
                TotalComentarios = _context.Comentarios.Count(c => c.Lugar != null && c.Lugar.IdMunicipio == m.IdMunicipio)
            });

            // Lógica de ordenamiento
            if (porComentarios)
                query = ascendente ? query.OrderBy(m => m.TotalComentarios) : query.OrderByDescending(m => m.TotalComentarios);
            else
                query = ascendente ? query.OrderBy(m => m.TotalLugares) : query.OrderByDescending(m => m.TotalLugares);

            return await query.ToListAsync();
        }
    }
}