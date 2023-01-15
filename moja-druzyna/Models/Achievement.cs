using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using moja_druzyna.Data;
using moja_druzyna.Lib.Exceptions;
using System;
using System.Linq;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Achievement
    {
        public Achievement()
        {
            ScoutAchievements = new List<ScoutAchievement>();
        }

        [MaxLength(50)]
        public string Type { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Key]
        public int IdAchievement { get; set; }
        public virtual ICollection<ScoutAchievement> ScoutAchievements { get; set; }

        public static Dictionary<string, string> GetScoutAchievements(ApplicationDbContext dbContext, string pesel)
        {
            //List<ScoutAchievement> scoutAchievements = new List<ScoutAchievement>();

            if (dbContext == null)
                throw new ArgumentNullException();

            //bool teamExists = dbContext.Teams.Find(teamId) != null;

            //if (!teamExists)
            //    throw new RecordNotFoundException(string.Format("Team with id '{0}' doesn't exist", teamId));

            bool scoutExists = dbContext.Scouts.Find(pesel) != null;

            if (!scoutExists)
                throw new RecordNotFoundException(string.Format("Scout with pesel '{0}' doesn't exist", pesel));

            
            //var scoutTeam = dbContext.ScoutTeam.Where(t => t.TeamIdTeam == teamId).ToList();

            var scoutAchievements = new Dictionary<string, string>();

            List<ScoutAchievement> scoutAchiev = dbContext.ScoutAchievements
                .Where(t => t.ScoutPeselScout == pesel)
                .OrderByDescending(t => t.Date)
                .ToList();
            
            foreach (var a in scoutAchiev)
            {
                var achievement = dbContext.Achievements.Where(t => t.IdAchievement == a.AchievementIdAchievement).FirstOrDefault();
                
                if (achievement != null)
                {
                    var achievementName = achievement.Type;
                    var achievementDate = a.Date.ToString("dd.MM.yyyy HH:mm");

                    scoutAchievements.Add(achievementName, achievementDate);
                }
            }

            return scoutAchievements;
        }

    }
}