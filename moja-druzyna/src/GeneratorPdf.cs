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
            Random generator = new Random();
            int mark = generator.Next();
            
            PdfDocument doc = new PdfDocument();

            PdfPageBase page = doc.Pages.Add();

            page.Canvas.DrawString($"Hello World, {mark}",
                new PdfFont(PdfFontFamily.Helvetica, 13f),
                new PdfSolidBrush(Color.Black),
                new PointF(50, 50));

            doc.SaveToFile($"Output{mark}.pdf");
        }

        public void GenerateOrder(Order order)
        {
            PdfDocument doc = new PdfDocument();
            PdfPageBase page = doc.Pages.Add();

            int posX;
            int posY;
            int mainCounter = 1;
            int secondaryCounter = 1;

            page.Canvas.DrawString($"ZHP, {order.Team}",
                new PdfFont(PdfFontFamily.Helvetica, 13f),
                new PdfSolidBrush(Color.Black),
                new PointF(50, 50));

            page.Canvas.DrawString($"{order.Location}, {order.Date}",
                new PdfFont(PdfFontFamily.Helvetica, 13f),
                new PdfSolidBrush(Color.Black),
                new PointF(300, 50));

            page.Canvas.DrawString($"Rozkaz {order.Number}",
                new PdfFont(PdfFontFamily.Helvetica, 13f),
                new PdfSolidBrush(Color.Black),
                new PointF(200, 80));

            posY = 100;

            if (order.releasings != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Zwolnienia",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (Releasing release in order.releasings)
                {
                    page.Canvas.DrawString($"{mainCounter}.{secondaryCounter}.Zwalniam dh. {release.nameScout} {release.surnameScout} z funkcji {release.Function}.",
                        new PdfFont(PdfFontFamily.Helvetica, 13f),
                        new PdfSolidBrush(Color.Black),
                        new PointF(50, posY));

                    secondaryCounter = secondaryCounter + 1;
                    posY = posY + 20;
                }
                mainCounter = mainCounter + 1;
                secondaryCounter = 1;
            }

            if (order.appointments != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Mianowania",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (Appointment app in order.appointments)
                {
                    page.Canvas.DrawString($"{mainCounter}.{secondaryCounter}.Mianuję dh. {app.nameScout} {app.surnameScout} na funkcję {app.Function}.",
                        new PdfFont(PdfFontFamily.Helvetica, 13f),
                        new PdfSolidBrush(Color.Black),
                        new PointF(50, posY));

                    secondaryCounter = secondaryCounter + 1;
                    posY = posY + 20;
                }
                mainCounter = mainCounter + 1;
                secondaryCounter = 1;
            }

            doc.SaveToFile($"{order.Number}.pdf");

        }
    }
}
