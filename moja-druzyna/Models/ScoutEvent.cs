using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class ScoutEvent
    {
        [ForeignKey("fk_scoutevent_scout")]
        [MaxLength(11)]
        public string ScoutPeselScout { get; set; }
        [ForeignKey("fk_scoutevent_event")]
        public int EventIdEvent { get; set; }

        public virtual Event Event { get; set; }
        public virtual Scout Scout { get; set; }
    }
}
