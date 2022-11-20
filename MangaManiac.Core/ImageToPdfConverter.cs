using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using System.Drawing;
using Image = iText.Layout.Element.Image;

namespace MangaManiac.Core
{
    public class ImageToPdfConverter
    {
        public void Convert(string pdfFilePath, string imageFilePath)
        {

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
    }
}
