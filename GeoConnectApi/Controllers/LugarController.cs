using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace GeoConnectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LugarController : ControllerBase
    {
        private readonly ILugarService _lugarService;

        public LugarController(ILugarService lugarService)
        {
            _lugarService = lugarService;
        }

        /// <summary>
        /// Obtiene los lugares dentro de un radio específico (en metros) desde la ubicación del usuario.
        /// </summary>
        /// <param name="lat">Latitud del usuario</param>
        /// <param name="lon">Longitud del usuario</param>
        /// <param name="radioEnMetros">Radio de búsqueda (ej. 5000 para 5km)</param>
        [HttpGet("cercanos")]
        public async Task<IActionResult> GetLugaresCercanos(double lat, double lon, double radioEnMetros)
        {
            var lugares = await _lugarService.GetLugaresCercanos(lat, lon, radioEnMetros);
            return Ok(lugares);
        }

        /// <summary>
        /// Obtiene la lista de lugares para mostrar en un mapa (Latitud/Longitud separadas).
        /// </summary>
        [HttpGet("mapa")]
        public async Task<IActionResult> GetTodosLosLugares()
        {
            var lugares = await _lugarService.GetTodosLosLugares();
            return Ok(lugares);
        }

        /// <summary>
        /// Obtiene los lugares ordenados por su popularidad (basado en cantidad de comentarios).
        /// </summary>
        /// <param name="ascendente">True para menos populares, False para más populares.</param>
        [HttpGet("populares")]
        public async Task<IActionResult> GetLugaresPopulares(bool ascendente = false)
        {
            var lugares = await _lugarService.GetLugaresPopulares(ascendente);
            return Ok(lugares);
        }
    }
}