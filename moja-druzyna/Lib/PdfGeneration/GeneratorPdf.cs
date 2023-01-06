using System;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using moja_druzyna.Lib.Order;

namespace moja_druzyna.Lib.PdfGeneration
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

        public void GenerateOrder(FormOrder order)
        {
            PdfDocument doc = new PdfDocument();
            PdfPageBase page = doc.Pages.Add();

            int posX;
            int posY;
            int mainCounter = 1;
            int secondaryCounter = 1;

            page.Canvas.DrawString($"ZHP, {order.TeamName}",
                new PdfFont(PdfFontFamily.Helvetica, 13f),
                new PdfSolidBrush(Color.Black),
                new PointF(50, 50));

            page.Canvas.DrawString($"{order.Location}, {order.CreationDate}",
                new PdfFont(PdfFontFamily.Helvetica, 13f),
                new PdfSolidBrush(Color.Black),
                new PointF(380 - (order.Location.Length*6), 50));

            page.Canvas.DrawString($"Rozkaz {order.OrderNumber}",
                new PdfFont(PdfFontFamily.Helvetica, 13f),
                new PdfSolidBrush(Color.Black),
                new PointF(200, 80));

            posX = 50;
            posY = 100;

            if (order.Layoffs != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Zwolnienia",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (Layoff layoff in order.Layoffs)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.Zwalniam dh. {layoff.ScoutName} {layoff.ScoutSurname} z funkcji {layoff.RoleName}.";
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if(posX < 480 - (word.Length*6))
                        {
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7*(word.Length + 1);
                        } else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                    }

                    secondaryCounter = secondaryCounter + 1;
                    posX = 50;
                    posY = posY + 20;
                }
                mainCounter = mainCounter + 1;
                secondaryCounter = 1;
            }

            if (order.Appointments != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Mianowania",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (Appointment app in order.Appointments)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.Mianuje dh. {app.ScoutName} {app.ScoutSurname} na funkcje {app.RoleName}.";
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                    }

                    secondaryCounter = secondaryCounter + 1;
                    posX = 50;
                    posY = posY + 20;
                }
                mainCounter = mainCounter + 1;
                secondaryCounter = 1;
            }

            if (order.TrialClosings != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Zamkniecia prob",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (TrialClosing c in order.TrialClosings)
                {
                    string text = $"{mainCounter}.{secondaryCounter}.Zamykam probe i przyznaje {c.TrialType} {c.TrialName} dh. {c.ScoutName} {c.ScoutSurname}";
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                    }

                    secondaryCounter = secondaryCounter + 1;
                    posX = 50;
                    posY = posY + 20;
                }
                mainCounter = mainCounter + 1;
                secondaryCounter = 1;
            }

            if (order.TrialOpenings != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Otwarcia prob",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (TrialOpening o in order.TrialOpenings)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.Otwieram probe na {o.TrialType} {o.TrialName} dh. {o.ScoutName} {o.ScoutSurname}";
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                    }

                    secondaryCounter = secondaryCounter + 1;
                    posX = 50;
                    posY = posY + 20;
                }
                mainCounter = mainCounter + 1;
                secondaryCounter = 1;
            }

            if (order.TrialClosings != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Przyznanie punktow za stopnie i sprawnosci",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (TrialClosing c in order.TrialClosings)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text;
                    int points;
                    if(c.TrialType == "sprawność"){
                        points = 25;
                        text = $"{mainCounter}.{secondaryCounter}.Za zdobycie {c.TrialType} {c.TrialName} przyznaje dh. {c.ScoutName} {c.ScoutSurname} {points} punktow do wspolzawodnictwa.";
                    } else if(c.TrialType == "krzyż")
                    {
                        points = 50;
                        text = $"{mainCounter}.{secondaryCounter}.Za zrealizowania proby harcerskiej przyznaje dh. {c.ScoutName} {c.ScoutSurname} {points} punktow do wspolzawodnictwa.";
                    } else 
                    {
                        points = 100;
                        text = $"{mainCounter}.{secondaryCounter}.Za zdobycie {c.TrialType} {c.TrialName} przyznaje dh. {c.ScoutName} {c.ScoutSurname} {points} punktow do wspolzawodnictwa.";
                    }
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                    }

                    secondaryCounter = secondaryCounter + 1;
                    posX = 50;
                    posY = posY + 20;
                }
                mainCounter = mainCounter + 1;
                secondaryCounter = 1;
            }

            if (order.GamePointsEntries != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Podsumowanie wspolzawodnictwa",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (GamePointsEntry g in order.GamePointsEntries)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.Za wyniki w grze {g.GameName} przyznaje dh. {g.ScoutName} {g.ScoutSurname} {g.Points} punktow do wspolzawodnictwa.";
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                    }

                    secondaryCounter = secondaryCounter + 1;
                    posX = 50;
                    posY = posY + 20;
                }
                mainCounter = mainCounter + 1;
                secondaryCounter = 1;
            }

            if (order.ReprimendsAndPraises != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Pochwaly, wyroznienia i nagany",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (ReprimendsAndPraises e in order.ReprimendsAndPraises)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.Udzielam dh. {e.ScoutName} {e.ScoutSurname} {e.Type} za {e.Explanation}";
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                    }

                    secondaryCounter = secondaryCounter + 1;
                    posX = 50;
                    posY = posY + 20;
                }
                mainCounter = mainCounter + 1;
                secondaryCounter = 1;
            }

            if (order.Exclusions != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Skreslenia z listy czlonkow",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (Exclusion d in order.Exclusions)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.Z powodu {d.Reason} skreslam dh. {d.ScoutName} {d.ScoutSurname} z listy czlonkow druzyny.";
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                    }

                    secondaryCounter = secondaryCounter + 1;
                    posX = 50;
                    posY = posY + 20;
                }
                mainCounter = mainCounter + 1;
                secondaryCounter = 1;
            }

            if (order.Other != null && order.Other.Contents != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Inne",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (string s in order.Other.Contents.Split('\n'))
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.{s}";
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                new PdfFont(PdfFontFamily.Helvetica, 13f),
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                    }

                    secondaryCounter = secondaryCounter + 1;
                    posX = 50;
                    posY = posY + 20;
                }
                mainCounter = mainCounter + 1;
                secondaryCounter = 1;
            }

            if (posY > 650)
            {
                page = doc.Pages.Add();
                posY = 50;
            }
            page.Canvas.DrawString($"Czuwaj!",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(350, posY+50));
            page.Canvas.DrawString($"Druzynowy {order.TeamName}",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(300, posY + 70));

            doc.SaveToFile($"{order.OrderNumber}.pdf");

        }
    }
}
