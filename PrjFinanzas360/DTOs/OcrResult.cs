namespace PrjFinanzas360.DTOs
{
    public class OcrResult
    {
        public string Ruc { get; set; }
        public string RazonSocial { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal? Total { get; set; }
        public bool ProcesadoCorrectamente { get; set; }
        public string TextoCrudo { get; set; }
    }

}
