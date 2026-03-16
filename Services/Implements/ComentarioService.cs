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

        public async Task<IEnumerable<object>> GetComentariosPorLugar(string googlePlaceId)
        {
            var comentarios = await _context.Comentarios
                .Where(c => c.GooglePlaceId == googlePlaceId)
                .OrderByDescending(c => c.FechaPublicacion)
                .Select(c => new
                {
                    c.IdComentario,
                    c.Comentario1,
                    c.FechaPublicacion,
                    // Magia aquí: Si el usuario es nulo o está inactivo, ocultamos su nombre
                    Autor = c.Usuario == null || c.Usuario.Verificado == false
                            ? "Usuario Eliminado" //operador ternario breve
                            : c.Usuario.Nombre
                }).ToListAsync();

            return comentarios;
        }

        public async Task<IEnumerable<object>> GetComentariosPorUsuario(int IdUsuario)
        {
            var comentarios = await _context.Comentarios
                .Where(c => c.IdUsuario == IdUsuario)
                .OrderByDescending(c => c.FechaPublicacion)
                .Select(c => new
                {
                    c.IdComentario,
                    c.Comentario1,
                    c.FechaPublicacion
                }).ToListAsync();
            return comentarios;
        }

        public async Task<(bool Exito, string Mensaje, object? Datos)> CrearComentario(CrearComentarioDto dto)
        {
            // Validamos que el usuario y el lugar existan (opcional pero recomendado)
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == dto.IdUsuario);
            if (!usuarioExiste) return (false, "El usuario no existe.", null);

            var lugarExiste = await _context.Lugares.AnyAsync(l => l.GooglePlaceId == dto.GooglePlaceId);
            if (!lugarExiste) return (false, "El lugar no existe en la base de datos.", null);

            // Creamos la entidad real a partir del DTO
            var nuevoComentario = new Comentario
            {
                IdUsuario = dto.IdUsuario,
                GooglePlaceId = dto.GooglePlaceId,
                Comentario1 = dto.TextoComentario,
                FechaPublicacion = DateTime.Now
            };

            _context.Comentarios.Add(nuevoComentario);
            await _context.SaveChangesAsync();

            // Devolvemos un 201 Created con los datos básicos
            return (true, "Comentario publicado exitosamente", new
            {
                nuevoComentario.IdComentario,
                nuevoComentario.Comentario1
            });
        }
    }
}