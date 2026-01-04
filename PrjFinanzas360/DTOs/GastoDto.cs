namespace PrjFinanzas360.DTOs
{
    public class GastoDto
    {
        public string IdGasto { get; set; }
        public string Nikname { get; set; }
        public string Categoria { get; set; }
        public string MetodoPago { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public int Tipo { get; set; }
        public string Descripcion { get; set; }
        public string? IdComprobante { get; set; }
    }

}
