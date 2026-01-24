using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjFinanzas360.Services;
using System.IdentityModel.Tokens.Jwt;

namespace PrjFinanzas360.Controllers
{
    [ApiController]
    [Route("v1/categoria")]
    [Authorize]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet("combos/listar")]
        public async Task<IActionResult> ListarCategoriasCombo()
        {
            var idUsuario =
                User.FindFirst("uid")?.Value ??
                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(idUsuario))
                return Unauthorized();

            var lista = await _categoriaService.ListarComboAsync(idUsuario);

            return Ok(lista);
        }
    }
}
