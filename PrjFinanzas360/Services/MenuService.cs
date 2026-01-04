using Dapper;
using PrjFinanzas360.Data;
using PrjFinanzas360.DTOs;
using System.Data;

namespace PrjFinanzas360.Services
{
    public class MenuService
    {
        private readonly DapperContext _context;

        public MenuService(DapperContext context)
        {
            _context = context;
        }

        public async Task<MenuProfileDto?> ObtenerPreviewAsync(string idUsuario)
        {
            using var connection = await _context.CreateConnectionAsync(idUsuario);

            return await connection.QueryFirstOrDefaultAsync<MenuProfileDto>(
                "SP_PREVIEW_PROFILE",
                new { ID_USUARIO = idUsuario },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
