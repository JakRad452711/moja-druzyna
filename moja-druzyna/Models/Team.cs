using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Team
    {
        public Team()
        {
            this.AttendanceLists = new List<AttendanceList>();
            this.EventTeams      = new List<EventTeam>();
            this.Hosts           = new List<Host>();
            this.ScoutTeam       = new List<ScoutTeam>();
        }
        
        [MaxLength(150)]
        public string Name { get; set; }
        [Key]
        public int IdTeam { get; set; }

        public virtual ICollection<AttendanceList> AttendanceLists { get; set; }
        public virtual ICollection<EventTeam> EventTeams { get; set; }
        public virtual ICollection<Host> Hosts { get; set; }
        public virtual ICollection<ScoutTeam> ScoutTeam { get; set; }
    }
}
