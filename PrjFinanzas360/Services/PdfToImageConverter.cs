using PdfiumViewer;
using System.Drawing;
using System.Drawing.Imaging;

namespace PrjFinanzas360.Services
{
    public class PdfToImageConverter
    {
        public string ConvertFirstPageToImage(string pdfPath)
        {
            var outputImage = pdfPath.Replace(".pdf", "_page1.png");

            using var document = PdfDocument.Load(pdfPath);
            using var image = document.Render(
                0, // página 1
                300,
                300,
                PdfRenderFlags.Annotations
            );

            image.Save(outputImage, ImageFormat.Png);
            return outputImage;
        }
    }
}
