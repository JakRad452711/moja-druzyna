using System;
using System.Collections.Generic;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class ScoutAchievement
    {
        public DateTime Date { get; set; }
        public string Pesel { get; set; }
        public int IdAchievement { get; set; }

        public virtual Achievement IdAchievementNavigation { get; set; }
        public virtual Scout PeselNavigation { get; set; }
    }
}
