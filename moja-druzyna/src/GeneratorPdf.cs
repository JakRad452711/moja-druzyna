using System;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

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
                new PointF(380 - (order.Location.Length*6), 50));

            page.Canvas.DrawString($"Rozkaz {order.Number}",
                new PdfFont(PdfFontFamily.Helvetica, 13f),
                new PdfSolidBrush(Color.Black),
                new PointF(200, 80));

            posX = 50;
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
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.Bardzo dlugi testowy ciag znakow zeby sprawdzic, czy dobrze lamie sie linia. Zwalniam dh. {release.nameScout} {release.surnameScout} z funkcji {release.Function}.";
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

            if (order.appointments != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Mianowania",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (Appointment app in order.appointments)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.Mianuje dh. {app.nameScout} {app.surnameScout} na funkcje {app.Function}.";
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

            if (order.closings != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Zamkniecia prob",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (ClosingTrial c in order.closings)
                {
                    string text = $"{mainCounter}.{secondaryCounter}.Zamykam probe i przyznaje {c.trialType} {c.trialName} dh. {c.person.Name} {c.person.Surname}";
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

            if (order.opens != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Otwarcia prob",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (OpenTrial o in order.opens)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.Otwieram probe na {o.trialType} {o.trialName} dh. {o.person.Name} {o.person.Surname}";
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

            if (order.closings != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Przyznanie punktow za stopnie i sprawnosci",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (ClosingTrial c in order.closings)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text;
                    int points;
                    if(c.trialType == "sprawność"){
                        points = 25;
                        text = $"{mainCounter}.{secondaryCounter}.Za zdobycie {c.trialType} {c.trialName} przyznaje dh. {c.person.Name} {c.person.Surname} {points} punktow do wspolzawodnictwa.";
                    } else if(c.trialType == "krzyż")
                    {
                        points = 50;
                        text = $"{mainCounter}.{secondaryCounter}.Za zrealizowania proby harcerskiej przyznaje dh. {c.person.Name} {c.person.Surname} {points} punktow do wspolzawodnictwa.";
                    } else 
                    {
                        points = 100;
                        text = $"{mainCounter}.{secondaryCounter}.Za zdobycie {c.trialType} {c.trialName} przyznaje dh. {c.person.Name} {c.person.Surname} {points} punktow do wspolzawodnictwa.";
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

            if (order.games != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Podsumowanie wspolzawodnictwa",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (Game g in order.games)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.Za wyniki w grze {g.gameName} przyznaje dh. {g.person.Name} {g.person.Surname} {g.points} punktow do wspolzawodnictwa.";
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

            if (order.extras != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Pochwaly, wyroznienia i nagany",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (Extraordinary e in order.extras)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.Udzielam dh. {e.person.Name} {e.person.Surname} {e.type} za {e.justification}";
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

            if (order.deletions != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Skreslenia z listy czlonkow",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (Deletion d in order.deletions)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.Z powodu {d.justification} skreslam dh. {d.person.Name} {d.person.Surname} z listy czlonkow druzyny.";
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

            if (order.others != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Inne",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (string s in order.others)
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
            page.Canvas.DrawString($"Druzynowy {order.Team}",
                    new PdfFont(PdfFontFamily.Helvetica, 13f),
                    new PdfSolidBrush(Color.Black),
                    new PointF(300, posY + 70));

            doc.SaveToFile($"{order.Number}.pdf");

        }
    }
}
