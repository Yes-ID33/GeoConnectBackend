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

        public async Task<IEnumerable<LugarCercanoResponseDto>> GetLugaresCercanos(double lat, double lon, double radioEnMetros)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var miUbicacion = geometryFactory.CreatePoint(new Coordinate(lon, lat));

            var lugares = await _context.Lugares
                // ¡CORRECCIÓN! Primero verificamos que las coordenadas NO sean nulas
                .Where(l => l.Coordenadas != null && l.Coordenadas.Distance(miUbicacion) <= radioEnMetros)
                .OrderBy(l => l.Coordenadas!.Distance(miUbicacion))
                .Select(l => new LugarCercanoResponseDto
                {
                    IdLugar = l.IdLugar,
                    GooglePlaceId = l.GooglePlaceId,
                    NombreLugar = l.NombreLugar,
                    DistanciaMetros = l.Coordenadas!.Distance(miUbicacion),
                    TotalComentarios = _context.Comentarios.Count(c => c.IdLugar == l.IdLugar),
                    FotoUrl = l.FotoUrl
                }).ToListAsync();

            return lugares;
        }

        public async Task<IEnumerable<LugarPopularResponseDto>> GetLugaresPopulares(bool ascendente = false)
        {
            var query = _context.Lugares.Select(l => new LugarPopularResponseDto
            {
                IdLugar = l.IdLugar,
                GooglePlaceId = l.GooglePlaceId,
                NombreLugar = l.NombreLugar,
                CantidadComentarios = _context.Comentarios.Count(c => c.IdLugar == l.IdLugar),

                // ¡LA MAGIA AQUÍ! Calculamos el promedio al vuelo. 
                // Usamos un casteo a double? para que no explote si un lugar tiene 0 comentarios.
                CalificacionPromedio = _context.Comentarios
                                        .Where(c => c.IdLugar == l.IdLugar)
                                        .Average(c => (double?)c.Calificacion) ?? 0.0,
                FotoUrl = l.FotoUrl
            });

            // Opcional: Podrías ordenar por Calificación en lugar de Cantidad de Comentarios
            if (ascendente)
                query = query.OrderBy(x => x.CantidadComentarios);
            else
                query = query.OrderByDescending(x => x.CantidadComentarios);

            var lugares = await query.ToListAsync();

            return lugares;
        }

        public async Task<IEnumerable<LugarMapaResponseDto>> GetTodosLosLugares()
        {
            return await _context.Lugares
                .Where(l => l.Coordenadas!= null)
                .Select(l => new LugarMapaResponseDto
                {
                    IdLugar = l.IdLugar,
                    NombreLugar = l.NombreLugar,
                    // lo que vamos a hacer se conoce como 'cast'
                    Latitud = ((Point)l.Coordenadas!).Y, //con esto ya NetTopology entiende que es un tipo de dato Geometry.Punto
                    Longitud = ((Point)l.Coordenadas!).X, // y no un Geometry.Poligono, es decir, sabe que es un dato espacial (SRID,lat, long)
                    FotoUrl = l.FotoUrl
                }).ToListAsync();
        }
    }
}