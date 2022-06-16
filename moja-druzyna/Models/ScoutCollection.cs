using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


#nullable disable

namespace moja_druzyna.Models​
{
    public partial class ScoutCollection
    {
        public int Ammount { get; set; }
        public DateTime DateAcquirement { get; set; }
        [MaxLength(100)]
        public string Advance { get; set; }
        [ForeignKey("fk_scoutcollection_scout")]
        [RegularExpression("[0-9]{11}")]
        public string ScoutPeselScout { get; set; }
        [ForeignKey("fk_scoutcollection_collection")]
        public int CollectionIdCollection { get; set; }

        public virtual Collection Collection { get; set; }
        public virtual Scout Scout { get; set; }
    }
}
