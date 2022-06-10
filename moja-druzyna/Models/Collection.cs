using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Collection
    {
        
        [Key]
        public int IdCollection { get; set; }
        public int? Quarter { get; set; }
        public string Type { get; set; }
        public int? IdEvent { get; set; }

        public virtual Event Event { get; set; }
        public virtual ICollection<ScoutCollection> ScoutCollections { get; set; }
    }
}
