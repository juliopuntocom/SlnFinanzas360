using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjFinanzas360.DTOs;
using PrjFinanzas360.Services;
using System.Security.Claims;
using System.Text.Json;

namespace PrjFinanzas360.Controllers
{
    [ApiController]
    [Route("v1/ocr")]
    [Authorize]
    public class OcrController : ControllerBase
    {
        private readonly OcrService _ocrService;

        public OcrController(OcrService ocrService)
        {
            _ocrService = ocrService;
        }

        // Angular ya llamó al microservicio Python y tiene el JSON resultante.
        // Este endpoint recibe el archivo original (para guardarlo) MÁS ese JSON
        // (para persistirlo), todo como multipart/form-data:
        //   - "archivo"   -> el File original (PDF/imagen)
        //   - "documento" -> el JSON de Python, como string, dentro del campo "documento"
        [HttpPost("procesar")]
        public async Task<IActionResult> ProcesarComprobante(IFormFile archivo, [FromForm] string documento)
        {
            var idUsuario =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value ??
                User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(idUsuario))
                return Unauthorized();

            if (archivo == null || archivo.Length == 0)
                return BadRequest(new { mensaje = "No se recibió ningún archivo" });

            var tiposPermitidos = new[] { "image/jpeg", "image/png", "application/pdf" };
            if (!tiposPermitidos.Contains(archivo.ContentType))
                return BadRequest(new { mensaje = "Formato no soportado. Use JPG, PNG o PDF." });

            if (archivo.Length > 10 * 1024 * 1024)
                return BadRequest(new { mensaje = "El archivo supera los 10MB permitidos." });

            if (string.IsNullOrWhiteSpace(documento))
                return BadRequest(new { mensaje = "No se recibió el resultado del OCR" });

            OcrDocumentoDto documentoExtraido;
            try
            {
                documentoExtraido = JsonSerializer.Deserialize<OcrDocumentoDto>(
                    documento,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new OcrDocumentoDto();
            }
            catch (JsonException)
            {
                return BadRequest(new { mensaje = "El resultado del OCR no tiene un formato JSON válido" });
            }

            var resultado = await _ocrService.ProcesarYGuardarAsync(idUsuario, archivo, documentoExtraido);

            return Ok(resultado);
        }
    }
}