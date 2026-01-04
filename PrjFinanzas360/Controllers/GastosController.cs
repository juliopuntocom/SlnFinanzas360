using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrjFinanzas360.DTOs;
using PrjFinanzas360.Services;
using System.Security.Claims;

namespace PrjFinanzas360.Controllers
{
    [ApiController]
    [Route("v1/gastos")]
    [Authorize]
    public class GastosController : ControllerBase
    {
        private readonly GastoService _gastoService;

        public GastosController(GastoService gastoService)
        {
            _gastoService = gastoService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarGasto([FromBody] RegistrarGastoDto request)
        {
            var idUsuario =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value ??
                User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(idUsuario))
                return Unauthorized();

            var idGasto = await _gastoService.RegistrarGastoAsync(
                idUsuario,
                request
            );

            return Ok(new
            {
                idGasto,
                mensaje = "Gasto registrado correctamente"
            });
        }


        [HttpGet("mis-gastos")]
        public async Task<IActionResult> GetGastos(
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta,
            [FromQuery] string? idCategoria,
            [FromQuery] string? idMetodo,
            [FromQuery] string? descripcion
        )
        {
            var idUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value
                            ?? User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(idUsuario))
                return Unauthorized();

            var gastos = await _gastoService.ObtenerGastosAsync(
                idUsuario,
                fechaDesde,
                fechaHasta,
                idCategoria,
                idMetodo,
                descripcion
            );

            return Ok(gastos);
        }

        [HttpGet("{idGasto}/detalle")]
        public async Task<IActionResult> GetGastoDetalle(string idGasto)
        {
            var idUsuario =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value ??
                User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(idUsuario))
                return Unauthorized();

            var detalle = await _gastoService.ObtenerGastoDetalleAsync(idGasto, idUsuario);

            if (detalle == null)
                return NotFound();

            return Ok(detalle);
        }


    }
}
