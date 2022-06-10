using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class ScoutTeam
    {
        public string Role { get; set; }
        [ForeignKey("fk_scoutteam_scout")]
        public string ScoutPeselScout { get; set; }
        [ForeignKey("fk_scoutteam_host")]
        public int HostIdHost { get; set; }

        public virtual Host Host { get; set; }
        public virtual Scout Scout { get; set; }
    }
}
