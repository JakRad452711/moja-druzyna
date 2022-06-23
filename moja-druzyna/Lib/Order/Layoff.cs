using moja_druzyna.Data;
using moja_druzyna.Models;
using System.Collections.Generic;
using System.Linq;

namespace moja_druzyna.Lib.Order
{
    public class Layoff
    {
        public string ScoutId { get; set; }
        public string ScoutPesel { get; set; }
        public string ScoutName { get; set; }
        public string ScoutSurname { get; set; }
        public string Role { get; set; }
        public string Host { get; set; }

        public bool UpdateDb(ApplicationDbContext dbContext, int teamId, bool execute)
        {
            bool scoutExistsInTheTeam = dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.ScoutPeselScout == ScoutPesel && scoutTeam.TeamIdTeam == teamId)
                .Count() != 0;
            bool hostHasACaptain = dbContext.ScoutHost
                .Where(scoutHost => scoutHost.HostIdHost == int.Parse(Host) && scoutHost.Role == "captain")
                .Count() != 0;

            if (!execute || !scoutExistsInTheTeam || (!hostHasACaptain && Role == "host captain"))
                return false;

            Scout scout = dbContext.Scouts.Find(ScoutPesel);
            ScoutTeam scoutTeam = dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.ScoutPeselScout == ScoutPesel && scoutTeam.TeamIdTeam == teamId)
                .First();

            if (Role == "host captain")
            {
                ScoutHost scoutHost = dbContext.ScoutHost
                    .Where(scoutHost => scoutHost.ScoutPeselScout == ScoutPesel && scoutHost.HostIdHost == int.Parse(Host))
                    .First();
                Host theHost = dbContext.Hosts.Find(int.Parse(Host));

                scoutTeam.Role = "scout";
                scoutHost.Role = "scout";

                dbContext.ScoutTeam.Update(scoutTeam);
                dbContext.ScoutHost.Update(scoutHost);
                dbContext.SaveChanges();

                return true;
            }
            else if (new List<string>() { "captain", "vice captain", "ensign", "quatermaster", "chronicler" }.Contains(Role))
            {
                scoutTeam.Role = "scout";

                dbContext.ScoutTeam.Update(scoutTeam);
                dbContext.SaveChanges();

                return true;
            }

            return false;
        }
    }
}
