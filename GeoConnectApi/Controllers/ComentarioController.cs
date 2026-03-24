using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace GeoConnectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentarioController : ControllerBase
    {
        private readonly IComentarioService _comentarioService;

        public ComentarioController(IComentarioService comentarioService)
        {
            _comentarioService = comentarioService;
        }

        /// <summary>
        /// Método get para obtener todos los comentarios de un lugar y si un usuario desactiva la cuenta
        /// que lo que hace en realidad es poner en false el verificado, se censura el nombre de quien publicó
        /// el comentario, pero el contenido se queda
        /// </summary>
        /// <param name="IdLugar"></param> ID interno de cada lugar en nuestra DB
        /// <returns></returns>
        [HttpGet("por-lugar/{googlePlaceId}")]
        public async Task<IActionResult> GetComentariosPorLugar(int IdLugar)
        {
            var comentarios = await _comentarioService.GetComentariosPorLugar(IdLugar);
            return Ok(comentarios);
        }

        /// <summary>
        /// Método para obtener todos los comentarios hechos por un usuario
        /// ordenados de forma descendente según fecha de publicación.
        /// </summary>
        /// <param name="IdUsuario"></param>
        /// <returns></returns>
        [HttpGet("por-usuario/{IdUsuario}")]
        public async Task<IActionResult> GetComentariosPorUsuario(int IdUsuario)
        {
            var comentarios = await _comentarioService.GetComentariosPorUsuario(IdUsuario);
            return Ok(comentarios);
        }

        /// <summary>
        /// Método para crear un nuevo comentario en un lugar.
        /// </summary>
        [HttpPost("crear")]
        public async Task<IActionResult> CrearComentario([FromBody] CrearComentarioDto dto)
        {
            var resultado = await _comentarioService.CrearComentario(dto);

            if (!resultado.Exito)
                return BadRequest(new { Mensaje = resultado.Mensaje });

            return StatusCode(StatusCodes.Status201Created, new { Mensaje = resultado.Mensaje, Datos = resultado.Datos });
        }
    }
}