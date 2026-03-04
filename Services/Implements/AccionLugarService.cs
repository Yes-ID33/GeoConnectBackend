using Microsoft.EntityFrameworkCore;
using Models;
using Services.Interface;

namespace Services.Implements
{
    public class AccionLugarService : IAccionLugarService
    {
        private readonly GeoConnectContext _context;

        public AccionLugarService(GeoConnectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetAccionesUsuario(int idUsuario, string? tipoAccion, bool ordenarPorLugar = false)
        {
            var query = _context.LugaresAcciones
                .Where(a => a.IdUsuario == idUsuario)
                .AsQueryable();

            // Filtro dinámico: Si envían "Favorito", "Quiero ir", etc., filtramos.
            if (!string.IsNullOrEmpty(tipoAccion))
            {
                query = query.Where(a => a.TipoAccion == tipoAccion);
            }

            // Ordenamiento dinámico
            if (ordenarPorLugar)
                query = query.OrderBy(a => a.GooglePlace!.NombreLugar);
            else
                query = query.OrderByDescending(a => a.FechaAccion);

            var resultados = await query.Select(a => new {
                a.IdAccion,
                a.TipoAccion,
                a.FechaAccion,
                Lugar = a.GooglePlace!.NombreLugar,
                IdLugar = a.GooglePlaceId
            }).ToListAsync();

            return resultados;
        }

        public async Task<(bool Exito, string Mensaje, object? Datos)> ToggleAccion(AccionLugarDto dto)
        {
            // 1. Validar que los tipos de acción coincidan exactamente con lo que esperas
            var accionesValidas = new[] { "Favorito", "Quiero ir", "Ya visité" }; 
            if (!accionesValidas.Contains(dto.TipoAccion))
            {
                return (false ,$"Acción no válida. Debe ser una de: {string.Join(", ", accionesValidas)}", null);
            }

            // 2. Buscamos si el usuario ya tiene esta acción registrada para este lugar
            var accionExistente = await _context.LugaresAcciones
                .FirstOrDefaultAsync(a => a.IdUsuario == dto.IdUsuario
                                       && a.GooglePlaceId == dto.GooglePlaceId
                                       && a.TipoAccion == dto.TipoAccion);

            // 3. Lógica del Toggle
            if (accionExistente != null)
            {
                // Si YA EXISTE (Ícono de color) -> Lo eliminamos (Volver a gris)
                _context.LugaresAcciones.Remove(accionExistente);
                await _context.SaveChangesAsync();

                // Devolvemos un estado claro para que el Front sepa qué hacer
                return (true, "Acción removida de la lista.", new { Estado = "Eliminado" });
            }
            else
            {
                // Si NO EXISTE (Ícono gris) -> Lo creamos (Pintar de color)
                var nuevaAccion = new AccionLugar 
                {
                    IdUsuario = dto.IdUsuario,
                    GooglePlaceId = dto.GooglePlaceId, //solo funciona con lugares ya existentes, si aparece un lugar desde 
                    // google maps que no está guardado en la base de datos, va a tirar error
                    TipoAccion = dto.TipoAccion,
                    FechaAccion = DateTime.Now
                };

                _context.LugaresAcciones.Add(nuevaAccion);
                await _context.SaveChangesAsync();

                return (true, "Acción registrada exitosamente.", new { Estado = "Agregado" });
            }
        }
    }
}