using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace GeoConnectApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccionLugarController : ControllerBase
    {
        private readonly IAccionLugarService _accionLugarService;

        public AccionLugarController(IAccionLugarService accionLugarService)
        {
            _accionLugarService = accionLugarService;
        }

        /// <summary>
        /// Método para obtener todos lugares con acciones asignadas de un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="tipoAccion"></param>
        /// <param name="ordenarPorLugar"></param>
        /// <returns></returns>
        [HttpGet("mis-acciones/{idUsuario}")]
        public async Task<IActionResult> GetAccionesUsuario(int idUsuario, [FromQuery] string? tipoAccion, [FromQuery] bool ordenarPorLugar = false)
        {
            var resultados = await _accionLugarService.GetAccionesUsuario(idUsuario, tipoAccion, ordenarPorLugar);
            return Ok(resultados);
        }

        /// <summary>
        /// Método para alternar (Toggle) una acción. 
        /// Si no existe, la crea. Si ya existe, la elimina.
        /// Ideal para botones de Favoritos/Quiero ir/Visitado.
        /// </summary>
        [HttpPost("toggle-accion")]
        public async Task<IActionResult> ToggleAccion([FromBody] AccionLugarDto dto)
        {
            var resultado = await _accionLugarService.ToggleAccion(dto);

            if (!resultado.Exito)
                return BadRequest(resultado.Mensaje);

            return Ok(new { Mensaje = resultado.Mensaje, Datos = resultado.Datos });
        }
    }
}