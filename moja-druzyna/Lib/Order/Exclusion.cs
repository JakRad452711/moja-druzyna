using moja_druzyna.Data;
using moja_druzyna.Models;
using System.Collections.Generic;
using System.Linq;

namespace moja_druzyna.Lib.Order
{
    public class Exclusion
    {
        public string ScoutId { get; set; }
        public string ScoutPesel { get; set; }
        public string ScoutName { get; set; }
        public string ScoutSurname { get; set; }
        public string Reason { get; set; }

        public bool UpdateDb(ApplicationDbContext dbContext, int teamId, bool execute)
        {
            bool scoutExistsInTheTeam = dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.ScoutPeselScout == ScoutPesel && scoutTeam.TeamIdTeam == teamId)
                .Count() != 0;
            bool scoutIsTheTeamCaptain = dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.ScoutPeselScout == ScoutPesel && scoutTeam.TeamIdTeam == teamId && scoutTeam.Role == "captain")
                .Count() != 0;

            if (!execute || !scoutExistsInTheTeam || scoutIsTheTeamCaptain)
                return false;

            Scout scout = dbContext.Scouts.Find(ScoutPesel);

            ScoutTeam scoutTeam = dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.ScoutPeselScout == ScoutPesel)
                .First();
            Parent parent = dbContext.Parents.Find(ScoutPesel);
            List<ScoutHost> scoutHosts = dbContext.ScoutHost
                .Where(scoutHost => scoutHost.ScoutPeselScout == ScoutPesel)
                .ToList();
            List<ScoutAchievement> scoutAchievements = dbContext.ScoutAchievements
                .Where(scoutHost => scoutHost.ScoutPeselScout == ScoutPesel)
                .ToList();
            List<ScoutRank> scoutRanks = dbContext.ScoutRanks
                .Where(scoutHost => scoutHost.ScoutPeselScout == ScoutPesel)
                .ToList();

            dbContext.Scouts.Remove(scout);
            dbContext.ScoutHost.RemoveRange(scoutHosts);
            dbContext.ScoutAchievements.RemoveRange(scoutAchievements);
            dbContext.ScoutRanks.RemoveRange(scoutRanks);
            dbContext.Parents.Remove(parent);
            dbContext.SaveChanges();

            return true;
        }
    }
}
