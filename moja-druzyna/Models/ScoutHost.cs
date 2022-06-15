using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class ScoutHost
    {
        public string Role { get; set; }
        [ForeignKey("fk_scouthost_scout")]
        [RegularExpression("[0-9]{11}")]
        public string ScoutPeselScout { get; set; }
        [ForeignKey("fk_scouthost_host")]
        public int HostIdHost { get; set; }

        public virtual Host Host { get; set; }
        public virtual Scout Scout { get; set; }
    }
}
