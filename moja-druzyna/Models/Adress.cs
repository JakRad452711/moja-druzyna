
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Adress
    {
        public string AddresZam { get; set; }
        public string StreatZam { get; set; }
        public string HouseZam { get; set; }
      

        public string ZipZam { get; set; }
        public string CountryZam { get; set; }
        public string CityZam { get; set; }
        public string HouseKor { get; set; }
        public string ZipKor { get; set; }
        public string CountryKor { get; set; }
        public string CityKor { get; set; }
        public string AdressKor { get; set; }
        public string StreatKor { get; set; }
        [ForeignKey("fk_adress_parent")]
        public string ParentPesel { get; set; }
        [ForeignKey("fk_address_scout")]
        public string ScoutPeselScout { get; set; }

        public virtual Parent Parent { get; set; }
        public virtual Scout Scout { get; set; }
    }
}
