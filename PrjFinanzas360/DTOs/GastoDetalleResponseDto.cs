namespace PrjFinanzas360.DTOs
{
    public class GastoDetalleResponseDto
    {
        public GastoDetalleCabeceraDto Cabecera { get; set; }
        public List<GastoDetalleItemDto> Detalle { get; set; }
        public GastoComprobanteDto? Comprobante { get; set; }
    }
}
