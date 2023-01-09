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
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Role
    {

        public Role()
        {
            this.ScoutRoles = new List<string>();
        }

        [Key]
        public string Name { get; set; }

        public virtual ICollection<string> ScoutRoles { get; set; }

        public static Dictionary<string, string> GetRoles(ApplicationDbContext dbContext, int teamId)
        {
            var roles = new List<string>();
            
            var exroles = new Dictionary<string, string>();

            if (dbContext == null)
                throw new ArgumentNullException();

            bool teamExists = dbContext.Teams.Find(teamId) != null;

            if (!teamExists)
                throw new RecordNotFoundException(string.Format("Team with id '{0}' doesn't exist", teamId));

            //int teamId = dbContext.Teams.Where(t => t.Name == teamName).First().IdTeam;

            var scoutTeam = dbContext.ScoutTeam.Where(t => t.TeamIdTeam == teamId).ToList();

            foreach (var scout in scoutTeam)
            {
                roles.Add(scout.Role);

                var scoutLop = dbContext.Scouts.Where(t => t.PeselScout == scout.ScoutPeselScout).First();
                var name = scoutLop.Name;
                var sur = scoutLop.Surname;

                exroles.Add(name + " " + sur, scout.Role);
            }

            return exroles;
        }

 
    }
}
