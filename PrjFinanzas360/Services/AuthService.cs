using Dapper;
using PrjFinanzas360.Data;
using PrjFinanzas360.DTOs;
using PrjFinanzas360.Security;
using BCrypt.Net;
using System.Data;

namespace PrjFinanzas360.Services
{
    public class AuthService
    {
        private readonly DapperContext _context;
        private readonly JwtTokenService _jwtService;

        public AuthService(DapperContext context, JwtTokenService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto?> LoginAsync(
            string email,
            string password
        )
        {
            using var connection = _context.CreateConnection();

            var user = await connection.QueryFirstOrDefaultAsync(
                "SP_LOGIN_USUARIO",
                new { EMAIL = email },
                commandType: CommandType.StoredProcedure
            );

            if (user == null)
                return null;

            bool passwordOk = BCrypt.Net.BCrypt.Verify(password, user.PWD);
            if (!passwordOk)
                return null;

            // 🔐 Generar token
            var jwt = _jwtService.GenerateToken(
                user.ID_USUARIO,
                user.EMAIL,
                user.NOMBRES,
                user.APE_PATERNO,
                user.APE_MATERNO
            );

            return new LoginResponseDto
            {
                Token = jwt.Token,
                Mensaje = "Login exitoso",
                ExpiraEn = jwt.Expires
            };


        }

        public LoginResponseDto GenerarTokenApiPublica()
        {
            var jwt = _jwtService.GenerateToken(
                "USR0001",
                "api@finanzas360.com",
                "API",
                "PUBLICA",
                ""
            );

            return new LoginResponseDto
            {
                Token = jwt.Token,
                Mensaje = "Token API generado correctamente",
                ExpiraEn = jwt.Expires
            };
        }
    }
}
