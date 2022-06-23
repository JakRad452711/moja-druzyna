using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace moja_druzyna.Lib.Order
{
    public class TrialClosing
    {
        public string ScoutId { get; set; }
        public string ScoutPesel { get; set; }
        public string ScoutName { get; set; }
        public string ScoutSurname { get; set; }
        public string TrialType { get; set; }
        public string Rank { get; set; }
        public string Ability { get; set; }

        public bool UpdateDb(ApplicationDbContext dbContext, int teamId, bool execute)
        {
            bool scoutExistsInTheTeam = dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.ScoutPeselScout == ScoutPesel && scoutTeam.TeamIdTeam == teamId)
                .Count() != 0;
            bool rankExists = dbContext.Ranks
                .Where(rank => rank.Name == Rank)
                .Count() != 0;
            bool scoutHasTheRank = dbContext.ScoutRanks
                .Where(scoutRank => scoutRank.RankName == Rank && scoutRank.ScoutPeselScout == ScoutPesel && scoutRank.IsCurrent)
                .Count() != 0;
            bool scoutHadTheRank = dbContext.ScoutRanks
                .Where(scoutRank => scoutRank.RankName == Rank && scoutRank.ScoutPeselScout == ScoutPesel && !scoutRank.IsCurrent)
                .Count() != 0;
            bool abilityExists = dbContext.Achievements
                .Where(achievement => achievement.IdAchievement == int.Parse(Ability))
                .Count() != 0;
            bool scoutHasTheAbility = dbContext.ScoutAchievements
                .Where(scoutAchievement => scoutAchievement.AchievementIdAchievement == int.Parse(Ability) && scoutAchievement.ScoutPeselScout == ScoutPesel)
                .Count() != 0;

            if (!execute || !scoutExistsInTheTeam || ((!rankExists || scoutHasTheRank) && TrialType == "rank") || ((!abilityExists || scoutHasTheAbility) && TrialType == "ability"))
                return false;

            Scout scout = dbContext.Scouts.Find(ScoutPesel);

            if (TrialType == "rank" && scoutHadTheRank)
            {
                Rank rank = dbContext.Ranks.Find(Rank);
                ScoutRank scoutRankCurrent = dbContext.ScoutRanks
                    .Where(scoutRank => scoutRank.RankName == Rank && scoutRank.ScoutPeselScout == ScoutPesel)
                    .First();
                ScoutRank scoutRankNew = dbContext.ScoutRanks
                    .Where(scoutRank => scoutRank.RankName == Rank && scoutRank.ScoutPeselScout == ScoutPesel)
                    .First();

                scoutRankCurrent.IsCurrent = false;
                scoutRankNew.IsCurrent = true;

                dbContext.ScoutRanks.Update(scoutRankCurrent);
                dbContext.ScoutRanks.Update(scoutRankNew);
                dbContext.SaveChanges();

                return true;
            }
            else if(TrialType == "rank")
            {
                Rank rank = dbContext.Ranks.Find(Rank);
                List<ScoutRank> scoutRanks = dbContext.ScoutRanks
                    .Where(scoutRank => scoutRank.RankName == Rank && scoutRank.ScoutPeselScout == ScoutPesel)
                    .ToList();
                ScoutRank scoutRank = new ScoutRank()
                {
                    Rank = rank,
                    RankName = Rank,
                    Scout = scout,
                    ScoutPeselScout = ScoutPesel,
                    DateAcquirement = DateTime.Now,
                    IsCurrent = true
                };

                scoutRanks.ForEach(rank => rank.IsCurrent = false);

                dbContext.ScoutRanks.UpdateRange(scoutRanks);
                dbContext.ScoutRanks.Add(scoutRank);
                dbContext.SaveChanges();

                return true;
            }
            else if(TrialType == "ability")
            {
                Achievement achievement = dbContext.Achievements.Find(int.Parse(Ability));
                ScoutAchievement scoutAchievement = new ScoutAchievement()
                {
                    Scout = scout,
                    ScoutPeselScout = ScoutPesel,
                    Achievement = achievement,
                    Date = DateTime.Now
                };
                
                dbContext.ScoutAchievements.Add(scoutAchievement);
                dbContext.SaveChanges();

                return true;
            }

            return false;
        }
    }
}
