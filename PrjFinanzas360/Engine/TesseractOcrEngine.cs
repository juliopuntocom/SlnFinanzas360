using Tesseract;
using PrjFinanzas360.Interface;
using System.Threading.Tasks;
using System;

namespace PrjFinanzas360.Engine
{
    public class TesseractOcrEngine : IOcrEngine
    {
        private readonly string _tessDataPath = "./ocr/tessdata";

        public async Task<string> ReadTextAsync(string imagePath)
        {
            return await Task.Run(() =>
            {
                using var engine = new TesseractEngine(
                    _tessDataPath,
                    "spa+eng",
                    EngineMode.Default
                );

                engine.SetVariable(
                    "tessedit_char_whitelist",
                    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.:,/-%$ "
                );

                using var img = Pix.LoadFromFile(imagePath);
                using var page = engine.Process(img);

                return page.GetText();
            });
        }
    }
}