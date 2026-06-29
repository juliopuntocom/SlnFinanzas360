using Dapper;
using PrjFinanzas360.Data;
using PrjFinanzas360.DTOs;
using System.Data;

namespace PrjFinanzas360.Services
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaComboDto>> ListarComboAsync(string idUsuario);
        Task<IEnumerable<CategoriaDto>> ListarAsync(string idUsuario);

        Task<bool> ActualizarAsync(string idUsuario, CategoriaUpdateDto dto);

        Task<bool> ActualizarEstadoAsync(string idUsuario, CategoriaEstadoDto dto);

        Task<bool> CrearAsync(string idUsuario, CategoriaCreateDto dto);

        Task<IEnumerable<CategoriaPublicaDto>> ListarPublicoAsync();
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

        public async Task<IEnumerable<CategoriaDto>> ListarAsync(string idUsuario)
        {
            using var cn = _context.CreateConnection();

            return await cn.QueryAsync<CategoriaDto>(
                "SP_LISTA_CATEGORIAS",
                new { ID_USUARIO = idUsuario },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> ActualizarAsync(string idUsuario, CategoriaUpdateDto dto)
        {
            using var cn = await _context.CreateConnectionAsync(idUsuario);

            var result = await cn.ExecuteScalarAsync<int>(
                "SP_ACTUALIZAR_CATEGORIA",
                new
                {
                    ID_CATEGORIA = dto.IdCategoria,
                    NAME = dto.Name,
                    ICON = dto.Icon,
                    COLOR = dto.Color
                },
                commandType: CommandType.StoredProcedure
            );

            return result == 1;
        }

        public async Task<bool> ActualizarEstadoAsync(string idUsuario, CategoriaEstadoDto dto)
        {
            using var cn = await _context.CreateConnectionAsync(idUsuario);

            try
            {
                var result = await cn.ExecuteScalarAsync<int>(
                    "SP_ACTUALIZAR_ESTADO_CATEGORIA",
                    new { ID_CAT = dto.IdCategoria, ESTADO = dto.Estado },
                    commandType: CommandType.StoredProcedure
                );

                return result == 1;

            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CrearAsync(string idUsuario, CategoriaCreateDto dto)
        {
            using var cn = await _context.CreateConnectionAsync(idUsuario);

            try
            {
                var result = await cn.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_INSERTAR_CATEGORIA",
                    new
                    {
                        ID_USUARIO = idUsuario,
                        NAME = dto.Name,
                        ICON = dto.Icon,
                        COLOR = dto.Color
                    },
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                    return false;

                int success = (int)result.success;

                return success == 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<IEnumerable<CategoriaPublicaDto>> ListarPublicoAsync()
        {
            using var cn = _context.CreateConnection();

            return await cn.QueryAsync<CategoriaPublicaDto>(
                "SP_LISTAR_CATEGORIAS_PUBLICAS",
                commandType: CommandType.StoredProcedure
            );
        }


    }
}
