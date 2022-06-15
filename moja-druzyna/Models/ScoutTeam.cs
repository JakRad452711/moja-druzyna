using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace moja_druzyna.Models​
{
    public class ScoutTeam
    {
        public string Role { get; set; }
        [ForeignKey("fk_scoutteam_scout")]
        [RegularExpression("[0-9]{11}")]
        public string ScoutPeselScout { get; set; }
        [ForeignKey("fk_scoutteam_team")]
        public int TeamIdTeam { get; set; }

        public virtual Team Team { get; set; }
        public virtual Scout Scout { get; set; }
        
    }
}
