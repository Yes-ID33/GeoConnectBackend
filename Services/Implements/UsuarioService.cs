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

        public async Task<IEnumerable<object>> ListarUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new
                {
                    u.IdUsuario,
                    u.Nombre,
                    u.Correo,
                    u.Verificado,
                    u.FechaCreacion
                    // NO incluimos la contraseña aquí. Tampoco devolvemos las listas completas de comentarios/acciones para evitar ciclos infinitos.
                })
                .ToListAsync();

            return usuarios; // Ya no usamos Ok()
        }

        public async Task<object> CrearUsuario(Usuario newUser)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Correo == newUser.Correo);
            if (existe) return new { Error="El correo ya está registrado."};

            // Por defecto, lo marcamos como no verificado hasta que confirme su correo (si aplicas esa lógica después)
            newUser.Verificado = false;

            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync();

            // Devolvemos el objeto anónimo directamente
            return new
            {
                newUser.IdUsuario,
                newUser.Nombre,
                newUser.Correo
            };
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarUsuario(int id, Usuario datosNuevos)
        {
            var usuarioBd = await _context.Usuarios.FindAsync(id);
            if (usuarioBd == null) return (false, "Usuario no encontrado.");

            if (usuarioBd.Correo != datosNuevos.Correo)
            {
                var correoOcupado = await _context.Usuarios.AnyAsync(u => u.Correo == datosNuevos.Correo);
                if (correoOcupado) return ( false, "El nuevo correo ya está en uso.");
            }

            usuarioBd.Nombre = datosNuevos.Nombre;
            usuarioBd.Correo = datosNuevos.Correo;
            usuarioBd.Contrasena = datosNuevos.Contrasena;
            // OJO: Deberías hacer DTOs para esto. Aquí confías en que el frontend no envíe datos basura.

            await _context.SaveChangesAsync();
            return (true, "Perfil actualizado");
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

            // 1. Verificamos si existe
            if (usuarioBd == null)
                return (false, "Usuario no encontrado.");

            // 2. Verificamos si realmente estaba desactivado (usando la lógica de tu Delete)
            if (usuarioBd.Verificado == true || !usuarioBd.Correo.StartsWith("eliminado_"))
                return (false, "La cuenta ya se encuentra activa o no fue desactivada correctamente.");

            // 3. Verificamos que el nuevo correo no esté repetido
            var correoOcupado = await _context.Usuarios.AnyAsync(u => u.Correo == nuevoCorreo);
            if (correoOcupado) return (false, "Este correo ya está en uso.");

            // 4. Reactivamos
            usuarioBd.Nombre = nuevoNombre;
            usuarioBd.Correo = nuevoCorreo; // Necesita un correo real nuevamente
            usuarioBd.Verificado = true; // O false si quieres que vuelva a confirmar el correo

            await _context.SaveChangesAsync();
            return (true, "Cuenta reactivada correctamente. Tus comentarios volverán a tener tu nombre.");
        }
    }
}