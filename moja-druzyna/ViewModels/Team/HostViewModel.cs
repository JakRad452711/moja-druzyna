using moja_druzyna.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moja_druzyna.ViewModels.Team
{
    public class HostViewModel
    {
        public HostViewModel()
        {
            Scouts = new List<HostViewModel_Scout>();
        }

        public int HostId { get; set; }
        public string AddedScoutPesel { get; set; }
        public List<HostViewModel_Scout> Scouts { get; set; }

        public class HostViewModel_Scout
        {
            public string IdentityId { get; set; }
            public string Rank { get; set; }
            public string Title { get; set; }
            public bool IsHostCaptain { get; set; }
        }
    }
}
