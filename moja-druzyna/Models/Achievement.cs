using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Achievement
    {
     

        public string Type { get; set; }
        public string Description { get; set; }
        [Key]
        public int IdAchievement { get; set; }
        public virtual ICollection<ScoutAchievement> ScoutAchievements { get; set; }
    }
}
