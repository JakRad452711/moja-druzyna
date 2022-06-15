using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class AttendanceList
    {
        public DateTime DateOfList { get; set; }
        [Key]
        public int IdList { get; set; }
        [ForeignKey("fk_attendancelist_event")]
        public int? EventIdEvent { get; set; }
        [ForeignKey("fk_attendancelist_host")]
        public int? HostIdHost { get; set; }
        [ForeignKey("fk_attendancelist_team")]
        public int? TeamIdTeam { get; set; }
        [ForeignKey("fk_attendancelist_scout")]
        public string ScoutIdScout { get; set; }

        public virtual Event Event { get; set; }
        public virtual Host Host { get; set; }
        public virtual Team Team { get; set; }
        public virtual Scout Scout { get; set; }
    }
}
