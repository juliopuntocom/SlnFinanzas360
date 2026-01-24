using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjFinanzas360.Services;
using System.Security.Claims;

namespace PrjFinanzas360.Controllers
{
    [ApiController]
    [Route("v1/metodo-pago")]
    [Authorize]
    public class MetodoPagoController : ControllerBase
    {
        private readonly MetodoPagoService _metodoPagoService;

        public MetodoPagoController(MetodoPagoService metodoPagoService)
        {
            _metodoPagoService = metodoPagoService;
        }

        [HttpGet("combos/listar")]
        public async Task<IActionResult> ListarCombos()
        {
            var idUsuario =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value ??
                User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(idUsuario))
                return Unauthorized();

            var data = await _metodoPagoService.ListarComboAsync(idUsuario);
            return Ok(data);
        }
    }
}
