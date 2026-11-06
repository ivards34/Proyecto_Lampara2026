using Microsoft.AspNetCore.Mvc;
using Negocio.Lampara;
using System.Text.Json;

namespace Presentacion.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LamparaController : ControllerBase
    {
        private readonly ILamparaService _lamparaService;

        public LamparaController(ILamparaService lamparaService)
        {
            _lamparaService = lamparaService;
        }

        [HttpPost]
        public async Task<IActionResult> RecibirDatosLampara([FromBody] JsonElement json)
        {
            try
            {
                string jsonString = json.GetRawText();
                await _lamparaService.ProcesarDatosLampara(jsonString);
                return Ok("Datos guardados correctamente");
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error al procesar los datos: {ex.Message}");
            }
        }

        [HttpGet("consultar")]
        public async Task<IActionResult> ConsultarESP32()
        {
            try
            {
                await _lamparaService.ConsultarESP32Async();
                return Ok("Consulta al ESP32 realizada y datos guardados");
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error al consultar ESP32: {ex.Message}");
            }
        }

        [HttpPost("modo")]
        public async Task<IActionResult> CambiarModoLampara([FromBody] ModoRequest request)
        {
            try
            {
                await _lamparaService.CambiarModoLamparaAsync(request.ModoId);
                return Ok($"Modo de lámpara cambiado a {request.ModoId}");
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error al cambiar modo: {ex.Message}");
            }
        }

        [HttpGet("lecturas")]
        public async Task<IActionResult> ObtenerTodasLecturas()
        {
            try
            {
                var datos = await _lamparaService.ObtenerTodasLecturas();
                return Ok(datos);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error al obtener lecturas: {ex.Message}");
            }
        }

        [HttpGet("actual")]
        public async Task<IActionResult> ObtenerLecturaActual()
        {
            try
            {
                var dato = await _lamparaService.ObtenerUltimaLectura();
                if (dato == null)
                {
                    return NotFound("No hay lecturas disponibles");
                }
                return Ok(dato);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error al obtener lectura actual: {ex.Message}");
            }
        }
    }

    public class ModoRequest
    {
        public int ModoId { get; set; }
    }
}
