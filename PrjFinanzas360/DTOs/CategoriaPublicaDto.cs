namespace PrjFinanzas360.DTOs
{
    public class CategoriaPublicaDto
    {
        public string IdCategoria { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public bool EsPersonalizada { get; set; }
        public byte Estado { get; set; }
    }
}
