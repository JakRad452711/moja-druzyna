using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Agreement
    {
        public Agreement()
        {
            this.ScoutAgreements = new List<ScoutAgreement>();
        }

        [Key]
        public int IdAgreement { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ScoutAgreement> ScoutAgreements { get; set; }
    }
}
