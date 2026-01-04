using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PrjFinanzas360.Security
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public JwtResult GenerateToken(
            string idUsuario,
            string email,
            string nombres,
            string apePaterno,
            string apeMaterno
        )
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, idUsuario),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("uid", idUsuario),

                new Claim("nombres", nombres),
                new Claim("ape_paterno", apePaterno),
                new Claim("ape_materno", apeMaterno),

                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256
            );

            var expires = DateTime.UtcNow.AddHours(12);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = expires
            };
        }
    }
}
