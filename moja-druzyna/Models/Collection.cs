using System;
using System.Collections.Generic;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Collection
    {
        public Collection()
        {
            ScoutCollections = new HashSet<ScoutCollection>();
        }

        public int IdCollection { get; set; }
        public int? Quarter { get; set; }
        public string Type { get; set; }
        public int? IdEvent { get; set; }

        public virtual Event IdEventNavigation { get; set; }
        public virtual ICollection<ScoutCollection> ScoutCollections { get; set; }
    }
}
