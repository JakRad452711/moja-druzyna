using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Event
    {
        [Key]
        public int IdEvent { get; set; }
        public DateTime DateStartDateNotNullDateEnd { get; set; }
        [MaxLength(50)]
        public string HasCost { get; set; }
        [MaxLength(50)]
        public string Type { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public int? Price { get; set; }

        public virtual ICollection<AttendanceList> AttendanceLists { get; set; }
        public virtual ICollection<Collection> Collections { get; set; }
        public virtual ICollection<EventTeam> EventTeams { get; set; }
        public virtual ICollection<ScoutEvent> ScoutEvents { get; set; }
    }
}
