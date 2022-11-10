using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using moja_druzyna.Const;
using moja_druzyna.Data;
using moja_druzyna.Data.Session;
using moja_druzyna.Models;
using moja_druzyna.ViewModels.Team;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace moja_druzyna.Controllers
{
#warning need to verify if user is authorized to make the api request in each action
    public class ApiController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TeamController> _logger;

        private readonly SessionAccesser sessionAccesser;

        public ApiController(ApplicationDbContext dbContext, ILogger<TeamController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;

            sessionAccesser = new SessionAccesser(dbContext, httpContextAccessor);
        }

        [HttpGet]
        public JsonResult GetIsSidebarCollapsed()
        {
            bool isSidebarCollapsed = sessionAccesser.IsSidebarCollapsed;

            TempData["isSidebarCollapsed"] = isSidebarCollapsed;

            return new JsonResult(Ok(isSidebarCollapsed));
        }

        [HttpPost]
        public JsonResult ToggleIsSidebarCollapsed()
        {
            bool isSidebarCollapsed = sessionAccesser.IsSidebarCollapsed;

            sessionAccesser.IsSidebarCollapsed = !isSidebarCollapsed;
            TempData["isSidebarCollapsed"] = !isSidebarCollapsed;

            return new JsonResult(Ok());
        }

        [HttpGet]
        public JsonResult GetScoutJson(string id)
        {
            Scout scout = Scout.GetScoutById(_dbContext, id).GetShallowCopy();

            string scoutStr = JsonConvert.SerializeObject(scout);

            return new JsonResult(Ok(scoutStr));
        }

        [HttpGet]
        public JsonResult GetScoutRanksJson(string id)
        {
            Scout scout = Scout.GetScoutById(_dbContext, id);
            List<string> ranks = scout.ScoutRanks.OrderByDescending(sr => sr.RankName).Select(sr => sr.RankName).ToList();

            string ranksStr = JsonConvert.SerializeObject(ranks);

            return new JsonResult(Ok(ranksStr));
        }

        [HttpGet]
        public JsonResult GetScoutRoleJsonPL(string id, string teamId)
        {
            Scout scout = Scout.GetScoutById(_dbContext, id).GetShallowCopy();
            Team team = Team.GetTeam(_dbContext, int.Parse(teamId));

            string role = team.GetScoutRole(scout.PeselScout);
            string rolePL = null;

            if (TeamRoles.TeamRolesList.Contains(role) && role != TeamRoles.Scout)
                rolePL = TeamRoles.TeamRolesTranslationsWithPolishLetters[role];

            return new JsonResult(Ok(rolePL));
        }

        [HttpGet]
        public JsonResult GetScoutHostJson(string id, string teamId)
        {
            Scout scout = Scout.GetScoutById(_dbContext, id);
            Team team = Team.GetTeam(_dbContext, int.Parse(teamId));

            Host host = team.GetScoutsHost(scout.PeselScout)?.GetShallowCopy();
            string hostStr = JsonConvert.SerializeObject(host);

            return new JsonResult(Ok(hostStr));
        }

        public JsonResult GetAchievementsAndRankAchievementsJsonPL(string id)
        {
            Scout scout = Scout.GetScoutById(_dbContext, id);

            List<ScoutAchievementsViewModel> achievements = new();

            foreach (ScoutRank scoutRank in scout.ScoutRanks)
            {
                achievements.Add(
                    new ScoutAchievementsViewModel()
                    {
                        Type = TrialTypes.Rank,
                        Rank = scoutRank.RankName,
                        AcquirementTime = scoutRank.DateAcquirement,
                    });
            }

            foreach (ScoutAchievement scoutAchievement in scout.GetScoutAchievements())
            {
                Achievement achievement = _dbContext.Achievements.Find(scoutAchievement.AchievementIdAchievement);
                achievements.Add(
                    new ScoutAchievementsViewModel()
                    {
                        Type = TrialTypes.Ability,
                        Achievement = ScoutAbilities.ScoutAbilitiesTranslationWithPolishLetters[achievement.Type],
                        AcquirementTime = scoutAchievement.Date
                    });
            }

            achievements.OrderByDescending(a => a.AcquirementTime);
            
            string achievementsStr = JsonConvert.SerializeObject(achievements);

            return new JsonResult(Ok(achievementsStr));
        }
    }
}
