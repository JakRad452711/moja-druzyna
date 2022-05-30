using System;
using System.Collections.Generic;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Event
    {
        public Event()
        {
            AttendanceLists = new HashSet<AttendanceList>();
            Collections = new HashSet<Collection>();
            EventTeams = new HashSet<EventTeam>();
            ScoutEvents = new HashSet<ScoutEvent>();
        }

        public int IdEvent { get; set; }
        public DateTime DateStartDateNotNullDateEnd { get; set; }
        public string HasCost { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int? Price { get; set; }

        public virtual ICollection<AttendanceList> AttendanceLists { get; set; }
        public virtual ICollection<Collection> Collections { get; set; }
        public virtual ICollection<EventTeam> EventTeams { get; set; }
        public virtual ICollection<ScoutEvent> ScoutEvents { get; set; }
    }
}
