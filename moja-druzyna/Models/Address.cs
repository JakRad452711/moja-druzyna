using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class Address
    {
        public string AddresZam { get; set; }
        public string StreatZam { get; set; }
        public string NumberHouseZam { get; set; }
        public string ZipZam { get; set; }
        public string CountryZam { get; set; }
        public string CityZam { get; set; }
        public string NumberHouseKor { get; set; }
        public string ZipKor { get; set; }
        public string CountryKor { get; set; }
        public string CityKor { get; set; }
        public string AdressKor { get; set; }
        public string StreatKor { get; set; }
        public string PeselParent { get; set; }
        public string PeselScout { get; set; }

        public virtual Parent PeselParentNavigation { get; set; }
        public virtual Scout PeselScoutNavigation { get; set; }
    }
}
