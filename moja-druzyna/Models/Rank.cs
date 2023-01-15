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
    public partial class Rank
    {
        public Rank()
        {
            this.ScoutRanks = new List<ScoutRank>();
        }

        [Key]
        public string Name { get; set; }

        public virtual ICollection<ScoutRank> ScoutRanks { get; set; }

        public static Rank GetRanks(ApplicationDbContext dbContext, string scoutPesel, string userId)
        {
            var rank = new Rank();

            if (dbContext == null)
                throw new System.ArgumentNullException();

            bool userExists = dbContext.Users.Find(userId) != null;

            if (!userExists)
                throw new RecordNotFoundException(string.Format("User with id '{0}' doesn't exist", scoutPesel));

            //Scout scout = dbContext.Scouts.Where(t => t.IdentityId == userId).FirstOrDefault();

            string foundPesel = dbContext.Scouts.Where(t => t.IdentityId == userId).FirstOrDefault().PeselScout;

            //bool scoutExists = dbContext.Scouts.Find(scoutPesel) != null;

            //if (!scoutExists)
            //    throw new RecordNotFoundException(string.Format("Scout with PESEL '{0}' doesn't exist", scoutPesel));

            //Rank rank = dbContext.Ranks.Where(t => t.Name == name).FirstOrDefault();

            List<ScoutRank> ranks = new List<ScoutRank>();

            ranks = dbContext.ScoutRanks.Where(t => t.ScoutPeselScout == foundPesel)
                .ToList();

            rank.ScoutRanks = ranks;

            return rank;
        }
    }
}
