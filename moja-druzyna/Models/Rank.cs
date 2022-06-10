using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Rank
    {
       
        [Key]
        public string Name { get; set; }

        public virtual ICollection<ScoutRank> ScoutRanks { get; set; }
    }
}
