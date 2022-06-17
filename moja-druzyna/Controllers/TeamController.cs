using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Data.Session;
using moja_druzyna.Models;
using moja_druzyna.ViewModels.Team;
using System.Collections.Generic;
using System.Linq;

namespace moja_druzyna.Controllers
{
    public class TeamController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TeamController> _logger;
        UserManager<IdentityUser> _userManager;

        private readonly SessionAccesser sessionAccesser;
        private readonly ModelManager modelManager;

        private static bool scoutWasAdded = false;

        public TeamController(ApplicationDbContext applicationDbContext, ILogger<TeamController> logger, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _dbContext = applicationDbContext;
            _logger = logger;
            _userManager = userManager;

            sessionAccesser = new SessionAccesser(applicationDbContext, httpContextAccessor);
            modelManager    = new ModelManager(applicationDbContext);
        }

        public IActionResult Team()
        {
            ICollection<TeamViewModel> scoutsInfo = new List<TeamViewModel>();

            int currentTeamId = sessionAccesser.CurrentTeamId;

            foreach (Scout scout in _dbContext.ScoutTeam.Where(scoutTeam => scoutTeam.TeamIdTeam == currentTeamId).Select(_scoutTeam => _scoutTeam.Scout))
            {
                string id = scout.IdentityId;
                string title = string.Format("{0} {1}", scout.Surname, scout.Name);
                string pesel = scout.PeselScout;
                string host = "nazwa zastępu";
                scoutsInfo.Add(new TeamViewModel() { Id = id, Title = title, Host = host, Pesel = pesel });
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
            if (ModelState.IsValid)
            {
                Scout addedScout = new Scout
                {
                    PeselScout       = addScoutViewModel.Pesel,
                    Name             = addScoutViewModel.Name,
                    Surname          = addScoutViewModel.Surname,
                    SecondName       = addScoutViewModel.SecondName,
                    MembershipNumber = addScoutViewModel.MembershipNumber,
                    Nationality      = addScoutViewModel.Nationality,
                    Ns               = addScoutViewModel.Ns
                };

                var user = new IdentityUser { UserName = addedScout.PeselScout, Email = null };
                _userManager.CreateAsync(user, "");

                addedScout.Identity = user;
                addedScout.IdentityId = user.Id;

                modelManager.CreateScoutAccount(sessionAccesser.CurrentTeamId, addedScout);

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

            editedScout.Adress = _dbContext.Adresses
                .Where(address => address.ScoutPeselScout == editedScout.PeselScout)
                .First();

            ViewBag.scoutWasEdited = false;
            ViewBag.scoutEditionFailed = false;

            return View(editedScout);
        }

        [HttpPost]
        public IActionResult EditScout(Scout scout)
        {
            if (ModelState.IsValid)
            {
                modelManager.EditScout(scout);

                ViewBag.scoutWasEdited = true;
                ViewBag.scoutEditionFailed = false;

                return View(scout);
            }

            ViewBag.scoutWasEdited = false;
            ViewBag.scoutEditionFailed = true;

            return View(scout);
        }
    }
}
