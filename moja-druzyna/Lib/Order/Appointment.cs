using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Models;
using System.Collections.Generic;
using System.Linq;

namespace moja_druzyna.Lib.Order
{
    public class Appointment
    {
        public string ScoutId { get; set; }
        public string ScoutPesel { get; set; }
        public string ScoutName { get; set; }
        public string ScoutSurname { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public string Host { get; set; }

        public bool UpdateDb(ApplicationDbContext dbContext, int teamId, bool execute, ILogger logger)
        {
            bool scoutExistsInTheTeam = dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.ScoutPeselScout == ScoutPesel && scoutTeam.TeamIdTeam == teamId)
                .Count() != 0;
            bool hostHasACaptain = Host != null && dbContext.ScoutHost
                .Where(scoutHost => scoutHost.HostIdHost == int.Parse(Host) && scoutHost.Role == "captain")
                .Count() != 0;
            bool teamHasCaptain = dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.TeamIdTeam == teamId && scoutTeam.Role == "captain")
                .Count() != 0;
            bool scoutIsInTheHost = Host != null && dbContext.ScoutHost
                .Where(scoutHost => scoutHost.ScoutPeselScout == ScoutPesel && scoutHost.HostIdHost == int.Parse(Host))
                .Count() != 0;

            if (!execute || !scoutExistsInTheTeam || (hostHasACaptain && Role == "host captain") || (teamHasCaptain && Role == "captain"))
                return false;
            logger.LogInformation("i am not here");
            Scout scout = dbContext.Scouts.Find(ScoutPesel);
            ScoutTeam scoutTeam = dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.ScoutPeselScout == ScoutPesel && scoutTeam.TeamIdTeam == teamId)
                .First();

            if (Role == "host captain")
            {
                if(scoutIsInTheHost)
                {
                    ScoutHost scoutHost = dbContext.ScoutHost
                    .Where(scoutHost => scoutHost.ScoutPeselScout == ScoutPesel && scoutHost.HostIdHost == int.Parse(Host))
                    .First();

                    dbContext.Remove(scoutHost);
                }

                Host theHost = dbContext.Hosts.Find(int.Parse(Host));

                scoutTeam.Role = "host captain";

                dbContext.ScoutTeam.Update(scoutTeam);
                dbContext.ScoutHost.Add(
                    new ScoutHost()
                    {
                        Role = "captain",
                        Scout = scout,
                        ScoutPeselScout = ScoutPesel,
                        Host = theHost
                    });
                dbContext.SaveChanges();

                return true;
            }
            else if(Role == "captain")
            {
                if(scoutIsInTheHost)
                {
                    ScoutHost scoutHost = dbContext.ScoutHost
                        .Where(scoutHost => scoutHost.ScoutPeselScout == ScoutPesel && scoutHost.HostIdHost == int.Parse(Host))
                        .First();

                    dbContext.ScoutHost.Remove(scoutHost);
                }

                scoutTeam.Role = "captain";

                dbContext.ScoutTeam.Update(scoutTeam);
                dbContext.SaveChanges();

                return true;
            }
            else if(new List<string>() { "vice captain", "ensign", "quatermaster", "chronicler" }.Contains(Role))
            {
                scoutTeam.Role = Role;

                dbContext.ScoutTeam.Update(scoutTeam);
                dbContext.SaveChanges();

                return true;
            }

            return false;
        }
    }
}
