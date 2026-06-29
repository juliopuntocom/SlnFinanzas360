using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjFinanzas360.DTOs;
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
        private readonly IConfiguration _configuration;

        public CategoriaController(ICategoriaService categoriaService, IConfiguration configuration )
        {
            _categoriaService = categoriaService;
            _configuration = configuration;
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

        [HttpGet("listar")]
        public async Task<IActionResult> ListarCategorias()
        {
            var idUsuario =
                User.FindFirst("uid")?.Value ??
                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(idUsuario))
                return Unauthorized();

            var lista = await _categoriaService.ListarAsync(idUsuario);

            return Ok(lista);
        }

        [HttpPut("actualizar")]
        public async Task<IActionResult> ActualizarCategoria(CategoriaUpdateDto dto)
        {
            var idUsuario =
                User.FindFirst("uid")?.Value ??
                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(idUsuario))
                return Unauthorized();

            var actualizado = await _categoriaService.ActualizarAsync(idUsuario, dto);

            if (actualizado == false)
                return BadRequest(new {actualizado = false});

            return Ok(new { actualizado = true });
        }

        [HttpPut("actualizar-estado")]
        public async Task<IActionResult> ActualizarEstadoCategoria(CategoriaEstadoDto dto)
        {
            var idUsuario =
                User.FindFirst("uid")?.Value ?? 
                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(idUsuario))
                return Unauthorized();

            var actualizado = await _categoriaService.ActualizarEstadoAsync(idUsuario, dto);

            if (!actualizado)
                return BadRequest(new { actualizado = false, mensaje = "No se pudo actualizar el estado." });

            return Ok(new { actualizado = true });
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearCategoria(CategoriaCreateDto dto)
        {
            var idUsuario =
                User.FindFirst("uid")?.Value ??
                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(idUsuario))
                return Unauthorized();

            var creado = await _categoriaService.CrearAsync(idUsuario, dto);

            if (!creado)
                return BadRequest(new { creado = false, mensaje = "No se pudo crear la categoría." });

            return Ok(new
            {
                creado = true,
                mensaje = "Categoría creada correctamente."
            });
        }

        [HttpGet("publico/listar")]
        [AllowAnonymous]
        public async Task<IActionResult> ListarCategoriasPublicas()
        {
            var tokenEsperado = _configuration["ApiPublica:Token"];

            var authorization =
                Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authorization))
                return Unauthorized();

            var tokenRecibido = authorization.Replace("Bearer ", "");

            if (tokenRecibido != tokenEsperado)
                return Unauthorized();

            var lista = await _categoriaService.ListarPublicoAsync();

            return Ok(lista);
        }


    }
}
