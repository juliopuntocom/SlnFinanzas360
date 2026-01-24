using Dapper;
using PrjFinanzas360.Data;
using PrjFinanzas360.DTOs;
using System.Data;

namespace PrjFinanzas360.Services
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaComboDto>> ListarComboAsync(string idUsuario);
    }

    public class CategoriaService : ICategoriaService
    {
        private readonly DapperContext _context;

        public CategoriaService(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoriaComboDto>> ListarComboAsync(string idUsuario)
        {
            using var cn = _context.CreateConnection();

            return await cn.QueryAsync<CategoriaComboDto>(
                "SP_LISTAR_CATEGORIAS_COMBOS",
                new { ID_USUARIO = idUsuario },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
