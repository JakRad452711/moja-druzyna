using Microsoft.EntityFrameworkCore;
using moja_druzyna.Const;
using moja_druzyna.Data;
using moja_druzyna.Lib.Exceptions;
using moja_druzyna.Lib.Order;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static moja_druzyna.Models.Host;
using static moja_druzyna.Models.Scout;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Team
    {
        private ApplicationDbContext _dbContext;

        public Team()
        {
            this.AttendanceLists = new List<AttendanceList>();
            this.EventTeams = new List<EventTeam>();
            this.Hosts = new List<Host>();
            this.ScoutTeam = new List<ScoutTeam>();
        }

        [MaxLength(150)]
        public string Name { get; set; }
        [Key]
        public int IdTeam { get; set; }

        public virtual ICollection<AttendanceList> AttendanceLists { get; set; }
        public virtual ICollection<EventTeam> EventTeams { get; set; }
        public virtual ICollection<Host> Hosts { get; set; }
        public virtual ICollection<ScoutTeam> ScoutTeam { get; set; }

        public static Team GetTeam(ApplicationDbContext dbContext, int teamId)
        {
            if (dbContext == null)
                throw new System.ArgumentNullException();

            bool teamExists = dbContext.Teams.Find(teamId) != null;

            if (!teamExists)
                throw new RecordNotFoundException(string.Format("Team with id '{0}' doesn't exist", teamId));

            Team team = dbContext.Teams.Where(t => t.IdTeam == teamId)
                .Include(t => t.AttendanceLists)
                .Include(t => t.EventTeams)
                .Include(t => t.Hosts)
                .Include(t => t.ScoutTeam)
                .First();

            team._dbContext = dbContext;

            return team;
        }

        public void AddOrder(FormOrder formOrder, string creatorPesel, string creationLocation)
        {
            CheckDbContextInstance("void AddOrder(FormOrder formOrder, string creatorPesel, string creationLocation)");

            string formOrderJSON = JsonConvert.SerializeObject(formOrder);

            Order order = new Order() { Contents = formOrderJSON };
            OrderInfo orderInfo = new OrderInfo()
            {
                Order = order,
                Scout = _dbContext.Scouts.Find(creatorPesel),
                ScoutPeselScout = creatorPesel,
                Name = formOrder.OrderNumber,
                Team = this,
                TeamIdTeam = this.IdTeam,
                Location = creationLocation,
                CreationDate = DateTime.Now
            };

            _dbContext.Orders.Add(order);
            _dbContext.OrderInfos.Add(orderInfo);
            _dbContext.SaveChanges();
        }

        public bool ScoutHasOneOfRoles(string scoutPesel, List<string> roles)
        {
            CheckDbContextInstance("bool ScoutHasOneOfRoles(string scoutPesel, List<string> roles)");

            ScoutTeam scoutTeam = ScoutTeam.FirstOrDefault(st => st.ScoutPeselScout == scoutPesel);

            if (scoutTeam == null)
                return false;

            return roles.Contains(scoutTeam.Role);
        }

        public void CreateScout(Scout scout)
        {
            CheckDbContextInstance("void CreateScout(Scout scout)");

            if (_dbContext.Scouts.Find(scout.PeselScout) != null)
                throw new ArgumentException("'void CreateScout(Scout scout)': scout with such primary key already exists");

            Team team = _dbContext.Teams.Find(this.IdTeam);
            ScoutTeam scoutTeam = new() { Role = TeamRoles.Scout, Scout = scout, ScoutPeselScout = scout.PeselScout, Team = team, TeamIdTeam = team.IdTeam };
            Parent parent = scout.Parent;
            Address address = scout.Adress;

            if (parent == null)
                parent = new Parent() { Pesel = scout.PeselScout, Scouts = new List<Scout>() { scout } };

            if (address == null)
                address = new Address() { Scout = scout, ScoutPeselScout = scout.PeselScout, Parent = parent, ParentPesel = parent.Pesel };

            scout.Adress = address;

            _dbContext.Scouts.Add(scout);
            _dbContext.ScoutTeam.Add(scoutTeam);
            _dbContext.Adresses.Add(address);
            _dbContext.Parents.Add(parent);
            _dbContext.SaveChanges();
        }

        public void CreateHost(Host host, string hostCaptainPesel)
        {
            CheckDbContextInstance("void CreateHost(Host host, string hostCaptainPesel)");

            if (!ScoutHasOneOfRoles(hostCaptainPesel, new() { TeamRoles.Scout }))
                throw new ArgumentException("void CreateHost(Host host, string hostCaptainPesel): scout must have no special role in team to become a host captain");

            Scout hostCaptain = GetScout(_dbContext, hostCaptainPesel);

            host.Team = this;
            host.TeamIdTeam = this.IdTeam;

            _dbContext.Hosts.Add(host);
            _dbContext.SaveChanges();

            host.IdHost = _dbContext.Hosts.First(h => h.Name == host.Name).IdHost;

            ScoutHost scoutHost = new ScoutHost()
            {
                Host = host,
                HostIdHost = host.IdHost,
                Scout = hostCaptain,
                ScoutPeselScout = hostCaptain.PeselScout,
                Role = HostRoles.HostCaptain
            };

            ScoutTeam scoutTeam = _dbContext.ScoutTeam
                .Where(st => st.ScoutPeselScout == hostCaptain.PeselScout && st.TeamIdTeam == IdTeam)
                .First();

            Hosts.Add(host);
            ScoutTeam.Add(scoutTeam);
            hostCaptain.ScoutHost.Add(scoutHost);
            host.ScoutHost.Add(scoutHost);
            scoutTeam.Role = TeamRoles.HostCaptain;

            _dbContext.Scouts.Update(hostCaptain);
            _dbContext.ScoutHost.Add(scoutHost);
            _dbContext.ScoutTeam.Update(scoutTeam);
            _dbContext.SaveChanges();
        }

        public void Appoint(Appointment appointment)
        {
            CheckDbContextInstance("void Appoint(Appointment appointment)");

            string scoutRole = GetScoutRole(appointment.ScoutPesel);

            bool scoutIsCaptain = ScoutHasOneOfRoles(appointment.ScoutPesel, new() { TeamRoles.Captain });
            bool scoutIsNotInTheTeam = scoutRole == null;

            if (scoutIsNotInTheTeam)
                throw new UnauthorizedAccessException("void Appoint(Appointment appointment): the scouts pesel belongs to a scout not from this team");

#warning temporarily hard locked changing team captains role
            if (scoutIsCaptain || appointment.Role == TeamRoles.Captain)
                throw new UnauthorizedAccessException("void Appoint(Appointment appointment): can't appoint team captain or to a team captain role");

            appointment.ScoutId = _dbContext.Scouts.Find(appointment.ScoutPesel).IdentityId;

            if (!TeamRoles.TeamRolesList.Contains(appointment.Role))
                throw new ArgumentException("void Layoff(Layoff layoff): illegal appointment.Role in 'Appointment appointment' argument");

            if (appointment.Role == TeamRoles.HostCaptain)
            {
                Host oldHost = GetScoutsHost(appointment.ScoutPesel);

                if (oldHost != null)
                    oldHost.RemoveScout(appointment.ScoutPesel);

                Host host = GetHost(_dbContext, int.Parse(appointment.Host));

                host.AddScout(appointment.ScoutPesel);
                host.UpdateScoutRole(appointment.ScoutPesel, HostRoles.HostCaptain);
            }
            else
            {
                Host host = GetScoutsHost(appointment.ScoutPesel);

                if (host != null)
                    host.UpdateScoutRole(appointment.ScoutPesel, HostRoles.Scout);
            }

            UpdateScoutRole(appointment.ScoutPesel, appointment.Role);
        }

#warning how should layoff work when layoff role doesn't match scouts role in a team?
        public void Layoff(Layoff layoff)
        {
            CheckDbContextInstance("void Layoff(Layoff layoff)");

            string scoutRole = GetScoutRole(layoff.ScoutPesel);

            bool scoutIsCaptain = ScoutHasOneOfRoles(layoff.ScoutPesel, new() { TeamRoles.Captain });
            bool scoutIsNotInTheTeam = scoutRole == null;

            if (scoutIsNotInTheTeam)
                throw new UnauthorizedAccessException("void Layoff(Layoff layoff): the scouts pesel belongs to a scout not from this team");

#warning temporarily hard locked changing team captains role
            if (scoutIsCaptain)
                throw new UnauthorizedAccessException("void Layoff(Layoff layoff): can't layoff team captain or to a team captain role");

            if (scoutRole != layoff.Role)
                throw new LayoffRoleMismatchException("void Layoff(Layoff layoff): layoff.Role doesn't match scouts role in the team");

            layoff.ScoutId = _dbContext.Scouts.Find(layoff.ScoutPesel).IdentityId;

            if (!TeamRoles.TeamRolesList.Contains(layoff.Role))
                throw new ArgumentException("void Layoff(Layoff layoff): illegal layoff.Role in 'Layoff layoff' argument");

            if (layoff.Role == TeamRoles.HostCaptain)
            {
                Host host = GetHost(_dbContext, int.Parse(layoff.Host));

                try
                {
                    host.UpdateScoutRole(layoff.ScoutPesel, HostRoles.Scout);
                }
                catch(UnauthorizedAccessException)
                {
                }
            }

            UpdateScoutRole(layoff.ScoutPesel, TeamRoles.Scout);
        }

        public void CloseATrial(TrialClosing trialClosing)
        {
            CheckDbContextInstance("void CloseATrial(TrialClosing trialClosing)");

            Scout scout = GetScout(_dbContext, trialClosing.ScoutPesel);

            if (!GetScouts().Select(s => s.PeselScout).Contains(trialClosing.ScoutPesel))
                throw new UnauthorizedAccessException("void CloseATrial(TrialClosing trialClosing): the scouts pesel belongs to a scout not from this team");

            trialClosing.ScoutId = _dbContext.Scouts.Find(trialClosing.ScoutPesel).IdentityId;

            if (!TrialTypes.TrialTypesList.Contains(trialClosing.TrialType))
                throw new ArgumentException("void CloseATrial(TrialClosing trialClosing): illegal trial type in 'Layoff layoff' argument");

            if (trialClosing.TrialType == TrialTypes.Rank)
            {
                List<ScoutRank> scoutRanks = scout.ScoutRanks.ToList();

                scoutRanks.ForEach(sr => sr.IsCurrent = false);

                _dbContext.ScoutRanks.UpdateRange(scoutRanks);
                _dbContext.SaveChanges();

                if (scoutRanks.Select(sr => sr.RankName).Contains(trialClosing.Rank))
                {
                    ScoutRank scoutRank = scoutRanks.First(sr => sr.RankName == trialClosing.Rank);
                    scoutRank.IsCurrent = true;

                    _dbContext.ScoutRanks.Update(scoutRank);
                }
                else
                {
                    ScoutRank scoutRank = new ScoutRank()
                    {
                        IsCurrent = true,
                        DateAcquirement = DateTime.Now,
                        Rank = _dbContext.Ranks.Find(trialClosing.Rank),
                        RankName = trialClosing.Rank,
                        Scout = scout,
                        ScoutPeselScout = scout.PeselScout
                    };

                    _dbContext.ScoutRanks.Add(scoutRank);
                }

                _dbContext.SaveChanges();
            }
            else if (trialClosing.TrialType == TrialTypes.Ability)
            {
                Achievement achievement = _dbContext.Achievements.Find(int.Parse(trialClosing.Ability));
                ScoutAchievement scoutAchievement = new ScoutAchievement()
                {
                    Scout = scout,
                    ScoutPeselScout = trialClosing.ScoutPesel,
                    Achievement = achievement,
                    AchievementIdAchievement = achievement.IdAchievement,
                    Date = DateTime.Now
                };

                if (_dbContext.ScoutAchievements.Find(trialClosing.ScoutPesel, achievement.IdAchievement) != null)
                    return;

                _dbContext.ScoutAchievements.Add(scoutAchievement);
                _dbContext.SaveChanges();
            }
        }

        public string GetScoutRole(string scoutPesel)
        {
            if (ScoutTeam == null)
                throw new NullReferenceException("string GetScoutRole(string scoutPesel): ScoutTeam variable is null");

            ScoutTeam scoutTeam = ScoutTeam.FirstOrDefault(st => st.ScoutPeselScout == scoutPesel);

            if (scoutTeam == null)
                return null;

            return scoutTeam.Role;
        }

        public List<Scout> GetScoutsThatDoNotHaveAHost()
        {
            CheckDbContextInstance("List<Scout> GetScoutsThatDoNotHaveAHost()");

            List<Scout> scouts = GetScouts();
            List<int> hostIdsInTheTeam = Hosts.Select(h => h.IdHost).ToList();
            List<string> scoutPeselsInTheTeam = scouts.Select(s => s.PeselScout)
                .ToList();

            List<string> peselsOfScoutsThatAreInAHost = _dbContext.ScoutHost
                .Where(sh => scoutPeselsInTheTeam.Contains(sh.ScoutPeselScout) && hostIdsInTheTeam.Contains(sh.HostIdHost))
                .Select(sh => sh.ScoutPeselScout)
                .ToList();

            List<Scout> scoutsThatAreNotInAnyHostFromTheTeam = scouts
                .Where(s => !peselsOfScoutsThatAreInAHost.Contains(s.PeselScout))
                .ToList();

            return scoutsThatAreNotInAnyHostFromTheTeam;
        }

        public List<Scout> GetScouts()
        {
            CheckDbContextInstance("List<Scout> GetScouts()");

            return _dbContext.ScoutTeam.Where(st => st.TeamIdTeam == this.IdTeam).Select(st => st.Scout).ToList();
        }

        public Host GetScoutsHost(string scoutPesel)
        {
            List<int> idsOfHostsInTheTeam = Hosts.Select(h => h.IdHost).ToList();

            ScoutHost scoutHost = _dbContext.ScoutHost.Where(sh => idsOfHostsInTheTeam.Contains(sh.HostIdHost) && sh.ScoutPeselScout == scoutPesel)
                .Include(sh => sh.Host)
                .FirstOrDefault();

            if (scoutHost == null)
                return null;

            return GetHost(_dbContext, scoutHost.Host.IdHost);
        }

        public void UpdateName(string name)
        {
            CheckDbContextInstance("void UpdateName(string name)");

            if (name == null)
                return;

            this.Name = name;

            _dbContext.Teams.Update(this);
            _dbContext.SaveChanges();
        }

        public void UpdateScoutRole(string scoutPesel, string role)
        {
            CheckDbContextInstance("void UpdateScoutRole(string scoutPesel, string role)");

            if (!GetScouts().Select(s => s.PeselScout).Contains(scoutPesel))
                throw new UnauthorizedAccessException("void UpdateScoutRole(string scoutPesel, string role): the scouts pesel belongs to a scout from a different team");

            if (!TeamRoles.TeamRolesList.Contains(role))
                throw new ArgumentException("void UpdateScoutRole(string scoutPesel, string role): invalid team role was passed as argument");

            ScoutTeam scoutTeam = this.ScoutTeam.First(st => st.ScoutPeselScout == scoutPesel);
            scoutTeam.Role = role;

            _dbContext.ScoutTeam.Update(scoutTeam);
            _dbContext.SaveChanges();
        }

        public void RemoveScout(string scoutPesel)
        {
            CheckDbContextInstance("void RemoveScout(string scoutPesel)");

            if (!ScoutTeam.Select(st => st.ScoutPeselScout).Contains(scoutPesel))
                throw new UnauthorizedAccessException("void RemoveScout(string scoutPesel): the scouts pesel belongs to a scout not from this team"); ;

#warning temporarily hard locked removing team captain
            if (GetScoutRole(scoutPesel) == TeamRoles.Captain)
                return;

            ScoutTeam scoutTeam = ScoutTeam.First(st => st.ScoutPeselScout == scoutPesel);
            Host host = GetScoutsHost(scoutPesel);

            if (host != null)
                host.RemoveScout(scoutPesel);

            _dbContext.ScoutTeam.Remove(scoutTeam);
            _dbContext.SaveChanges();

            ScoutTeam = ScoutTeam.Where(st => st.ScoutPeselScout != scoutPesel).ToList();
        }

        public void RemoveHost(int hostId)
        {
            CheckDbContextInstance("void RemoveHost(int hostId)");

            if (!Hosts.Select(h => h.IdHost).Contains(hostId))
                throw new UnauthorizedAccessException("void RemoveHost(int hostId): the hosts id belongs to a host from a different team");

            Host host = GetHost(_dbContext, hostId);
            Scout hostCaptain = host.GetCaptain();
            
            if(hostCaptain != null)
            {
                UpdateScoutRole(host.GetCaptain().PeselScout, TeamRoles.Scout);
            }

            _dbContext.Hosts.Remove(host);
            _dbContext.ScoutHost.RemoveRange(host.ScoutHost);
            _dbContext.SaveChanges();
        }

        private void CheckDbContextInstance(string methodName)
        {
            string message = string.Format("Team instance needs to be initialized with a dbContext instance to use '{0}' method. " +
                "Use static method 'Team GetTeam(ApplicationDbContext dbContext, int IdTeam)' to get a Team object with a dbContext instance.", methodName);

            if (_dbContext == null)
                throw new MissingDbContextInstanceException(message);
        }
    }
}
