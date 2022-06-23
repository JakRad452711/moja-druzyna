using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
    }
}