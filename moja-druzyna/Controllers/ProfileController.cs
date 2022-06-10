using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Models;
using moja_druzyna.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace moja_druzyna.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ProfileController> _logger;

        private static AFormViewModel aFormViewModel = new AFormViewModel();

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

        [HttpGet]
        public IActionResult AForm()
        {
            List<Scout> unselectedScouts = _dbContext.Scouts.Where(scout => !aFormViewModel.AForm_Scouts.Select(scout => scout.Id).ToList().Contains(scout.Pesel)).ToList<Scout>();

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();

            foreach (var scout in unselectedScouts)
                dropDownList_Scouts.Add(new SelectListItem { Value = scout.Pesel, Text = string.Format("{0} {1}\t({2})", scout.Name, scout.Surname, scout.Pesel) });

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;
            ViewBag.Scouts = aFormViewModel.AForm_Scouts;

            return View(aFormViewModel);
        }

        [HttpPost]
        public IActionResult AForm_Submit(AFormViewModel _aFormViewModel)
        {
            aFormViewModel = _aFormViewModel;

            throw new NotImplementedException();

            return Redirect("AForm");
        }

        [HttpPost]
        public IActionResult AForm_AddScout(AFormViewModel _aFormViewModel, string scoutId)
        {
            aFormViewModel = _aFormViewModel;

            Scout addedScout = _dbContext.Scouts.Find(scoutId);

            aFormViewModel.AddScout(addedScout);

            return Redirect("AForm");
        }

        [HttpPost]
        public IActionResult AForm_RemoveScout(AFormViewModel _aFormViewModel, int scoutIndex)
        {
            aFormViewModel = _aFormViewModel;
            aFormViewModel.AForm_Scouts.RemoveAt(scoutIndex);

            return Redirect("AForm");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
