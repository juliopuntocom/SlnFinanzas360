namespace PrjFinanzas360.DTOs
{
    public class EditarGastoDto
    {
        public string IdGasto { get; set; } = null!;
        public string IdCategoria { get; set; } = null!;
        public string IdMetodo { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public byte Tipo { get; set; }
        public string Descripcion { get; set; } = null!;
        public List<DetalleEditarGastoDto> Detalles { get; set; } = new();
    }
}
