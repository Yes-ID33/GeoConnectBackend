using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Interface;

namespace GeoConnectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        // Inyectamos la Interfaz, NO el Contexto de la DB
        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Método para obtener todos los usuarios de forma segura (sin contraseñas).
        /// </summary>
        [HttpGet("GetUsers")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = await _usuarioService.ListarUsuarios();
            return Ok(usuarios);
        }

        /// <summary>
        /// Método para crear usuarios nuevos, ideal para Register.
        /// </summary>
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CrearUsuario([FromBody] Usuario newUser)
        {
            var usuarioCreado = await _usuarioService.CrearUsuario(newUser);
            return StatusCode(StatusCodes.Status201Created, usuarioCreado);
        }

        /// <summary>
        /// Actualizar algunos campos del usuario.
        /// </summary>
        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] Usuario datosNuevos)
        {
            var resultado = await _usuarioService.ActualizarUsuario(id, datosNuevos);

            if (!resultado.Exito) return NotFound(resultado.Mensaje);

            return Ok(new { Mensaje = resultado.Mensaje });
        }

        /// <summary>
        /// Método para desactivar una cuenta y convertir los comentarios en anónimos.
        /// </summary>
        [HttpDelete("DeactivateAccount/{id}")]
        public async Task<IActionResult> DesactivarCuenta(int id)
        {
            var resultado = await _usuarioService.DesactivarCuenta(id);

            if (!resultado.Exito) return NotFound(resultado.Mensaje);

            return Ok(new { Mensaje = resultado.Mensaje });
        }

        /// <summary>
        /// Método para reactivar una cuenta.
        /// </summary>
        [HttpPatch("ReactivateAccount/{id}")]
        public async Task<IActionResult> ReactivarCuenta(int id, string nuevoCorreo, string nuevoNombre)
        {
            var resultado = await _usuarioService.ReactivarCuenta(id, nuevoCorreo, nuevoNombre);

            // Si devuelve false, puede ser que no se encontró o que el estado era inválido, devolvemos un BadRequest por seguridad
            if (!resultado.Exito) return BadRequest(resultado.Mensaje);

            return Ok(new { Mensaje = resultado.Mensaje });
        }
    }
}