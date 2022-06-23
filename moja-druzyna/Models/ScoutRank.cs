using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class ScoutRank
    {
        public DateTime DateAcquirement { get; set; }
        public int CurrentRank { get; set; }
        [ForeignKey("fk_scoutrank_scout")]
        [RegularExpression("[0-9]{11}")]
        public string ScoutPeselScout { get; set; }
        [ForeignKey("fk_scoutrank_rank")]
        public string RankName { get; set; }
        public bool IsCurrent { get; set; }

        public virtual Rank Rank { get; set; }
        public virtual Scout Scout { get; set; }
    }
}
