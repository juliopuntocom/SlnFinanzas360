namespace PrjFinanzas360.DTOs
{
    // ── Respuesta global del dashboard ──────────────────────────────────────
    public class DashboardDto
    {
        public ResumenMensualDto Resumen { get; set; } = new();
        public List<CategoriaTopDto> TopCategorias { get; set; } = new();
        public List<EvolucionMesDto> Evolucion { get; set; } = new();
        public List<MovimientoDto> MovimientosRecientes { get; set; } = new();
        public List<PresupuestoDto> Presupuestos { get; set; } = new();
        public List<ComparacionDto> Comparacion { get; set; } = new();
        public List<MetodoPagoDto> MetodosPago { get; set; } = new();
    }

    // ── KPIs principales ────────────────────────────────────────────────────
    public class ResumenMensualDto
    {
        public decimal GastoMesActual { get; set; }
        public decimal GastoMesAnterior { get; set; }
        public decimal PromedioDiario { get; set; }
        public decimal VariacionPorcentual { get; set; }
        public int CantidadGastos { get; set; }
        public decimal PresupuestoMensual { get; set; }
        public decimal PorcentajePresupuesto { get; set; }
        public int MesActual { get; set; }
        public int AnioActual { get; set; }
    }

    // ── Categorías top ──────────────────────────────────────────────────────
    public class CategoriaTopDto
    {
        public string IdCategoria { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string? Icono { get; set; }
        public string? Color { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal Porcentaje { get; set; }
        public int CantidadGastos { get; set; }
    }

    // ── Evolución mes a mes ─────────────────────────────────────────────────
    public class EvolucionMesDto
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string NombreMes { get; set; } = string.Empty;
        public decimal TotalGastado { get; set; }
        public int CantidadGastos { get; set; }
    }

    // ── Movimientos recientes ───────────────────────────────────────────────
    public class MovimientoDto
    {
        public string IdGasto { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public int Tipo { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string? CategoriaIcono { get; set; }
        public string? CategoriaColor { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public int MetodoTipo { get; set; }
        public string FechaRelativa { get; set; } = string.Empty;
    }

    // ── Estado de presupuestos ──────────────────────────────────────────────
    public class PresupuestoDto
    {
        public string IdPresupuesto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string? Icono { get; set; }
        public string? Color { get; set; }
        public decimal PresupuestoAsignado { get; set; }
        public decimal GastadoActual { get; set; }
        public decimal Disponible { get; set; }
        public decimal PorcentajeUsado { get; set; }
        public string EstadoSemaforo { get; set; } = "OK";
    }

    // ── Comparación mensual ─────────────────────────────────────────────────
    public class ComparacionDto
    {
        public string Categoria { get; set; } = string.Empty;
        public string? Icono { get; set; }
        public string? Color { get; set; }
        public decimal MontoMesActual { get; set; }
        public decimal MontoMesAnterior { get; set; }
        public decimal Variacion { get; set; }
    }

    // ── Métodos de pago ─────────────────────────────────────────────────────
    public class MetodoPagoDto
    {
        public string MetodoPago { get; set; } = string.Empty;
        public string TipoNombre { get; set; } = string.Empty;
        public int Tipo { get; set; }
        public decimal MontoTotal { get; set; }
        public int CantidadUsos { get; set; }
        public decimal Porcentaje { get; set; }
    }
}