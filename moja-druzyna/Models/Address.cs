
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Address
    {
        public string AddressZam { get; set; }
        public string StreetZam { get; set; }
        public string NumberHouseZam { get; set; } // numer mieszkania, numer szeregówki, numer domu ( np 93/4, 102B, 27)


        public string ZipZam { get; set; }
        public string CountryZam { get; set; }
        public string CityZam { get; set; }
        public string NumberHouseKor { get; set; } // numer mieszkania, numer szeregówki, numer domu ( np 93/4, 102B, 27)
        public string ZipKor { get; set; }
        public string CountryKor { get; set; }
        public string CityKor { get; set; }
        public string AddressKor { get; set; }
        public string StreetKor { get; set; }
        [ForeignKey("fk_adress_parent")]
        public string ParentPesel { get; set; }
        [ForeignKey("fk_address_scout")]
        public string ScoutPeselScout { get; set; }

        public virtual Parent Parent { get; set; }
        public virtual Scout Scout { get; set; }
    }
}
