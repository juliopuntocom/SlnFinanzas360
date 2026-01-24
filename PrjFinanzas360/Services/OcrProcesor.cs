using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;
using System;


namespace PrjFinanzas360.Services
{
    public class OcrProcesor
    {
        public string PreprocesarImagen(string inputPath)
        {
            var outputPath = inputPath.Replace(".jpg", "_pre.jpg");

            using var image = Image.Load(inputPath);

            image.Mutate(x =>
            {
                x.Grayscale();
                x.Contrast(1.5f);
                x.BinaryThreshold(0.5f);
            });

            image.Save(outputPath);
            return outputPath;
        }

        public string ExtraerRuc(string texto)
        {
            var match = Regex.Match(texto, @"\b(10|20)\d{9}\b");
            return match.Success ? match.Value : null;
        }

        public decimal? ExtraerTotal(string texto)
        {
            var match = Regex.Match(texto,
                @"TOTAL\s*(S\/\.?|S\/)?\s*(\d+(\.\d{2})?)",
                RegexOptions.IgnoreCase
            );

            return match.Success
                ? decimal.Parse(match.Groups[2].Value)
                : null;
        }

        public DateTime? ExtraerFecha(string texto)
        {
            var match = Regex.Match(texto,
                @"(\d{2}\/\d{2}\/\d{4})"
            );

            return match.Success
                ? DateTime.Parse(match.Value)
                : null;
        }

    }
}
