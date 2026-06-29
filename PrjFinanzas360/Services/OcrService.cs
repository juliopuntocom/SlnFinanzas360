using Dapper;
using PrjFinanzas360.Data;
using PrjFinanzas360.DTOs;
using System.Data;
using System.Security.Cryptography;
using System.Text.Json;

namespace PrjFinanzas360.Services
{
    public class OcrService
    {
        private readonly DapperContext _context;

        public OcrService(DapperContext context)
        {
            _context = context;
        }

        // Ya no llama a ningún servicio de OCR externo: el documentoExtraido
        // llega listo desde Angular (resultado del microservicio Python).
        // Esta función solo se encarga de persistir el comprobante y su resultado OCR.
        public async Task<OcrResultadoDto> ProcesarYGuardarAsync(
            string idUsuario,
            IFormFile archivo,
            OcrDocumentoDto documentoExtraido)
        {
            // 1. Leer bytes y calcular hash
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await archivo.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            var hash = ComputeSha256(fileBytes);

            // 2. Convertir a base64 para guardar
            var base64Archivo = Convert.ToBase64String(fileBytes);
            var dataUri = $"data:{archivo.ContentType};base64,{base64Archivo}";

            // 3. Generar IDs
            var idComprobante = await GenerarIdAsync("COMPROBANTE", "COM");
            var idOcr = await GenerarIdAsync("OCR_RESULTADO", "OCR");

            // 4. Guardar COMPROBANTE en BD
            using var connection = await _context.CreateConnectionAsync(idUsuario);

            await connection.ExecuteAsync(
                "SP_REGISTRAR_COMPROBANTE",
                new
                {
                    ID_COMPROBANTE = idComprobante,
                    ID_USUARIO = idUsuario,
                    ARCHIVO = dataUri,
                    HASH_ARCHIVO = hash,
                    ASOCIADO_GASTO = 0
                },
                commandType: CommandType.StoredProcedure
            );

            // 5. Guardar OCR_RESULTADO en BD (el documento ya viene extraído desde Angular/Python)
            var detalleJson = documentoExtraido.Productos != null
                ? JsonSerializer.Serialize(documentoExtraido.Productos)
                : null;

            const bool procesadoCorrectamente = true; // si llegó hasta aquí, Python ya procesó OK
            const string observacion = "-";

            await connection.ExecuteAsync(
                "SP_REGISTRAR_OCR_RESULTADO",
                new
                {
                    ID_OCR = idOcr,
                    ID_COMPROBANTE = idComprobante,
                    RUC = documentoExtraido.Ruc,
                    RAZON_SOCIAL = documentoExtraido.Emisor,
                    OBSERVACION = observacion,
                    FECHA = documentoExtraido.Fecha,
                    TOTAL = documentoExtraido.TotalGeneral,
                    DETALLE_COM = detalleJson,
                    PROCESADO_CORRECTAMENTE = procesadoCorrectamente
                },
                commandType: CommandType.StoredProcedure
            );

            // 6. Retornar resultado al frontend
            return new OcrResultadoDto
            {
                IdComprobante = idComprobante,
                Documento = documentoExtraido
            };
        }

        private string ComputeSha256(byte[] data)
        {
            var hash = SHA256.HashData(data);
            return Convert.ToHexString(hash).ToLower();
        }

        private async Task<string> GenerarIdAsync(string tabla, string prefijo)
        {
            // Adapta según tu lógica de generación de IDs
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return $"{prefijo}{timestamp % 100000:D5}";
        }
    }
}