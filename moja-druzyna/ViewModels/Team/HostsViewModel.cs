using moja_druzyna.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace moja_druzyna.ViewModels.Team
{
    public class HostsViewModel
    {
        public HostsViewModel()
        {
            HostsViewModel_Hosts = new List<HostsViewModel_Host>();
        }

        [Required(ErrorMessage = "ta rubryka musi zostać wypełniona")]
        public string addedHostName { get; set; }
        public string hostCaptainPesel { get; set; }

        public List<HostsViewModel_Host> HostsViewModel_Hosts { get; set; }

        public class HostsViewModel_Host
        {
            public int HostId { get; set; }
            public string HostName { get; set; }
            public string HostCaptainLabel { get; set; }
        }
    }
}
