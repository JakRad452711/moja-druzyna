using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moja_druzyna.Data.Session
{
    interface ISessionAddHostContext
    {
        public string AddedHostName { get; set; }
        public string AddedHostCaptainPesel { get; set; }
    }
}
