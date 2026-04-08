using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace GeoConnectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Inyectamos el servicio de autenticación que acabamos de crear
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Método para autenticar un usuario y generar su token JWT de acceso.
        /// </summary>
        /// <param name="dto">Credenciales del usuario (Correo y Contraseña).</param>
        /// <returns>Mensaje de éxito y el Token JWT si las credenciales son válidas.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var resultado = await _authService.LoginAsync(dto);

            // Si falla (correo no existe o contraseña incorrecta), devolvemos 401 Unauthorized
            if (!resultado.Exito)
            {
                return Unauthorized(new { Mensaje = resultado.Mensaje });
            }

            // Si todo sale bien, devolvemos 200 OK con el Token empaquetado
            return Ok(new
            {
                Mensaje = resultado.Mensaje,
                Token = resultado.Token
            });
        }

        /// <summary>
        /// Método para verificar la cuenta usando el código enviado al correo.
        /// </summary>
        [HttpPost("verificar")]
        public async Task<IActionResult> VerificarCuenta([FromBody] VerificarCuentaDto dto)
        {
            var resultado = await _authService.VerificarCuentaAsync(dto);

            if (!resultado.Exito)
                return BadRequest(new { Mensaje = resultado.Mensaje });

            return Ok(new 
            { 
                Mensaje = resultado.Mensaje,
                Token = resultado.Token
            });
        }
    }
}