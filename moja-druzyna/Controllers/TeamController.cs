using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Models;
using moja_druzyna.ViewModels.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moja_druzyna.Controllers
{
    public class TeamController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TeamController> _logger;

        private static bool scoutWasAdded = false;

        public TeamController(ApplicationDbContext applicationDbContext, ILogger<TeamController> logger)
        {
            _dbContext = applicationDbContext;
            _logger = logger;
        }

        public IActionResult Team()
        {
            ICollection<TeamViewModel> scoutsInfo = new List<TeamViewModel>();

            foreach(Scout scout in _dbContext.Scouts)
            {
                string id    = scout.IdentityId;
                string title = string.Format("{0} {1}", scout.Surname, scout.Name);
                string host  = "nazwa zastępu";
                scoutsInfo.Add(new TeamViewModel() { Id=id, Title=title, Host=host });
            }

            scoutsInfo = scoutsInfo.OrderBy(info => info.Title).ToList();

            return View(scoutsInfo);
        }

        public IActionResult AddScout()
        {
            ViewBag.scoutWasAdded = scoutWasAdded;
            scoutWasAdded = false;

            return View();
        }
        
        [HttpPost]
        public IActionResult AddScout(AddScoutViewModel addScoutViewModel)
        {
            if(ModelState.IsValid)
            {
                scoutWasAdded = true;

                return Redirect("addscout");
            }

            scoutWasAdded = false;

            return View();
        }

        public IActionResult EditScout(string scoutId)
        {
            Scout editedScout = _dbContext.Scouts
                .Where(scout => scout.IdentityId == scoutId)
                .First();

            ViewBag.scoutWasEdited     = false;
            ViewBag.scoutEditionFailed = false;

            return View(editedScout);
        }

        [HttpPost]
        public IActionResult EditScout(Scout scout)
        {
            if(ModelState.IsValid)
            {
                ViewBag.scoutWasEdited     = true;
                ViewBag.scoutEditionFailed = false;
                return View(scout);
            }

            ViewBag.scoutWasEdited     = false;
            ViewBag.scoutEditionFailed = true;

            return View(scout);
        }
    }
}
