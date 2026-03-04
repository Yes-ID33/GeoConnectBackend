using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace GeoConnectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MunicipioController : ControllerBase
    {
        private readonly IMunicipioService _municipioService;

        public MunicipioController(IMunicipioService municipioService)
        {
            _municipioService = municipioService;
        }

        /// <summary>
        /// Obtener los datos según los municipios cuyos lugares tengan más comentarios o simplemente
        /// los municipios que tengan más lugares turísticos
        /// </summary>
        /// <param name="porComentarios"></param>
        /// <param name="ascendente"></param>
        /// <returns></returns>
        [HttpGet("estadisticas")]
        public async Task<IActionResult> GetEstadisticasMunicipios(bool porComentarios = true, bool ascendente = false)
        {
            var resultados = await _municipioService.GetEstadisticasMunicipios(porComentarios, ascendente);
            return Ok(resultados);
        }
    }
}