using PrjFinanzas360.DTOs;
using PrjFinanzas360.Interface;
using System;
using System.Threading.Tasks;

namespace PrjFinanzas360.Services
{
    public class OcrService
    {
        private readonly IOcrEngine _ocrEngine;
        private readonly OcrProcesor _procesor;

        public OcrService(IOcrEngine ocrEngine)
        {
            _ocrEngine = ocrEngine;
            _procesor = new OcrProcesor();
        }

        public async Task<OcrResult> ProcesarAsync(string path)
        {
            var result = new OcrResult();

            try
            {
                string imagePath = path;

                // 🟢 Si es PDF → convertir
                if (Path.GetExtension(path).ToLower() == ".pdf")
                {
                    var converter = new PdfToImageConverter();
                    imagePath = converter.ConvertFirstPageToImage(path);
                }

                var preprocessedPath = _procesor.PreprocesarImagen(imagePath);
                var texto = await _ocrEngine.ReadTextAsync(preprocessedPath);

                result.TextoCrudo = texto;
                result.Ruc = _procesor.ExtraerRuc(texto);
                result.Total = _procesor.ExtraerTotal(texto);
                result.Fecha = _procesor.ExtraerFecha(texto);

                result.ProcesadoCorrectamente =
                    !string.IsNullOrEmpty(result.Ruc) && result.Total.HasValue;

                return result;
            }
            catch (Exception ex)
            {
                result.ProcesadoCorrectamente = false;
                result.TextoCrudo = ex.Message; // DEBUG
                return result;
            }

        }

    }
}
