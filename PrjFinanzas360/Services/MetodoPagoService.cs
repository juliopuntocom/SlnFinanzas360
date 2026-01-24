using Dapper;
using PrjFinanzas360.Data;
using PrjFinanzas360.DTOs;
using System.Data;

namespace PrjFinanzas360.Services
{
    public class MetodoPagoService
    {
        private readonly DapperContext _context;

        public MetodoPagoService(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MetodoPagoComboDto>> ListarComboAsync(string idUsuario)
        {
            using var connection = await _context.CreateConnectionAsync(idUsuario);

            var result = await connection.QueryAsync<MetodoPagoComboDto>(
                "SP_LISTAR_METODOS_P_COMBO",
                new { ID_USUARIO = idUsuario },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
    }
}
