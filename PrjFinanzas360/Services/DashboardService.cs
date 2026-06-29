using Microsoft.Data.SqlClient;
using PrjFinanzas360.DTOs;
using System.Data;

namespace PrjFinanzas360.Services
{
    // ── Interface ────────────────────────────────────────────────────────────
    public interface IDashboardService
    {
        Task<DashboardDto> ObtenerDashboardCompletoAsync(string idUsuario);
        Task<ResumenMensualDto> ObtenerResumenAsync(string idUsuario);
        Task<List<CategoriaTopDto>> ObtenerTopCategoriasAsync(string idUsuario, int top = 5);
        Task<List<EvolucionMesDto>> ObtenerEvolucionAsync(string idUsuario, int meses = 6);
        Task<List<MovimientoDto>> ObtenerMovimientosRecientesAsync(string idUsuario, int top = 8);
        Task<List<PresupuestoDto>> ObtenerPresupuestosAsync(string idUsuario);
        Task<List<ComparacionDto>> ObtenerComparacionMensualAsync(string idUsuario);
        Task<List<MetodoPagoDto>> ObtenerMetodosPagoAsync(string idUsuario);
    }

    // ── Implementación ───────────────────────────────────────────────────────
    public class DashboardService : IDashboardService
    {
        private readonly string _connectionString;

        public DashboardService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlServer1")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        // ── Dashboard completo (una sola llamada para el frontend) ────────────
        public async Task<DashboardDto> ObtenerDashboardCompletoAsync(string idUsuario)
        {
            var resumenTask = ObtenerResumenAsync(idUsuario);
            var topTask = ObtenerTopCategoriasAsync(idUsuario);
            var evolucionTask = ObtenerEvolucionAsync(idUsuario);
            var movimientosTask = ObtenerMovimientosRecientesAsync(idUsuario);
            var presupuestosTask = ObtenerPresupuestosAsync(idUsuario);
            var comparacionTask = ObtenerComparacionMensualAsync(idUsuario);
            var metodosTask = ObtenerMetodosPagoAsync(idUsuario);

            await Task.WhenAll(
                resumenTask,
                topTask,
                evolucionTask,
                movimientosTask,
                presupuestosTask,
                comparacionTask,
                metodosTask
            );

            return new DashboardDto
            {
                Resumen = await resumenTask,
                TopCategorias = await topTask,
                Evolucion = await evolucionTask,
                MovimientosRecientes = await movimientosTask,
                Presupuestos = await presupuestosTask,
                Comparacion = await comparacionTask,
                MetodosPago = await metodosTask
            };
        }

        // ── Resumen mensual ───────────────────────────────────────────────────
        public async Task<ResumenMensualDto> ObtenerResumenAsync(string idUsuario)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_DASHBOARD_RESUMEN_MENSUAL", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ResumenMensualDto
                {
                    GastoMesActual = reader.GetDecimal(reader.GetOrdinal("GastoMesActual")),
                    GastoMesAnterior = reader.GetDecimal(reader.GetOrdinal("GastoMesAnterior")),
                    PromedioDiario = reader.GetDecimal(reader.GetOrdinal("PromedioDiario")),
                    VariacionPorcentual = reader.GetDecimal(reader.GetOrdinal("VariacionPorcentual")),
                    CantidadGastos = reader.GetInt32(reader.GetOrdinal("CantidadGastos")),
                    PresupuestoMensual = reader.GetDecimal(reader.GetOrdinal("PresupuestoMensual")),
                    PorcentajePresupuesto = reader.GetDecimal(reader.GetOrdinal("PorcentajePresupuesto")),
                    MesActual = reader.GetInt32(reader.GetOrdinal("MesActual")),
                    AnioActual = reader.GetInt32(reader.GetOrdinal("AnioActual"))
                };
            }
            return new ResumenMensualDto();
        }

        // ── Top categorías ────────────────────────────────────────────────────
        public async Task<List<CategoriaTopDto>> ObtenerTopCategoriasAsync(string idUsuario, int top = 5)
        {
            var lista = new List<CategoriaTopDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_DASHBOARD_GASTOS_POR_CATEGORIA", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);
            cmd.Parameters.AddWithValue("@TOP", top);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new CategoriaTopDto
                {
                    IdCategoria = reader.GetString(reader.GetOrdinal("IdCategoria")),
                    Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                    Icono = reader.IsDBNull(reader.GetOrdinal("Icono")) ? null : reader.GetString(reader.GetOrdinal("Icono")),
                    Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
                    MontoTotal = reader.GetDecimal(reader.GetOrdinal("MontoTotal")),
                    Porcentaje = reader.GetDecimal(reader.GetOrdinal("Porcentaje")),
                    CantidadGastos = reader.GetInt32(reader.GetOrdinal("CantidadGastos"))
                });
            }
            return lista;
        }

        // ── Evolución mensual ─────────────────────────────────────────────────
        public async Task<List<EvolucionMesDto>> ObtenerEvolucionAsync(string idUsuario, int meses = 6)
        {
            var lista = new List<EvolucionMesDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_DASHBOARD_EVOLUCION_MENSUAL", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);
            cmd.Parameters.AddWithValue("@MESES_ATRAS", meses);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new EvolucionMesDto
                {
                    Anio = reader.GetInt32(reader.GetOrdinal("Anio")),
                    Mes = reader.GetInt32(reader.GetOrdinal("Mes")),
                    NombreMes = reader.GetString(reader.GetOrdinal("NombreMes")),
                    TotalGastado = reader.GetDecimal(reader.GetOrdinal("TotalGastado")),
                    CantidadGastos = reader.GetInt32(reader.GetOrdinal("CantidadGastos"))
                });
            }
            return lista;
        }

        // ── Movimientos recientes ─────────────────────────────────────────────
        public async Task<List<MovimientoDto>> ObtenerMovimientosRecientesAsync(string idUsuario, int top = 8)
        {
            var lista = new List<MovimientoDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_DASHBOARD_MOVIMIENTOS_RECIENTES", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);
            cmd.Parameters.AddWithValue("@TOP", top);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new MovimientoDto
                {
                    IdGasto = reader.GetString(reader.GetOrdinal("IdGasto")),
                    Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                    Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                    Tipo = reader.GetByte(reader.GetOrdinal("Tipo")),
                    Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                    CategoriaIcono = reader.IsDBNull(reader.GetOrdinal("CategoriaIcono")) ? null : reader.GetString(reader.GetOrdinal("CategoriaIcono")),
                    CategoriaColor = reader.IsDBNull(reader.GetOrdinal("CategoriaColor")) ? null : reader.GetString(reader.GetOrdinal("CategoriaColor")),
                    MetodoPago = reader.GetString(reader.GetOrdinal("MetodoPago")),
                    MetodoTipo = reader.GetByte(reader.GetOrdinal("MetodoTipo")),
                    FechaRelativa = reader.GetString(reader.GetOrdinal("FechaRelativa"))
                });
            }
            return lista;
        }

        // ── Presupuestos ──────────────────────────────────────────────────────
        public async Task<List<PresupuestoDto>> ObtenerPresupuestosAsync(string idUsuario)
        {
            var lista = new List<PresupuestoDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_DASHBOARD_ESTADO_PRESUPUESTOS", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new PresupuestoDto
                {
                    IdPresupuesto = reader.GetString(reader.GetOrdinal("IdPresupuesto")),
                    Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                    Icono = reader.IsDBNull(reader.GetOrdinal("Icono")) ? null : reader.GetString(reader.GetOrdinal("Icono")),
                    Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
                    PresupuestoAsignado = reader.GetDecimal(reader.GetOrdinal("PresupuestoAsignado")),
                    GastadoActual = reader.GetDecimal(reader.GetOrdinal("GastadoActual")),
                    Disponible = reader.GetDecimal(reader.GetOrdinal("Disponible")),
                    PorcentajeUsado = reader.GetDecimal(reader.GetOrdinal("PorcentajeUsado")),
                    EstadoSemaforo = reader.GetString(reader.GetOrdinal("EstadoSemaforo"))
                });
            }
            return lista;
        }

        // ── Comparación mensual ───────────────────────────────────────────────
        public async Task<List<ComparacionDto>> ObtenerComparacionMensualAsync(string idUsuario)
        {
            var lista = new List<ComparacionDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_DASHBOARD_COMPARACION_MENSUAL", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new ComparacionDto
                {
                    Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                    Icono = reader.IsDBNull(reader.GetOrdinal("Icono")) ? null : reader.GetString(reader.GetOrdinal("Icono")),
                    Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
                    MontoMesActual = reader.GetDecimal(reader.GetOrdinal("MontoMesActual")),
                    MontoMesAnterior = reader.GetDecimal(reader.GetOrdinal("MontoMesAnterior")),
                    Variacion = reader.GetDecimal(reader.GetOrdinal("Variacion"))
                });
            }
            return lista;
        }

        // ── Métodos de pago ───────────────────────────────────────────────────
        public async Task<List<MetodoPagoDto>> ObtenerMetodosPagoAsync(string idUsuario)
        {
            var lista = new List<MetodoPagoDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_DASHBOARD_METODOS_PAGO", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new MetodoPagoDto
                {
                    MetodoPago = reader.GetString(reader.GetOrdinal("MetodoPago")),
                    TipoNombre = reader.GetString(reader.GetOrdinal("TipoNombre")),
                    Tipo = reader.GetByte(reader.GetOrdinal("Tipo")),
                    MontoTotal = reader.GetDecimal(reader.GetOrdinal("MontoTotal")),
                    CantidadUsos = reader.GetInt32(reader.GetOrdinal("CantidadUsos")),
                    Porcentaje = reader.GetDecimal(reader.GetOrdinal("Porcentaje"))
                });
            }
            return lista;
        }
    }
}