using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Models;
using Services.Interface;

namespace Services.Implements
{
    public class LugarService : ILugarService
    {
        private readonly GeoConnectContext _context;

        public LugarService(GeoConnectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetLugaresCercanos(double lat, double lon, double radioEnMetros)
        {
            // 4326 es el estándar GPS (WGS84)
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            // IMPORTANTE: En NTS la coordenada es (Longitud, Latitud), en ese orden.
            var miUbicacion = geometryFactory.CreatePoint(new Coordinate(lon, lat));

            var lugares = await _context.Lugares
                // Filtramos los que estén dentro del radio
                .Where(l => l.Coordenadas.Distance(miUbicacion) <= radioEnMetros)
                // Ordenamos del más cercano al más lejano
                .OrderBy(l => l.Coordenadas.Distance(miUbicacion))
                .Select(l => new {
                    l.GooglePlaceId,
                    l.NombreLugar,
                    DistanciaMetros = l.Coordenadas.Distance(miUbicacion),
                    TotalComentarios = _context.Comentarios.Count(c => c.GooglePlaceId == l.GooglePlaceId )
                }).ToListAsync();

            return lugares;
        }

        public async Task<IEnumerable<object>> GetLugaresPopulares(bool ascendente = false)
        {
            var query = _context.Lugares.Select(l => new{
                l.NombreLugar,
                //
                CantidadComentarios = _context.Comentarios.Count(c=>c.GooglePlaceId==l.GooglePlaceId)
            });

            if (ascendente)
                query = query.OrderBy(x => x.CantidadComentarios);
            else
                query = query.OrderByDescending(x => x.CantidadComentarios); // Más comentados primero

            var lugares = await query.ToListAsync();

            return lugares;
        }
    }
}