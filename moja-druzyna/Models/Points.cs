using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace moja_druzyna.Models
{
    public class Points
    {
        public DateTime DateAcquirement { get; set; }

        [ForeignKey("fk_scoutrank_scout")]
        [RegularExpression("[0-9]{11}")]
        public string ScoutPeselScout { get; set; }

        public int Ammount { get; set; }
        
        public string OrderId { get; set; }
        

        public virtual Scout Scout { get; set; }
    }
}
