using System;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Drawing;

namespace moja_druzyna.src
{
    public class GeneratorPdf
    {
        public void CreateTestPdf()
        {
            PdfDocument doc = new PdfDocument();

            PdfPageBase page = doc.Pages.Add();

            page.Canvas.DrawString("Hello World",
                new PdfFont(PdfFontFamily.Helvetica, 13f),
                new PdfSolidBrush(Color.Black),
                new PointF(50, 50));

            doc.SaveToFile("Output.pdf");
        }
    }
}
