using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Rank
    {
        public Rank()
        {
            this.ScoutRanks = new List<ScoutRank>();
        }

        [Key]
        public string Name { get; set; }

        public virtual ICollection<ScoutRank> ScoutRanks { get; set; }
    }
}
