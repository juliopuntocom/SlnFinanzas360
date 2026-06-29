using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjFinanzas360.DTOs;
using PrjFinanzas360.Services;
using System.IdentityModel.Tokens.Jwt;

namespace PrjFinanzas360.Controllers
{
    /// <summary>
    /// Dashboard principal — métricas y KPIs de finanzas personales
    /// </summary>
    [ApiController]
    [Route("v1/dashboard")]
    [Authorize]
    [Produces("application/json")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helper: extrae ID de usuario del JWT
        // ─────────────────────────────────────────────────────────────────────
        private string? ObtenerIdUsuario() =>
            User.FindFirst("uid")?.Value ??
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        // ─────────────────────────────────────────────────────────────────────
        // GET v1/dashboard
        // Dashboard completo (una sola petición — recomendado para el frontend)
        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Retorna todos los datos del dashboard en una sola respuesta:
        /// KPIs, categorías top, evolución, movimientos, presupuestos,
        /// comparación mensual y distribución por método de pago.
        /// </summary>
        /// <response code="200">Dashboard cargado correctamente</response>
        /// <response code="401">Token inválido o expirado</response>
        [HttpGet]
        [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerDashboard()
        {
            var idUsuario = ObtenerIdUsuario();
            if (string.IsNullOrEmpty(idUsuario)) return Unauthorized();

            var dashboard = await _dashboardService.ObtenerDashboardCompletoAsync(idUsuario);
            return Ok(dashboard);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET v1/dashboard/resumen
        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// KPIs del mes actual: gasto total, promedio diario, variación
        /// porcentual vs mes anterior y % de presupuesto consumido.
        /// </summary>
        [HttpGet("resumen")]
        [ProducesResponseType(typeof(ResumenMensualDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerResumen()
        {
            var idUsuario = ObtenerIdUsuario();
            if (string.IsNullOrEmpty(idUsuario)) return Unauthorized();

            var resumen = await _dashboardService.ObtenerResumenAsync(idUsuario);
            return Ok(resumen);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET v1/dashboard/categorias-top?top=5
        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Top N categorías con mayor gasto en el mes actual,
        /// incluyendo monto, porcentaje del total y cantidad de gastos.
        /// </summary>
        /// <param name="top">Cantidad máxima de categorías a retornar (default 5)</param>
        [HttpGet("categorias-top")]
        [ProducesResponseType(typeof(List<CategoriaTopDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerTopCategorias([FromQuery] int top = 5)
        {
            var idUsuario = ObtenerIdUsuario();
            if (string.IsNullOrEmpty(idUsuario)) return Unauthorized();

            var categorias = await _dashboardService.ObtenerTopCategoriasAsync(idUsuario, top);
            return Ok(categorias);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET v1/dashboard/evolucion?meses=6
        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Evolución del gasto de los últimos N meses para gráfico de línea/barra.
        /// </summary>
        /// <param name="meses">Cantidad de meses hacia atrás (default 6, max 12)</param>
        [HttpGet("evolucion")]
        [ProducesResponseType(typeof(List<EvolucionMesDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerEvolucion([FromQuery] int meses = 6)
        {
            var idUsuario = ObtenerIdUsuario();
            if (string.IsNullOrEmpty(idUsuario)) return Unauthorized();

            if (meses < 1 || meses > 12) meses = 6;

            var evolucion = await _dashboardService.ObtenerEvolucionAsync(idUsuario, meses);
            return Ok(evolucion);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET v1/dashboard/movimientos-recientes?top=8
        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Últimos N gastos registrados con categoría, método de pago y fecha relativa.
        /// </summary>
        /// <param name="top">Cantidad de movimientos a retornar (default 8)</param>
        [HttpGet("movimientos-recientes")]
        [ProducesResponseType(typeof(List<MovimientoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerMovimientosRecientes([FromQuery] int top = 8)
        {
            var idUsuario = ObtenerIdUsuario();
            if (string.IsNullOrEmpty(idUsuario)) return Unauthorized();

            var movimientos = await _dashboardService.ObtenerMovimientosRecientesAsync(idUsuario, top);
            return Ok(movimientos);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET v1/dashboard/presupuestos
        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Estado de todos los presupuestos del mes: monto asignado, gastado,
        /// disponible, % de uso y semáforo (OK / ALERTA / EXCEDIDO).
        /// </summary>
        [HttpGet("presupuestos")]
        [ProducesResponseType(typeof(List<PresupuestoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerPresupuestos()
        {
            var idUsuario = ObtenerIdUsuario();
            if (string.IsNullOrEmpty(idUsuario)) return Unauthorized();

            var presupuestos = await _dashboardService.ObtenerPresupuestosAsync(idUsuario);
            return Ok(presupuestos);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET v1/dashboard/comparacion-mensual
        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Comparación de gastos por categoría entre el mes actual y el anterior.
        /// </summary>
        [HttpGet("comparacion-mensual")]
        [ProducesResponseType(typeof(List<ComparacionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerComparacionMensual()
        {
            var idUsuario = ObtenerIdUsuario();
            if (string.IsNullOrEmpty(idUsuario)) return Unauthorized();

            var comparacion = await _dashboardService.ObtenerComparacionMensualAsync(idUsuario);
            return Ok(comparacion);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET v1/dashboard/metodos-pago
        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Distribución de gastos del mes por método de pago (efectivo, tarjeta, etc.)
        /// con monto, cantidad de usos y porcentaje del total.
        /// </summary>
        [HttpGet("metodos-pago")]
        [ProducesResponseType(typeof(List<MetodoPagoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerMetodosPago()
        {
            var idUsuario = ObtenerIdUsuario();
            if (string.IsNullOrEmpty(idUsuario)) return Unauthorized();

            var metodos = await _dashboardService.ObtenerMetodosPagoAsync(idUsuario);
            return Ok(metodos);
        }
    }
}