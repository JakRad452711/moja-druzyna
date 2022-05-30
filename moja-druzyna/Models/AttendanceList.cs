using System;
using System.Collections.Generic;

#nullable disable
#warning
namespace moja_druzyna.Models​
{
    public partial class AttendanceList
    {
        public DateTime DateOfList { get; set; }
        public int IdList { get; set; }
        public int? IdEvent { get; set; }
        public int? IdHost { get; set; }
        public int? IdTeam { get; set; }
        public string IdScout { get; set; }

        public virtual Event IdEventNavigation { get; set; }
        public virtual Host IdHostNavigation { get; set; }
        public virtual Team IdTeamNavigation { get; set; }
        public virtual Scout IdScoutNavigation { get; set; }
    }
}
