namespace PrjFinanzas360.DTOs
{
    public class GastoComprobanteDto
    {
        public string? IdComprobante { get; set; }
        public string? Archivo { get; set; }
        public DateTime? FechaRegistro { get; set; }

        public string? IdOcr { get; set; }
        public string? Ruc { get; set; }
        public string? RazonSocial { get; set; }
        public string? Observacion { get; set; }
        public DateTime? FechaComprobante { get; set; }
        public decimal? Total { get; set; }
        public string? DetalleComprobante { get; set; }
        public bool? ProcesadoCorrectamente { get; set; }
        public DateTime? FechaProceso { get; set; }
    }
}
