using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace moja_druzyna.Data.Session
{
    public class SessionTeamContext : ISessionTeamContext
    {
        public int CurrentTeamId { get; set; }
        public int CurrentHostId { get; set; }
        public string CurrentTeamName { get; set; }
        public string CurrentHostName { get; set; }
    }
}
