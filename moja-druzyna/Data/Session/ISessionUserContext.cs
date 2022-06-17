using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moja_druzyna.Data.Session
{
    interface ISessionUserContext
    {
        public string UserId { get; set; }
        public string UserPesel { get; set; }
    }
}
