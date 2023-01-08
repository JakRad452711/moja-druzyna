using System;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using moja_druzyna.Lib.Order;
using moja_druzyna.Models;

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
            PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(new Font(new FontFamily("Arial"), 13, FontStyle.Regular), true);

            int posX;
            int posY;
            int mainCounter = 1;
            int secondaryCounter = 1;

            page.Canvas.DrawString($"ZHP, {order.TeamName}",
            trueTypeFont,
            new PdfSolidBrush(Color.Black),
            new PointF(50, 50));

            page.Canvas.DrawString($"{order.Location}, {order.CreationDate.Day}.{order.CreationDate.Month}.{order.CreationDate.Year}",
                trueTypeFont,
                new PdfSolidBrush(Color.Black),
                new PointF(380 - (order.Location.Length * 6), 50));

            page.Canvas.DrawString($"Rozkaz {order.OrderNumber}",
                trueTypeFont,
                new PdfSolidBrush(Color.Black),
                new PointF(200, 80));

            posX = 50;
            posY = 100;

            if (order.Layoffs != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Zwolnienia",
                    trueTypeFont,
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
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                trueTypeFont,
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                trueTypeFont,
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
                    trueTypeFont,
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
                    string text = $"{mainCounter}.{secondaryCounter}.Mianuję dh. {app.ScoutName} {app.ScoutSurname} na funkcję {app.RoleName}.";
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                trueTypeFont,
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                trueTypeFont,
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
                page.Canvas.DrawString($"{mainCounter}.Zamknięcia prób",
                    trueTypeFont,
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (TrialClosing c in order.TrialClosings)
                {
                    string text = $"{mainCounter}.{secondaryCounter}.Zamykam próbę i przyznaję {c.TrialType} {c.TrialName} dh. {c.ScoutName} {c.ScoutSurname}";
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                trueTypeFont,
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                trueTypeFont,
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
                page.Canvas.DrawString($"{mainCounter}.Otwarcia prób",
                    trueTypeFont,
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
                    string text = $"{mainCounter}.{secondaryCounter}.Otwieram próbę na {o.TrialType} {o.TrialName} dh. {o.ScoutName} {o.ScoutSurname}";
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                trueTypeFont,
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                trueTypeFont,
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
                page.Canvas.DrawString($"{mainCounter}.Przyznanie punktów za stopnie i sprawności",
                    trueTypeFont,
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
                    if (c.TrialType == "sprawność")
                    {
                        points = 25;
                        text = $"{mainCounter}.{secondaryCounter}.Za zdobycie {c.TrialType} {c.TrialName} przyznaję dh. {c.ScoutName} {c.ScoutSurname} {points} punktów do współzawodnictwa.";
                    }
                    else if (c.TrialType == "krzyż")
                    {
                        points = 50;
                        text = $"{mainCounter}.{secondaryCounter}.Za zrealizowania próby harcerskiej przyznaję dh. {c.ScoutName} {c.ScoutSurname} {points} punktów do współzawodnictwa.";
                    }
                    else
                    {
                        points = 100;
                        text = $"{mainCounter}.{secondaryCounter}.Za zdobycie {c.TrialType} {c.TrialName} przyznaję dh. {c.ScoutName} {c.ScoutSurname} {points} punktów do współzawodnictwa.";
                    }
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                trueTypeFont,
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                trueTypeFont,
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

            if (order.Games != null)
            {
                page.Canvas.DrawString($"{mainCounter}.Podsumowanie współzawodnictwa",
                    trueTypeFont,
                    new PdfSolidBrush(Color.Black),
                    new PointF(50, posY));

                posY = posY + 20;
                foreach (Game g in order.Games)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    string text = $"{mainCounter}.{secondaryCounter}.Za wyniki w grze {g.GameName} przyznaję dh. {g.Person.Name} {g.Person.Surname} {g.Points} punktów do współzawodnictwa.";
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                trueTypeFont,
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                trueTypeFont,
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
                page.Canvas.DrawString($"{mainCounter}.Pochwały, wyróżnienia i nagany",
                    trueTypeFont,
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
                                trueTypeFont,
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                trueTypeFont,
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
                page.Canvas.DrawString($"{mainCounter}.Skreślenia z listy członków",
                    trueTypeFont,
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
                    string text = $"{mainCounter}.{secondaryCounter}.Z powodu {d.Reason} skreślam dh. {d.ScoutName} {d.ScoutSurname} z listy członków drużyny.";
                    List<string> words = new List<string>();
                    words = text.Split(' ').ToList();
                    foreach (string word in words)
                    {
                        if (posX < 480 - (word.Length * 6))
                        {
                            page.Canvas.DrawString(word,
                                trueTypeFont,
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                trueTypeFont,
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
                    trueTypeFont,
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
                                trueTypeFont,
                                new PdfSolidBrush(Color.Black),
                                new PointF(posX, posY));
                            posX = posX + 7 * (word.Length + 1);
                        }
                        else
                        {
                            posX = 50;
                            posY = posY + 20;
                            page.Canvas.DrawString(word,
                                trueTypeFont,
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
                    trueTypeFont,
                    new PdfSolidBrush(Color.Black),
                    new PointF(350, posY + 50));
            page.Canvas.DrawString($"Drużynowy {order.TeamName}",
                    trueTypeFont,
                    new PdfSolidBrush(Color.Black),
                    new PointF(300, posY + 70));

            doc.SaveToFile($"{order.OrderNumber}.pdf");

        }

        public void GenerateEmptyList(List<Scout> team, Event evnt)
        {
            PdfDocument doc = new PdfDocument();
            PdfPageBase page = doc.Pages.Add();
            PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(new Font(new FontFamily("Arial"), 13, FontStyle.Regular), true);
            int posX = 50;
            int posY = 170;
            int lp = 1;

            page.Canvas.DrawString("Lista obecności",
                trueTypeFont,
                new PdfSolidBrush(Color.Black),
                new PointF(200, 50));

            page.Canvas.DrawString($"Wydarzenie: {evnt.IdEvent} - {evnt.Type}",
                trueTypeFont,
                new PdfSolidBrush(Color.Black),
                new PointF(170, 80));

            page.Canvas.DrawString($"Data: {evnt.DateStartDateNotNullDateEnd}",
                trueTypeFont,
                new PdfSolidBrush(Color.Black),
                new PointF(170, 110));

            foreach (Scout scout in team)
            {
                if (posY > 700)
                {
                    page = doc.Pages.Add();
                    posY = 50;
                    posX = 50;
                }
                page.Canvas.DrawString($"{lp}. ",
                    trueTypeFont,
                    new PdfSolidBrush(Color.Black),
                    new PointF(100, posY));
                page.Canvas.DrawString($"{scout.Surname}",
                    trueTypeFont,
                    new PdfSolidBrush(Color.Black),
                    new PointF(130, posY));
                page.Canvas.DrawString($"{scout.Name}",
                    trueTypeFont,
                    new PdfSolidBrush(Color.Black),
                    new PointF(250, posY));
                page.Canvas.DrawString("|  |",
                    trueTypeFont,
                    new PdfSolidBrush(Color.Black),
                    new PointF(370, posY));

                posY = posY + 30;
                lp = lp + 1;

            }
            doc.SaveToFile($"{evnt.IdEvent}_{evnt.Type}_lista_pusta.pdf");
        }

        public void GenerateEventList(Event ev, List<Scout> team)
        {
            PdfDocument doc = new PdfDocument();
            PdfPageBase page = doc.Pages.Add();
            PdfTrueTypeFont trueTypeFont = new PdfTrueTypeFont(new Font(new FontFamily("Arial"), 13, FontStyle.Regular), true);
            int posX = 50;
            int posY = 170;
            int lp = 1;

            page.Canvas.DrawString("Lista obecności",
                trueTypeFont,
                new PdfSolidBrush(Color.Black),
                new PointF(200, 50));

            page.Canvas.DrawString($"Wydarzenie: {ev.IdEvent} - {ev.Type}",
                trueTypeFont,
                new PdfSolidBrush(Color.Black),
                new PointF(170, 80));

            page.Canvas.DrawString($"Data: {ev.DateStartDateNotNullDateEnd}",
                trueTypeFont,
                new PdfSolidBrush(Color.Black),
                new PointF(170, 110));

            if (team.Count() == 0)
            {
                page.Canvas.DrawString("Na wydarzeniu nie było żadnych harcerzy",
                    trueTypeFont,
                    new PdfSolidBrush(Color.Black),
                    new PointF(150, posY));
            }
            else
            {
                foreach (Scout scout in team)
                {
                    if (posY > 700)
                    {
                        page = doc.Pages.Add();
                        posY = 50;
                        posX = 50;
                    }
                    page.Canvas.DrawString($"{lp}. ",
                        trueTypeFont,
                        new PdfSolidBrush(Color.Black),
                        new PointF(130, posY));
                    page.Canvas.DrawString($"{scout.Surname}",
                        trueTypeFont,
                        new PdfSolidBrush(Color.Black),
                        new PointF(160, posY));
                    page.Canvas.DrawString($"{scout.Name}",
                        trueTypeFont,
                        new PdfSolidBrush(Color.Black),
                        new PointF(280, posY));

                    posY = posY + 30;
                    lp = lp + 1;

                }
            }
            doc.SaveToFile($"{ev.IdEvent}_{ev.Type}_lista.pdf");
        }

    }
}
