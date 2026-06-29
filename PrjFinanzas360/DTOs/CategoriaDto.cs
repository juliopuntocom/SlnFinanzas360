namespace PrjFinanzas360.DTOs
{
    public class CategoriaDto
    {
        public string IdCat { get; set; }
        public string Categoria { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public bool ESPersonalizada { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public byte Estado { get; set; }
    }
}
