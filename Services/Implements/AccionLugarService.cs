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

        public async Task<IEnumerable<AccionUsuarioResponseDto>> GetAccionesUsuario(int idUsuario, string? tipoAccion, bool ordenarPorLugar = false)
        {
            var query = _context.LugaresAcciones // Cambia a _context.AccionLugar si renombraste el DbSet
                .Where(a => a.IdUsuario == idUsuario)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tipoAccion))
            {
                query = query.Where(a => a.TipoAccion == tipoAccion);
            }

            if (ordenarPorLugar)
                query = query.OrderBy(a => a.Lugar!.NombreLugar);
            else
                query = query.OrderByDescending(a => a.FechaAccion);

            // Mapeamos fuertemente a nuestro nuevo DTO de salida
            var resultados = await query.Select(a => new AccionUsuarioResponseDto
            {
                IdAccion = a.IdAccion,
                TipoAccion = a.TipoAccion,
                FechaAccion = a.FechaAccion,
                NombreLugar = a.Lugar!.NombreLugar,
                IdLugar = a.IdLugar, // Usamos la nueva PK de tu BD
                GooglePlaceId = a.Lugar.GooglePlaceId
            }).ToListAsync();

            return resultados;
        }

        public async Task<(bool Exito, string Mensaje, object? Datos)> ToggleAccion(AccionLugarDto dto)
        {
            var accionesValidas = new[] { "Favorito", "Quiero ir", "Ya visité" };
            if (!accionesValidas.Contains(dto.TipoAccion))
            {
                return (false, $"Acción no válida. Debe ser una de: {string.Join(", ", accionesValidas)}", null);
            }

            // Validar que el Lugar interno exista antes de intentar agregar una acción
            var lugarExiste = await _context.Lugares.AnyAsync(l => l.IdLugar == dto.IdLugar);
            if (!lugarExiste)
            {
                return (false, "El lugar especificado no existe en la base de datos.", null);
            }

            // Buscamos usando la nueva relación de llave foránea (IdLugar)
            var accionExistente = await _context.LugaresAcciones
                .FirstOrDefaultAsync(a => a.IdUsuario == dto.IdUsuario
                                       && a.IdLugar == dto.IdLugar
                                       && a.TipoAccion == dto.TipoAccion);

            if (accionExistente != null)
            {
                _context.LugaresAcciones.Remove(accionExistente);
                await _context.SaveChangesAsync();

                return (true, "Acción removida de la lista.", new { Estado = "Eliminado" });
            }
            else
            {
                var nuevaAccion = new AccionLugar
                {
                    IdUsuario = dto.IdUsuario,
                    IdLugar = dto.IdLugar, // Asignamos el ID interno
                    TipoAccion = dto.TipoAccion,
                    // No hace falta poner DateTime.Now si en OnModelCreating pusimos GetDate(), pero no hace daño dejarlo.
                    FechaAccion = DateTime.Now
                };

                _context.LugaresAcciones.Add(nuevaAccion);
                await _context.SaveChangesAsync();

                return (true, "Acción registrada exitosamente.", new { Estado = "Agregado" });
            }
        }
    }
}