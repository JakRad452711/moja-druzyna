using System;
using System.Collections.Generic;

#nullable disable

namespace moja_druzyna.Models​
{
    public partial class ScoutTeam
    {
        public string Role { get; set; }
        public string Pesel { get; set; }
        public int IdHost { get; set; }

        public virtual Host IdHostNavigation { get; set; }
        public virtual Scout PeselNavigation { get; set; }
    }
}
