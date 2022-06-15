using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Parent
    {
        
        [Key]
        [MaxLength(11)]
        public string Pesel { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string SecondName { get; set; }
        [MaxLength(50)]
        public string Surname { get; set; }
        [MaxLength(50)]
        public string Email { get; set; }
        [ForeignKey("fk_scout_identity")]
        public string IdentityId { get; set; }

        public IdentityUser Identity { get; set; }
        public virtual Adress Adresses { get; set; }
        public virtual ICollection<Scout> Scouts { get; set; }
    }
}
