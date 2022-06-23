using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;

namespace moja_druzyna.Data.Session
{
    public class SessionAccesser : ISessionUserContext, ISessionTeamContext, ISessionAddHostContext
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly string sessionUserContextName = "UserContext";
        private readonly string sessionTeamContextName = "TeamContext";
        private readonly string sessionFormOrderContextName = "FormOrderContext";
        private readonly string sessionAddHostContextName = "AddHostContext";
        
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
            InitializeSessionFormOrderContext(httpContextAccessor);
            InitializeSessionAddHostContext(httpContextAccessor);
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

        public int CurrentHostId
        {
            get
            {
                ISessionTeamContext sessionTeamContext = JsonConvert
                    .DeserializeObject<SessionTeamContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionTeamContextName));

                return sessionTeamContext.CurrentHostId;
            }
            set
            {
                ISessionTeamContext sessionTeamContext = JsonConvert
                    .DeserializeObject<SessionTeamContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionTeamContextName));

                sessionTeamContext.CurrentHostId = value;

                _httpContextAccessor.HttpContext.Session.SetString(sessionTeamContextName, JsonConvert.SerializeObject(sessionTeamContext));
            }
        }

        public string CurrentScoutId
        {
            get
            {
                ISessionTeamContext sessionTeamContext = JsonConvert
                    .DeserializeObject<SessionTeamContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionTeamContextName));

                return sessionTeamContext.CurrentScoutId;
            }
            set
            {
                ISessionTeamContext sessionTeamContext = JsonConvert
                    .DeserializeObject<SessionTeamContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionTeamContextName));

                sessionTeamContext.CurrentScoutId = value;

                _httpContextAccessor.HttpContext.Session.SetString(sessionTeamContextName, JsonConvert.SerializeObject(sessionTeamContext));
            }
        }

        public string CurrentTeamName
        {
            get
            {
                ISessionTeamContext sessionTeamContext = JsonConvert
                    .DeserializeObject<SessionTeamContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionTeamContextName));

                return sessionTeamContext.CurrentTeamName;
            }
            set
            {
                ISessionTeamContext sessionTeamContext = JsonConvert
                    .DeserializeObject<SessionTeamContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionTeamContextName));

                sessionTeamContext.CurrentTeamName = value;

                _httpContextAccessor.HttpContext.Session.SetString(sessionTeamContextName, JsonConvert.SerializeObject(sessionTeamContext));
            }
        }

        public string CurrentHostName
        {
            get
            {
                ISessionTeamContext sessionTeamContext = JsonConvert
                    .DeserializeObject<SessionTeamContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionTeamContextName));

                return sessionTeamContext.CurrentHostName;
            }
            set
            {
                ISessionTeamContext sessionTeamContext = JsonConvert
                    .DeserializeObject<SessionTeamContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionTeamContextName));

                sessionTeamContext.CurrentHostName = value;

                _httpContextAccessor.HttpContext.Session.SetString(sessionTeamContextName, JsonConvert.SerializeObject(sessionTeamContext));
            }
        }

        public string AddedHostName
        {
            get
            {
                ISessionAddHostContext sessionAddHostcontext = JsonConvert
                    .DeserializeObject<SessionAddHostContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionAddHostContextName));

                return sessionAddHostcontext.AddedHostName;
            }
            set
            {
                ISessionAddHostContext sessionAddHostContext = JsonConvert
                    .DeserializeObject<SessionAddHostContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionAddHostContextName));

                sessionAddHostContext.AddedHostName = value;

                _httpContextAccessor.HttpContext.Session.SetString(sessionAddHostContextName, JsonConvert.SerializeObject(sessionAddHostContext));
            }
        }

        public string AddedHostCaptainPesel
        {
            get
            {
                ISessionAddHostContext sessionAddHostContext = JsonConvert
                    .DeserializeObject<SessionAddHostContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionAddHostContextName));

                return sessionAddHostContext.AddedHostCaptainPesel;
            }
            set
            {
                ISessionAddHostContext sessionAddHostContext = JsonConvert
                    .DeserializeObject<SessionAddHostContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionAddHostContextName));

                sessionAddHostContext.AddedHostCaptainPesel = value;

                _httpContextAccessor.HttpContext.Session.SetString(sessionAddHostContextName, JsonConvert.SerializeObject(sessionAddHostContext));
            }
        }

        public SessionFormOrderContext FormOrder
        {
            get
            {
                SessionFormOrderContext sessionFormOrderContext = JsonConvert
                    .DeserializeObject<SessionFormOrderContext>(_httpContextAccessor.HttpContext.Session.GetString(sessionFormOrderContextName));

                return sessionFormOrderContext;
            }
            set
            {
                SessionFormOrderContext sessionFormOrderContext = value;

                _httpContextAccessor.HttpContext.Session.SetString(sessionFormOrderContextName, JsonConvert.SerializeObject(sessionFormOrderContext));
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
            string _CurrentTeamName = _dbContext.Teams.Find(_CurrentTeamId).Name;

            ISessionTeamContext sessionTeamContext = new SessionTeamContext()
            {
                CurrentTeamId = _CurrentTeamId,
                CurrentHostId = -1,
                CurrentTeamName = _CurrentTeamName,
                CurrentHostName = null
            };

            httpContextAccessor.HttpContext.Session.SetString(sessionTeamContextName, JsonConvert.SerializeObject(sessionTeamContext));
        }

        public void InitializeSessionAddHostContext(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext.Session.GetString(sessionAddHostContextName) != null)
                return;

            ISessionAddHostContext sessionAddHostContext = new SessionAddHostContext();

            httpContextAccessor.HttpContext.Session.SetString(sessionAddHostContextName, JsonConvert.SerializeObject(sessionAddHostContext));
        }

        public void InitializeSessionFormOrderContext(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext.Session.GetString(sessionFormOrderContextName) != null)
                return;

            httpContextAccessor.HttpContext.Session.SetString(sessionFormOrderContextName, JsonConvert.SerializeObject(new SessionFormOrderContext()));
        }
    }
}
