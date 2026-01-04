using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjFinanzas360.Services;
using System.Security.Claims;

namespace PrjFinanzas360.Controllers
{
    [ApiController]
    [Route("v1/menu")]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly MenuService _menuService;

        public MenuController(MenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet("preview-profile")]
        public async Task<IActionResult> GetProfilePreview()
        {
            var idUsuario =
                User.FindFirst("uid")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(idUsuario))
                return Unauthorized();

            var profile = await _menuService.ObtenerPreviewAsync(idUsuario);

            if (profile == null)
                return NotFound();

            return Ok(profile);
        }
    }
}
