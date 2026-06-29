namespace PrjFinanzas360.DTOs
{
    public class RegistrarGastoDto
    {
        public string IdMetodo { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public byte Tipo { get; set; }
        public string Descripcion { get; set; } = null!;
        public string? IdComprobante { get; set; }
        public List<DetalleGastoDto>? Detalle { get; set; }
    }
}
