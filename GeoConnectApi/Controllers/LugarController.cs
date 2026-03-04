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
        /// Filtra los lugares turísticos según la distancia que tenga con el usuario
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="radioEnMetros"></param>
        /// <returns></returns>
        [HttpGet("cercanos")]
        public async Task<IActionResult> GetLugaresCercanos(double lat, double lon, double radioEnMetros)
        {
            var lugares = await _lugarService.GetLugaresCercanos(lat, lon, radioEnMetros);
            return Ok(lugares);
        }

        /// <summary>
        /// Filtra los lugares y los ordena según la cantidad de comentarios que tenga cada uno
        /// </summary>
        /// <param name="ascendente"></param>
        /// <returns></returns>
        [HttpGet("populares")]
        public async Task<IActionResult> GetLugaresPopulares(bool ascendente = false)
        {
            var lugares = await _lugarService.GetLugaresPopulares(ascendente);
            return Ok(lugares);
        }
    }
}