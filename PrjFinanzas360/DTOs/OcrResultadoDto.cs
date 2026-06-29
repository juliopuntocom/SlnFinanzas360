namespace PrjFinanzas360.DTOs
{
    public class OcrResultadoDto
    {
        public string IdComprobante { get; set; } = string.Empty;
        public OcrDocumentoDto Documento { get; set; } = new();
    }

}
