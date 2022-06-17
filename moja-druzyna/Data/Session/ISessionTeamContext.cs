using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moja_druzyna.Data.Session
{
    public interface ISessionTeamContext
    {
        public int CurrentTeamId { get; set; }
    }
}
