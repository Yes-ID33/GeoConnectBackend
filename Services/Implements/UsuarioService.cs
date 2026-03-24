using Microsoft.EntityFrameworkCore;
using Models;
using Services.Interface;

namespace Services.Implements
{
    public class UsuarioService : IUsuarioService
    {
        private readonly GeoConnectContext _context;

        public UsuarioService(GeoConnectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuarioResponseDto>> ListarUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new UsuarioResponseDto
                {
                    IdUsuario = u.IdUsuario,
                    Nombre = u.Nombre,
                    Correo = u.Correo,
                    Verificado = u.Verificado ?? false,
                    FechaCreacion = u.FechaCreacion ?? DateTime.MinValue
                    // La contraseña no viaja al frontend, está protegida por el DTO
                })
                .ToListAsync();

            return usuarios;
        }

        public async Task<(bool Exito, string Mensaje, UsuarioResponseDto? Datos)> CrearUsuario(CrearUsuarioDto dto)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo);
            if (existe)
                return (false, "El correo ya está registrado.", null);

            var newUser = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Contrasena = dto.Contrasena, // Nota: En un entorno real, aquí deberías hashear la contraseña
                Verificado = false,
                FechaCreacion = DateTime.Now
            };

            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync();

            // Mapeamos a la respuesta segura
            var respuesta = new UsuarioResponseDto
            {
                IdUsuario = newUser.IdUsuario,
                Nombre = newUser.Nombre,
                Correo = newUser.Correo,
                Verificado = newUser.Verificado ?? false,
                FechaCreacion = newUser.FechaCreacion ?? DateTime.Now
            };

            return (true, "Usuario creado exitosamente.", respuesta);
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarUsuario(int id, ActualizarUsuarioDto dto)
        {
            var usuarioBd = await _context.Usuarios.FindAsync(id);
            if (usuarioBd == null) return (false, "Usuario no encontrado.");

            if (usuarioBd.Correo != dto.Correo)
            {
                var correoOcupado = await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo);
                if (correoOcupado) return (false, "El nuevo correo ya está en uso.");
            }

            usuarioBd.Nombre = dto.Nombre;
            usuarioBd.Correo = dto.Correo;
            usuarioBd.Contrasena = dto.Contrasena;

            await _context.SaveChangesAsync();
            return (true, "Perfil actualizado correctamente.");
        }

        public async Task<(bool Exito, string Mensaje)> DesactivarCuenta(int id)
        {
            var usuarioBd = await _context.Usuarios.FindAsync(id);
            if (usuarioBd == null) return (false, "Usuario no encontrado.");

            usuarioBd.Verificado = false;
            usuarioBd.Nombre = "Cuenta Eliminada";
            usuarioBd.Correo = $"eliminado_{id}@geoconnect.com";

            await _context.SaveChangesAsync();
            return (true, "Cuenta desactivada correctamente. Tus comentarios permanecerán anónimos.");
        }

        public async Task<(bool Exito, string Mensaje)> ReactivarCuenta(int id, string nuevoCorreo, string nuevoNombre)
        {
            var usuarioBd = await _context.Usuarios.FindAsync(id);

            if (usuarioBd == null)
                return (false, "Usuario no encontrado.");

            if (usuarioBd.Verificado == true || !usuarioBd.Correo.StartsWith("eliminado_"))
                return (false, "La cuenta ya se encuentra activa o no fue desactivada correctamente.");

            var correoOcupado = await _context.Usuarios.AnyAsync(u => u.Correo == nuevoCorreo);
            if (correoOcupado) return (false, "Este correo ya está en uso.");

            usuarioBd.Nombre = nuevoNombre;
            usuarioBd.Correo = nuevoCorreo;
            usuarioBd.Verificado = true;

            await _context.SaveChangesAsync();
            return (true, "Cuenta reactivada correctamente. Tus comentarios volverán a tener tu nombre.");
        }
    }
}