using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Data.Session;
using moja_druzyna.Models;
using moja_druzyna.ViewModels;
using System.Diagnostics;
using System.Linq;

namespace moja_druzyna.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ProfileController> _logger;

        private readonly SessionAccesser sessionAccesser;

        private static AFormViewModel aFormViewModel = new AFormViewModel();

        public ProfileController(ApplicationDbContext dbContext, ILogger<ProfileController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;

            sessionAccesser = new SessionAccesser(dbContext, httpContextAccessor);
        }

        public IActionResult PersonalData()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            Scout userData = _dbContext.Scouts.Find(sessionAccesser.UserPesel);
            bool hasAddress = _dbContext.Adresses.Where(address => address.ScoutPeselScout == sessionAccesser.UserPesel).Any();

            if(hasAddress)
            {
                userData.Adress = _dbContext.Adresses.Where(address => address.ScoutPeselScout == sessionAccesser.UserPesel).First();
            }

            return View(userData);
        }

        public IActionResult ServiceHistory()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View();
        }

        public IActionResult Ranks()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View();
        }

        public IActionResult Achievments()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View();
        }

        public IActionResult Roles()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View();
        }

        public IActionResult CoursesAndPermissions()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View();
        }

        public IActionResult GdprConsents()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View();
        }

        public IActionResult Privacy()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
