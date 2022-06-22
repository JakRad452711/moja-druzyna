using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Data.Session;
using moja_druzyna.Models;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System;
using moja_druzyna.src;

namespace moja_druzyna.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ProfileController> _logger;

        private readonly SessionAccesser sessionAccesser;

        public ProfileController(ApplicationDbContext dbContext, ILogger<ProfileController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;

            sessionAccesser = new SessionAccesser(dbContext, httpContextAccessor);
        }

        public IActionResult PersonalData()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            Scout userData = _dbContext.Scouts.Find(sessionAccesser.UserPesel);

            return View(userData);
        }

        public IActionResult ServiceHistory()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            List<string> data = new List<string>();
            data.Add("22222222222");
            data.Add("98061485535");
            data.Add("03313088369");
            data.Add("01111111111");
            data.Add("123456");
            data.Add("XD123");
            data.Add("abcdefghijk");

            Test_Pesel test1 = new Test_Pesel();
            foreach (string test in data)
            {
                _logger.LogInformation(test1.testPesel(test));
            }

            return View();
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
