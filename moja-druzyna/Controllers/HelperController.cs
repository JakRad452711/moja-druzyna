using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using moja_druzyna.Data;
using moja_druzyna.Data.Session;
using moja_druzyna.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moja_druzyna.Controllers
{
    public class HelperController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly SessionAccesser sessionAccesser;

        public HelperController(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;

            sessionAccesser = new SessionAccesser(dbContext, httpContextAccessor);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddTestData()
        {
            return Redirect("index");
        }

        public IActionResult AddDbAtoms()
        {
            List<string> achievementNames = new List<string>()
            {
                "hygenist",             // higienista
                "paramedic",            // sanitariusz
                "lifesaver",            // ratownik
                "glimmer",              // ognik
                "fire guard",           // strażnik ognia
                "fireplace master",     // mistrz ognisk
                "drill expert",         // znawca musztry
                "drill master",         // mistrz musztry
                "needle",               // igiełka
                "tailor",               // krawiec
                "young swimmer",        // młody pływak
                "swimmer",              // pływak
                "excellent swimmer",    // pływak doskonały
                "internaut",            // internauta
                "family historian",     // historyk rodzinny
                "european",             // europejczyk
                "health leader",        // lider zdrowia
                "nature friend",        // przyjaciel przyrody
                "photograph"            // fotograf
            };

            List<string> rankNames = new List<string>()
            {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6"
            };

            foreach (string name in achievementNames)
            {
                Achievement achievement = new Achievement()
                {
                    Type = name
                };

                _dbContext.Achievements.Add(achievement);
                _dbContext.SaveChanges();
            }

            foreach (string name in rankNames)
            {
                Rank rank = new Rank()
                {
                    Name = name,
                };

                _dbContext.Ranks.Add(rank);
                _dbContext.SaveChanges();
            }

            return Redirect("index");
        }

        public async Task<IActionResult> ChangeMyRole(string role)
        {
            string userId = sessionAccesser.UserId;
            IdentityUser user = _dbContext.Users.Find(userId);
            var roles = await _userManager.GetRolesAsync(user);

            ScoutTeam scoutTeam = _dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.ScoutPeselScout == sessionAccesser.UserPesel && scoutTeam.TeamIdTeam == sessionAccesser.CurrentTeamId)
                .First();

            scoutTeam.Role = role;

            _dbContext.ScoutTeam.Update(scoutTeam);
            _dbContext.SaveChanges();

            _ = await _userManager.RemoveFromRolesAsync(user, roles);
            _ = await _userManager.AddToRoleAsync(user, role);

            return Redirect("index");
        }
    }
}
