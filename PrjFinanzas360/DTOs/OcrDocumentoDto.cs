using System.Text.Json.Serialization;

namespace PrjFinanzas360.DTOs
{
    public class OcrDocumentoDto
    {
        [JsonPropertyName("emisor")]
        public string? Emisor { get; set; }

        [JsonPropertyName("ruc")]
        public string? Ruc { get; set; }

        [JsonPropertyName("tipo_documento")]
        public string? TipoDocumento { get; set; }

        [JsonPropertyName("fecha")]
        public string? Fecha { get; set; }

        [JsonPropertyName("metodo_pago")]
        public string? MetodoPago { get; set; }

        [JsonPropertyName("total_general")]
        public decimal? TotalGeneral { get; set; }

        [JsonPropertyName("productos")]
        public List<OcrProductoDto> Productos { get; set; } = new();

        // Estos dos no se usaban antes pero vienen del JSON de Python;
        // los dejo mapeados por si los necesitas más adelante (ej. auditoría/debug).
        [JsonPropertyName("texto_crudo")]
        public string? TextoCrudo { get; set; }
    }
}