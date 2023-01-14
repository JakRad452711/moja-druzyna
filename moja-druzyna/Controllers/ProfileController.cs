using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Data.Session;
using moja_druzyna.Models;
using moja_druzyna.ViewModels.Profile;
using static moja_druzyna.Models.Rank;
using static moja_druzyna.Models.Role;
using static moja_druzyna.Models.ScoutAchievement;
using static moja_druzyna.Models.Achievement;
using static moja_druzyna.Models.ScoutRank;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace moja_druzyna.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ProfileController> _logger;

        private readonly SessionAccesser sessionAccesser;

        private static bool initializedDb = false;

        public ProfileController(ApplicationDbContext dbContext, ILogger<ProfileController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;

            sessionAccesser = new SessionAccesser(dbContext, httpContextAccessor);

            if (!initializedDb)
            {
                ModelManager modelManager = new ModelManager(_dbContext);
                modelManager.InitializeDbValues();
                initializedDb = true;
            }
        }

        public IActionResult PersonalData()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            Scout userData = _dbContext.Scouts.Find(sessionAccesser.UserPesel);
            bool hasAddress = _dbContext.Adresses.Where(a => a.ScoutPeselScout == sessionAccesser.UserPesel).Any();

            if (hasAddress)
            {
                userData.Adress = _dbContext.Adresses.Where(a => a.ScoutPeselScout == sessionAccesser.UserPesel).First();
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

            Rank rank = GetRanks(_dbContext, sessionAccesser.UserPesel, sessionAccesser.UserId);

            ICollection<ScoutRank> ranks = rank.ScoutRanks;

            ICollection<RankViewModel> scoutRanks = new List<RankViewModel>();

            foreach (ScoutRank temp in ranks)
            {

                string temp_rank = temp.RankName;
                string date_aquirement = temp.DateAcquirement.ToString();
                string is_current = temp.IsCurrent.ToString();

                scoutRanks.Add(new RankViewModel()
                {
                    Rank = temp_rank,
                    IsCurrent = is_current,
                    DateAquirement = date_aquirement
                }
                );


            }

            scoutRanks = scoutRanks.OrderBy(x => x.DateAquirement).ToList();

            return View(scoutRanks);
        }

        public IActionResult Achievments()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            var achievements = GetScoutAchievements(_dbContext, sessionAccesser.UserPesel);

            ICollection<AchievementsViewModel> achievementsViewModels = new List<AchievementsViewModel>();
            
            foreach (KeyValuePair<string, string> achievement in achievements)
            {
                achievementsViewModels.Add(new AchievementsViewModel()
                {
                    AchievementDate = achievement.Value,
                    //AchievementType = achievement.Key
                    AchievementType = moja_druzyna.Const.ScoutAbilities.ScoutAbilitiesTranslation[achievement.Key]
                });
            }

            achievementsViewModels = achievementsViewModels.OrderBy(x => x.AchievementDate).ToList();

            return View(achievementsViewModels);
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
