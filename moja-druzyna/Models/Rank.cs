using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
