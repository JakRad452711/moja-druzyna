using moja_druzyna.Data;
using moja_druzyna.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moja_druzyna.ViewModels.Team
{
    public class TeamViewModel
    {
        public TeamViewModel(ApplicationDbContext dbContext, int idTeam, List<TeamViewModelEntry> entries)
        {
            Models.Team team = Models.Team.GetTeam(dbContext, idTeam);
            Entries = entries.OrderBy(e => e.Title).ToList();
            string firstScoutPesel = Entries.First().Pesel;

            FirstScout = Scout.GetScout(dbContext, firstScoutPesel);
            FirstScoutTeamRole = team.GetScoutRole(firstScoutPesel);
            FirstScoutHostName = team.GetScoutsHost(firstScoutPesel)?.Name;
            Achievements = new();

            foreach (ScoutRank scoutRank in FirstScout.ScoutRanks)
            {
                Achievements.Add(
                    new ScoutAchievementsViewModel()
                    {
                        Type = Const.TrialTypes.Rank,
                        Rank = scoutRank.RankName,
                        AcquirementTime = scoutRank.DateAcquirement,
                    });
            }

            foreach (ScoutAchievement scoutAchievement in FirstScout.GetScoutAchievements())
            {
                Achievement achievement = dbContext.Achievements.Find(scoutAchievement.AchievementIdAchievement);
                Achievements.Add(
                    new ScoutAchievementsViewModel()
                    {
                        Type = Const.TrialTypes.Ability,
                        Achievement = achievement.Type,
                        AcquirementTime = scoutAchievement.Date
                    });
            }

            Achievements.OrderByDescending(a => a.AcquirementTime);
        }

        public List<TeamViewModelEntry> Entries { get; set; }
        public Scout FirstScout { get; } 
        public List<ScoutAchievementsViewModel> Achievements { get; set; }
        public string FirstScoutTeamRole { get; }
        public string FirstScoutHostName { get; }

        public class TeamViewModelEntry
        {
            public string Id { get; set; }
            public string Pesel { get; set; }
            public string Rank { get; set; }
            public string Title { get; set; }
            public string Host { get; set; }
        }

        public class ScoutAchievementsViewModel
        {
            public string Achievement { get; set; }
            public string Type { get; set; }
            public string Rank { get; set; }
            public DateTime AcquirementTime { get; set; }
            public string OrderNumber { get; set; }
            public string Team { get; set; }
        }
    }
}
