using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrjFinanzas360.Services;
using System.IO;
using System.Threading.Tasks;

namespace PrjFinanzas360.Controllers
{
    [ApiController]
    [Route("v1/ocr")]
    public class OcrController : ControllerBase
    {
        private readonly OcrService _ocrService;

        public OcrController(OcrService ocrService)
        {
            _ocrService = ocrService;
        }

        [HttpPost("procesar")]
        public async Task<IActionResult> ProcesarOCR(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo inválido");

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            Directory.CreateDirectory(uploadsPath);

            var filePath = Path.Combine(uploadsPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var resultado = await _ocrService.ProcesarAsync(filePath);

            return Ok(resultado);
        }
    }
}
