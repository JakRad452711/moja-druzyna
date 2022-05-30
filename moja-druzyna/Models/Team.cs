using System;
using System.Collections.Generic;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Team
    {
        public Team()
        {
            AttendanceLists = new HashSet<AttendanceList>();
            EventTeams = new HashSet<EventTeam>();
            Hosts = new HashSet<Host>();
        }

        public string Name { get; set; }
        public int IdTeam { get; set; }

        public virtual ICollection<AttendanceList> AttendanceLists { get; set; }
        public virtual ICollection<EventTeam> EventTeams { get; set; }
        public virtual ICollection<Host> Hosts { get; set; }
    }
}
