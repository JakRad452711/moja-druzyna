using Microsoft.EntityFrameworkCore;
using moja_druzyna.Const;
using moja_druzyna.Data;
using moja_druzyna.Lib.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Host
    {
        private ApplicationDbContext _dbContext;

        public Host()
        {
            this.AttendanceLists = new List<AttendanceList>();
            this.ScoutHost       = new List<ScoutHost>();
        }

        [Key]
        public int IdHost { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [ForeignKey("fk_host_team")]
        public int TeamIdTeam { get; set; }
        public bool IsDefaultHost { get; set; }

        public virtual Team Team { get; set; }
        public virtual ICollection<AttendanceList> AttendanceLists { get; set; }
        public virtual ICollection<ScoutHost> ScoutHost { get; set; }

        public static Host GetHost(ApplicationDbContext dbContext, int hostId)
        {
            if (dbContext == null)
                throw new System.ArgumentNullException();

            bool hostExists = dbContext.Hosts.Find(hostId) != null;

            if (!hostExists)
                throw new RecordNotFoundException(string.Format("Host with id '{0}' doesn't exist", hostId));

            Host host = dbContext.Hosts.Where(h => h.IdHost == hostId)
                .Include(t => t.AttendanceLists)
                .Include(t => t.ScoutHost)
                .First();

            host._dbContext = dbContext;

            return host;
        }

        public void AddScout(string scoutPesel)
        {
            CheckDbContextInstance("void AddScout(string scoutPesel)");

            if (!_dbContext.Scouts.Any(s => s.PeselScout == scoutPesel))
                throw new RecordNotFoundException(String.Format("Scout with {0} pesel doesn't exist", scoutPesel));

            if (GetScouts().Select(s => s.PeselScout).Contains(scoutPesel))
                return;

            Scout scout = _dbContext.Scouts.Find(scoutPesel);
            ScoutHost newScoutHost = new ScoutHost()
            {
                Host = this,
                HostIdHost = IdHost,
                Scout = scout,
                ScoutPeselScout = scout.PeselScout,
                Role = "scout"
            };

            scout.ScoutHost.Add(newScoutHost);
            ScoutHost.Add(newScoutHost);

            _dbContext.Scouts.Update(scout);
            _dbContext.Hosts.Update(this);
            _dbContext.ScoutHost.Add(newScoutHost);
            _dbContext.SaveChanges();
        }

        public string GetScoutRole(string scoutPesel)
        {
            if (ScoutHost == null)
                throw new NullReferenceException("string GetScoutRole(string scoutPesel): ScoutHost variable is null");

            ScoutHost scoutHost = ScoutHost.FirstOrDefault(sh => sh.ScoutPeselScout == scoutPesel);

            if (scoutHost == null)
                return null;

            return scoutHost.Role;
        }

        public Scout GetCaptain()
        {
            CheckDbContextInstance("Scout GetCaptain()");

            ScoutHost hostCaptainSH = ScoutHost.FirstOrDefault(sh => sh.Role == HostRoles.HostCaptain);

            if (hostCaptainSH == null)
                return null;

            return _dbContext.Scouts.FirstOrDefault(s => s.PeselScout == hostCaptainSH.ScoutPeselScout);
        }

        public List<Scout> GetScouts()
        {
            List<string> scoutPesels = ScoutHost.Select(sh => sh.ScoutPeselScout).ToList();

            return _dbContext.Scouts.Where(s => scoutPesels.Contains(s.PeselScout)).ToList();
        }

        public void UpdateScoutRole(string scoutPesel, string role)
        {
            CheckDbContextInstance("void UpdateScoutRole(string scoutPesel, string role)");

            if (!GetScouts().Select(s => s.PeselScout).Contains(scoutPesel))
                throw new UnauthorizedAccessException("void UpdateScoutRole(string scoutPesel, string role): the scouts pesel belongs to a scout not from this host");

            if (!HostRoles.HostRolesList.Contains(role))
                throw new ArgumentException("void UpdateScoutRole(string scoutPesel, string role): invalid host role was passed as argument");
            
            Scout captain = GetCaptain();

            if (role == HostRoles.HostCaptain && captain != null)
            {
                ScoutTeam scoutTeam = _dbContext.ScoutTeam.Find(captain.PeselScout, Team.IdTeam);
                ScoutHost oldScoutHost = _dbContext.ScoutHost.Find(captain.PeselScout, IdHost);
                scoutTeam.Role = TeamRoles.Scout;
                oldScoutHost.Role = HostRoles.Scout;

                _dbContext.ScoutTeam.Update(scoutTeam);
                _dbContext.ScoutHost.Update(oldScoutHost);
            }

            ScoutHost scoutHost = this.ScoutHost.First(st => st.ScoutPeselScout == scoutPesel);
            scoutHost.Role = role;

            if(role == HostRoles.HostCaptain)
            {
                ScoutTeam scoutTeam = _dbContext.ScoutTeam.Find(scoutPesel, Team.IdTeam);
                scoutTeam.Role = TeamRoles.HostCaptain;

                _dbContext.ScoutTeam.Update(scoutTeam);
            }

            _dbContext.ScoutHost.Update(scoutHost);
            _dbContext.SaveChanges();
        }

        public void RemoveScout(string scoutPesel)
        {
            if (!ScoutHost.Select(sh => sh.ScoutPeselScout).Contains(scoutPesel) || GetScoutRole(scoutPesel) == HostRoles.HostCaptain)
                return;

            ScoutHost scoutHost = ScoutHost.First(sh => sh.ScoutPeselScout == scoutPesel);
            ScoutTeam scoutTeam = _dbContext.ScoutTeam.Find(scoutPesel, Team.IdTeam);

            if(scoutTeam.Role == TeamRoles.HostCaptain)
            {
                scoutTeam.Role = TeamRoles.Scout;
                _dbContext.ScoutTeam.Update(scoutTeam);
            }

            _dbContext.ScoutHost.Remove(scoutHost);
            _dbContext.SaveChanges();
        }

        private void CheckDbContextInstance(string methodName)
        {
            string message = string.Format("Host instance needs to be initialized with a dbContext instance to use '{0}' method. " +
                "Use static method 'Host GetHost(ApplicationDbContext dbContext, int IdTeam)' to get a Host object with a dbContext instance.", methodName);

            if (_dbContext == null)
                throw new MissingDbContextInstanceException(message);
        }
    }
}
