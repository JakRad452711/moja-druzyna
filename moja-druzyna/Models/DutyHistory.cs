using System;
using System.Collections.Generic;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class DutyHistory
    {
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Team { get; set; }
        public string Detachment { get; set; }
        public string Banner { get; set; }
        public string Host { get; set; }
        public string Pesel { get; set; }

        public virtual Scout PeselNavigation { get; set; }
    }
}
