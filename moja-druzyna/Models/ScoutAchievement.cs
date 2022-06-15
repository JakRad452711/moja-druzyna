using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class ScoutAchievement
    {
        public DateTime Date { get; set; }
        [RegularExpression("[0-9]{11}")]
        [ForeignKey("fk_scoutachievement_scout")]
        public string ScoutPeselScout { get; set; }
        [ForeignKey("fk_scoutachievement_achievement")]
        public int AchievementIdAchievement { get; set; }

        public virtual Achievement Achievement { get; set; }
        public virtual Scout Scout { get; set; }
    }
}
