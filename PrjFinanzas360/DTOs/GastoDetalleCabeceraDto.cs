namespace PrjFinanzas360.DTOs
{
    public class GastoDetalleCabeceraDto
    {
        public string IdGasto { get; set; }
        public string Categoria { get; set; }
        public string MetodoPago { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public int Tipo { get; set; }
        public string Descripcion { get; set; }
        public string? IdComprobante { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
