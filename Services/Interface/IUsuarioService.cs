using Models;

namespace Services.Interface
{
    public interface IUsuarioService
    {
        Task<IEnumerable<object>> ListarUsuarios();
        Task<object> CrearUsuario(Usuario newUser);
        Task<(bool Exito, string Mensaje)> ActualizarUsuario(int id, Usuario datosNuevos);
        Task<(bool Exito, string Mensaje)> DesactivarCuenta(int id);
        Task<(bool Exito, string Mensaje)> ReactivarCuenta(int id, string nuevoCorreo, string nuevoNombre);
    }
}