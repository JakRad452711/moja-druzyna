using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Agreement
    {
     
        [Key]
        public int IdAgreement { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ScoutAgreement> ScoutAgreements { get; set; }
    }
}
