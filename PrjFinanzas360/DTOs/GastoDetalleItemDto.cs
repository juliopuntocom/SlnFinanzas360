namespace PrjFinanzas360.DTOs
{
    public class GastoDetalleItemDto
    {
        public string IdDetalle { get; set; } = string.Empty;

        public string IdCategoria { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public string Producto { get; set; } = string.Empty;

        public decimal Precio { get; set; }
    }
}