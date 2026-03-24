using Microsoft.EntityFrameworkCore;
using Models;
using Services.Interface;

namespace Services.Implements
{
    public class ComentarioService : IComentarioService
    {
        private readonly GeoConnectContext _context;

        public ComentarioService(GeoConnectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ComentarioLugarResponseDto>> GetComentariosPorLugar(int idLugar)
        {
            var comentarios = await _context.Comentarios
                .Where(c => c.IdLugar == idLugar) // Filtramos por la nueva llave foránea
                .OrderByDescending(c => c.FechaPublicacion)
                .Select(c => new ComentarioLugarResponseDto
                {
                    IdComentario = c.IdComentario,
                    Comentario = c.Comentario1,
                    Calificacion = c.Calificacion, // Mapeamos calificación
                    FechaPublicacion = c.FechaPublicacion,
                    Autor = c.Usuario == null || c.Usuario.Verificado == false
                            ? "Usuario Eliminado"
                            : c.Usuario.Nombre
                }).ToListAsync();

            return comentarios;
        }

        public async Task<IEnumerable<ComentarioUsuarioResponseDto>> GetComentariosPorUsuario(int idUsuario)
        {
            var comentarios = await _context.Comentarios
                .Where(c => c.IdUsuario == idUsuario)
                .OrderByDescending(c => c.FechaPublicacion)
                .Select(c => new ComentarioUsuarioResponseDto
                {
                    IdComentario = c.IdComentario,
                    Comentario = c.Comentario1,
                    Calificacion = c.Calificacion, // Mapeamos calificación
                    FechaPublicacion = c.FechaPublicacion,
                    IdLugar = c.IdLugar,
                    NombreLugar = c.Lugar!.NombreLugar // Agregamos el nombre del lugar para el Front
                }).ToListAsync();

            return comentarios;
        }

        public async Task<(bool Exito, string Mensaje, object? Datos)> CrearComentario(CrearComentarioDto dto)
        {
            // 1. Validamos la calificación por código para dar un mensaje claro
            if (dto.Calificacion < 1 || dto.Calificacion > 5)
            {
                return (false, "La calificación debe ser un valor entre 1 y 5 estrellas.", null);
            }

            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == dto.IdUsuario);
            if (!usuarioExiste) return (false, "El usuario no existe.", null);

            // 2. Validamos contra el ID interno, no el de Google
            var lugarExiste = await _context.Lugares.AnyAsync(l => l.IdLugar == dto.IdLugar);
            if (!lugarExiste) return (false, "El lugar no existe en la base de datos.", null);

            var nuevoComentario = new Comentario
            {
                IdUsuario = dto.IdUsuario,
                IdLugar = dto.IdLugar, // Asignamos ID interno
                Comentario1 = dto.TextoComentario,
                Calificacion = dto.Calificacion, // Pasamos la calificación
                FechaPublicacion = DateTime.Now
            };

            _context.Comentarios.Add(nuevoComentario);
            await _context.SaveChangesAsync();

            return (true, "Comentario publicado exitosamente", new
            {
                nuevoComentario.IdComentario,
                nuevoComentario.Comentario1,
                nuevoComentario.Calificacion
            });
        }
    }
}