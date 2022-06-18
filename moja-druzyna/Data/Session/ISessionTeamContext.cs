using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moja_druzyna.Data.Session
{
    public interface ISessionTeamContext
    {
        public int CurrentTeamId { get; set; }
        public int CurrentHostId { get; set; }
        public string CurrentTeamName { get; set; }
        public string CurrentHostName { get; set; }
    }
}
