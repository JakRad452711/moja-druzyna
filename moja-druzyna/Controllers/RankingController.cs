using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Data.Session;
using moja_druzyna.Models;
using moja_druzyna.ViewModels.Ranking;
using System.Collections.Generic;
using System.Linq;

namespace moja_druzyna.Controllers
{
    public class RankingController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DocumentsGeneratorsController> _logger;

        private readonly SessionAccesser sessionAccesser;

        public RankingController(ApplicationDbContext dbContext,
            ILogger<DocumentsGeneratorsController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;

            sessionAccesser = new SessionAccesser(dbContext, httpContextAccessor);
        }

        public IActionResult Scores()
        {
            Team team = Team.GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            List<string> scoutPesels = team.GetScouts().Select(s => s.PeselScout).ToList();
            List<ScoresViewModel.ScoreEntry> scores = new();

            foreach(string pesel in scoutPesels)
            {
                Scout scout = Scout.GetScout(_dbContext, pesel);
                List<int> pointsList = _dbContext.Points
                    .Where(p => p.ScoutPeselScout == scout.PeselScout)
                    .Select(p => p.Ammount)
                    .ToList();
                int points = pointsList.Count == 0 ? 0 : pointsList.Sum();

                ScoresViewModel.ScoreEntry scoreEntry = new()
                {
                    Name = scout.Name,
                    Surname = scout.Surname,
                    HostName = team.GetScoutsHost(pesel)?.Name,
                    Rank = scout.GetRank()?.Name,
                    Points = points
                };

                scores.Add(scoreEntry);
            }

            return View(new ScoresViewModel() { Scores = scores.OrderByDescending(s => s.Points).ToList() });
        }
    }
}
