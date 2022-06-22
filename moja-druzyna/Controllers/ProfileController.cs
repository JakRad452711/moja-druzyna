﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Models;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Drawing;
using moja_druzyna.src;

namespace moja_druzyna.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ApplicationDbContext dbContext, ILogger<ProfileController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IActionResult PersonalData()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            Scout userData = _dbContext.Scouts.First();

            return View(userData);
        }

        public IActionResult ServiceHistory()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            // Team teamData = _dbContext.Teams.Where(x => x.TeamId == 2).First();
            /*        
            var interactionData = _dbContext.ScoutTeams;
            
            List<ScoutTeam> interactions = new List<ScoutTeam>();
            foreach (var team in interactionData)
            {
                interactions.Add(team);
            }

            return View(interactions);
            */
            Scout interactionData = _dbContext.Scouts.First();
            GeneratorPdf generatorPdf = new GeneratorPdf();
            //generatorPdf.CreateTestPdf();
            Releasing r1 = new Releasing(interactionData, "zastępowy", "Wilki");
            Releasing r2 = new Releasing(interactionData, "kronikarz", "kadra");
            Appointment a1 = new Appointment(interactionData, "przyboczny", "kadra");
            Appointment a2 = new Appointment(interactionData, "zastępowy", "Wiewiórki");
            List<Releasing> rel = new List<Releasing>();
            List<Appointment> app = new List<Appointment>();
            rel.Add(r1);
            rel.Add(r2);
            app.Add(a1);
            app.Add(a2);
            Order order = new Order("L1-2022", "88 PDHS Dragon", "22.06.2022", "Poznań", rel, app);
            generatorPdf.GenerateOrder(order);
            return View(interactionData);
        }

        public IActionResult Ranks()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            return View();
        }

        public IActionResult Achievments()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            return View();
        }

        public IActionResult Roles()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            return View();
        }

        public IActionResult CoursesAndPermissions()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            return View();
        }

        public IActionResult GdprConsents()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            return View();
        }

        public IActionResult Privacy()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
