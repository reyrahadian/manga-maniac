using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using Serilog;
using System.Drawing;
using Image = iText.Layout.Element.Image;

namespace MangaManiac.Core
{
    public class ImageToPdfConverter
    {
        private readonly ILogger _logger;

        public ImageToPdfConverter(ILogger logger)
        {
            _logger = logger;
        }

        public void Convert(string pdfFilePath, string imageFilePath)
        {
            try
            {
                _logger.Information($"Converting {imageFilePath} to pdf");
                using (var bitmap = new Bitmap(imageFilePath))
                {
                    using (var pdfFile = File.Create(pdfFilePath))
                    {
                        var pdfDoc = new PdfDocument(new PdfWriter(pdfFile));
                        var doc = new Document(pdfDoc, new iText.Kernel.Geom.PageSize(bitmap.Width, bitmap.Height));
                        var img = new Image(ImageDataFactory.Create(imageFilePath));
                        doc.Add(img);

                        doc.Close();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Failed to convert {imageFilePath} to pdf");
            }
        }
    }
}
