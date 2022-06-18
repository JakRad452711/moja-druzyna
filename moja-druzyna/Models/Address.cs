using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Address
    {
        [MaxLength(100)]
        public string AddressZam { get; set; }
        [MaxLength(70)]
        public string StreetZam { get; set; }
        [MaxLength(20)]
        public string NumberHouseZam { get; set; } // numer mieszkania, numer szeregówki, numer domu ( np 93/4, 102B, 27)
        [MaxLength(20)]
        public string ZipZam { get; set; }
        [MaxLength(50)]
        public string CountryZam { get; set; }
        [MaxLength(50)]
        public string CityZam { get; set; }
        [MaxLength(20)]
        public string NumberHouseKor { get; set; } // numer mieszkania, numer szeregówki, numer domu ( np 93/4, 102B, 27)
        [MaxLength(20)]
        public string ZipKor { get; set; }
        [MaxLength(50)]
        public string CountryKor { get; set; }
        [MaxLength(50)]
        public string CityKor { get; set; }
        [MaxLength(100)]
        public string AddressKor { get; set; }
        [MaxLength(70)]
        public string StreetKor { get; set; }
        [ForeignKey("fk_adress_parent")]
        public string ParentPesel { get; set; }
        [ForeignKey("fk_address_scout")]
        public string ScoutPeselScout { get; set; }

        public virtual Parent Parent { get; set; }
        public virtual Scout Scout { get; set; }
    }
}
