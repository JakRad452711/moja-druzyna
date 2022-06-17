using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;

namespace moja_druzyna.Data.Session
{
    public class SessionAccesser : ISessionUserContext, ISessionTeamContext
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly string sessionUserContextName = "UserContext";
        private readonly string sessionTeamContextName = "TeamContext";
        
        public SessionAccesser(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            if(httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                _dbContext = dbContext;
                _httpContextAccessor = httpContextAccessor;

                Initialize(httpContextAccessor);
            }
        }

        public void Initialize(IHttpContextAccessor httpContextAccessor)
        {
            InitializeSessionUserContext(httpContextAccessor);
            InitializeSessionTeamContext(httpContextAccessor);
        }

        public string UserId
        {
            get
            {
                ISessionUserContext sessionUserContext = JsonConvert
                    .DeserializeObject<SessionUserContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionUserContextName));

                return sessionUserContext.UserId;
            }
            set
            {
                ISessionUserContext sessionUserContext = JsonConvert
                    .DeserializeObject<SessionUserContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionUserContextName));

                sessionUserContext.UserId = value;

                _httpContextAccessor.HttpContext.Session.SetString(sessionUserContextName, JsonConvert.SerializeObject(sessionUserContextName));
            }
        }

        public string UserPesel
        {
            get
            {
                ISessionUserContext sessionUserContext = JsonConvert
                    .DeserializeObject<SessionUserContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionUserContextName));

                return sessionUserContext.UserPesel;
            }
            set
            {
                ISessionUserContext sessionUserContext = JsonConvert
                    .DeserializeObject<SessionUserContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionUserContextName));

                sessionUserContext.UserPesel = value;

                _httpContextAccessor.HttpContext.Session.SetString(sessionUserContextName, JsonConvert.SerializeObject(sessionUserContextName));
            }
        }

        public int CurrentTeamId
        {
            get
            {
                ISessionTeamContext sessionTeamContext = JsonConvert
                    .DeserializeObject<SessionTeamContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionTeamContextName));

                return sessionTeamContext.CurrentTeamId;
            }
            set
            {
                ISessionTeamContext sessionTeamContext = JsonConvert
                    .DeserializeObject<SessionTeamContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionTeamContextName));

                sessionTeamContext.CurrentTeamId = value;

                _httpContextAccessor.HttpContext.Session.SetString(sessionTeamContextName, JsonConvert.SerializeObject(sessionTeamContext));
            }
        }

        public void InitializeSessionUserContext(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext.Session.GetString(sessionUserContextName) != null)
                return;

            string _UserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string _UserPesel = _dbContext.Scouts.Where(scout => scout.IdentityId == _UserId).First().PeselScout;

            ISessionUserContext sessionUserContext = new SessionUserContext()
            {
                UserId = _UserId,
                UserPesel = _UserPesel
            };

            httpContextAccessor.HttpContext.Session.SetString(sessionUserContextName, JsonConvert.SerializeObject(sessionUserContext));
        }

        public void InitializeSessionTeamContext(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext.Session.GetString(sessionTeamContextName) != null)
                return;

            string _CurrentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int _CurrentTeamId = _dbContext.ScoutTeam.First(scoutTeam => scoutTeam.Scout.IdentityId == _CurrentUserId).TeamIdTeam;

            ISessionTeamContext sessionTeamContext = new SessionTeamContext()
            {
                CurrentTeamId = _CurrentTeamId
            };

            httpContextAccessor.HttpContext.Session.SetString(sessionTeamContextName, JsonConvert.SerializeObject(sessionTeamContext));
        }
    }
}
