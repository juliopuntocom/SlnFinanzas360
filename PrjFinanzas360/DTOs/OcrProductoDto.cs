using System.Text.Json.Serialization;

namespace PrjFinanzas360.DTOs
{
    public class OcrProductoDto
    {
        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }

        [JsonPropertyName("cantidad")]
        public decimal? Cantidad { get; set; }

        [JsonPropertyName("precio_unitario")]
        public decimal? PrecioUnitario { get; set; }

        [JsonPropertyName("total")]
        public decimal? Total { get; set; }

        [JsonPropertyName("categoria_id")]
        public string? CategoriaId { get; set; }
    }
}