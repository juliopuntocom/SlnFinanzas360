namespace PrjFinanzas360.DTOs
{
    public class DetalleGastoDto
    {
        public string Producto { get; set; } = null!;
        public decimal Precio { get; set; }

        public string IdCategoria { get; set; } = null!;
    }
}
