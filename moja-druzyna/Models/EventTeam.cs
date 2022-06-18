using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class EventTeam
    {
        [ForeignKey("fk_eventteam_event")]
        public int EventIdEvent { get; set; }
        [ForeignKey("fk_eventteam_team")]
        public int TeamIdTeam { get; set; }
        public virtual Event Event { get; set; }
        public virtual Team Team { get; set; }
    }
}
