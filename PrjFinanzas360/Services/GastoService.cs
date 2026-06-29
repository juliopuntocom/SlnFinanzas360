using Dapper;
using PrjFinanzas360.Data;
using PrjFinanzas360.DTOs;
using System.Data;
using System.Text.Json;

namespace PrjFinanzas360.Services
{
    public class GastoService
    {
        private readonly DapperContext _context;

        public GastoService(DapperContext context)
        {
            _context = context;
        }

        public async Task<string> RegistrarGastoAsync(
        string idUsuario,
        RegistrarGastoDto request)
        {
            var detalleJson = request.Detalle != null && request.Detalle.Any()
                ? JsonSerializer.Serialize(
                    request.Detalle.Select(d => new
                    {
                        producto = d.Producto,
                        precio = d.Precio,
                        idCategoria = d.IdCategoria
                    }))
                : null;

            using var connection = await _context.CreateConnectionAsync(idUsuario);

            var parameters = new DynamicParameters();
            parameters.Add("@ID_USUARIO", idUsuario);
            parameters.Add("@ID_METODO", request.IdMetodo);
            parameters.Add("@FECHA", request.Fecha);
            parameters.Add("@MONTO", request.Monto);
            parameters.Add("@TIPO", request.Tipo);
            parameters.Add("@DESCRIPCION", request.Descripcion);
            parameters.Add("@ID_COMPROBANTE", request.IdComprobante);
            parameters.Add("@DETALLE_JSON", detalleJson);

            var result = await connection.QueryFirstAsync<dynamic>(
                "SP_REGISTRAR_GASTO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ID_GASTO;
        }

        public async Task<GastoDetalleResponseDto?> ObtenerGastoDetalleAsync(string idGasto, string idUsuario)
        {
            using var connection = await _context.CreateConnectionAsync(idUsuario);

            using var multi = await connection.QueryMultipleAsync(
                "SP_GASTO_DETALLE",
                new { ID_GASTO = idGasto },
                commandType: CommandType.StoredProcedure
            );

            var cabecera = await multi.ReadFirstOrDefaultAsync<GastoDetalleCabeceraDto>();
            if (cabecera == null)
                return null;

            var detalle = (await multi.ReadAsync<GastoDetalleItemDto>()).ToList();
            var comprobante = await multi.ReadFirstOrDefaultAsync<GastoComprobanteDto>();

            return new GastoDetalleResponseDto
            {
                Cabecera = cabecera,
                Detalle = detalle,
                Comprobante = comprobante?.IdComprobante == null ? null : comprobante
            };
        }

        public async Task<IEnumerable<GastoDto>> ObtenerGastosAsync(
            string idUsuario,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            string? idCategoria,
            string? idMetodo,
            string? descripcion
        )
        {
            using var connection = await _context.CreateConnectionAsync(idUsuario);

            var gastos = await connection.QueryAsync<GastoDto>(
                "SP_GASTOS",
                new
                {
                    ID_USUARIO = idUsuario,
                    FECHA_DESDE = fechaDesde,
                    FECHA_HASTA = fechaHasta,
                    ID_CATEGORIA = idCategoria,
                    ID_METODO = idMetodo,
                    DESCRIPCION = descripcion
                },
                commandType: CommandType.StoredProcedure
            );

            return gastos;
        }

        public async Task EditarGastoAsync(string idUsuario, EditarGastoDto request)
        {
            using var connection = await _context.CreateConnectionAsync(idUsuario);

            var dtDetalles = new DataTable();
            dtDetalles.Columns.Add("PRODUCTO", typeof(string));
            dtDetalles.Columns.Add("PRECIO", typeof(decimal));

            foreach (var d in request.Detalles)
            {
                dtDetalles.Rows.Add(d.Producto, d.Precio);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@ID_GASTO", request.IdGasto);
            parameters.Add("@ID_CATEGORIA", request.IdCategoria);
            parameters.Add("@ID_METODO", request.IdMetodo);
            parameters.Add("@FECHA", request.Fecha);
            parameters.Add("@MONTO", request.Monto);
            parameters.Add("@TIPO", request.Tipo);
            parameters.Add("@DESCRIPCION", request.Descripcion);

            parameters.Add(
                "@DETALLES",
                dtDetalles.AsTableValuedParameter("TVP_DETALLE_GASTO")
            );

            await connection.ExecuteAsync(
                "SP_EDITAR_GASTO",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task EliminarGastoAsync(string idGasto, string idUsuario)
        {
            using var connection = await _context.CreateConnectionAsync(idUsuario);

            var rows = await connection.ExecuteAsync(
                "SP_ELIMINAR_GASTO",
                new
                {
                    ID_GASTO = idGasto
                },
                commandType: CommandType.StoredProcedure
            );

            if (rows == 0)
                throw new Exception("No se pudo eliminar el gasto o ya fue eliminado");
        }

    }
}
