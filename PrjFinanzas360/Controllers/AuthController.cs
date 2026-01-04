using Microsoft.AspNetCore.Mvc;
using PrjFinanzas360.DTOs;
using PrjFinanzas360.Services;

namespace PrjFinanzas360.Controllers
{
    [ApiController]
    [Route("v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(
                request.Email,
                request.Password
            );

            if (response == null)
                return Unauthorized("Credenciales incorrectas");

            return Ok(response);
        }

    }
}
