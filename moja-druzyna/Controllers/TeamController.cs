using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using moja_druzyna.Const;
using moja_druzyna.Data;
using moja_druzyna.Data.Session;
using moja_druzyna.Models;
using moja_druzyna.ViewModels.Team;
using System.Collections.Generic;
using System.Linq;
using static moja_druzyna.Models.Host;
using static moja_druzyna.Models.Scout;
using static moja_druzyna.Models.Team;
using static moja_druzyna.ViewModels.Team.HostsViewModel;
using static moja_druzyna.ViewModels.Team.HostViewModel;

namespace moja_druzyna.Controllers
{
    public class TeamController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TeamController> _logger;
        UserManager<IdentityUser> _userManager;

        private readonly SessionAccesser sessionAccesser;
        private readonly ModelManager modelManager;

        public TeamController(ApplicationDbContext applicationDbContext, ILogger<TeamController> logger, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _dbContext = applicationDbContext;
            _logger = logger;
            _userManager = userManager;

            sessionAccesser = new SessionAccesser(applicationDbContext, httpContextAccessor);
            modelManager = new ModelManager(applicationDbContext);
        }

        [HttpGet]
        public IActionResult Team()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);
            List<string> scoutPesels = team.GetScouts().Select(s => s.PeselScout).ToList();

            ViewBag.UserRole = team.GetScoutRole(sessionAccesser.UserPesel);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            ICollection<TeamViewModel> scoutsInfo = new List<TeamViewModel>();

            foreach (string scoutPesel in scoutPesels)
            {
                Host host = team.GetScoutsHost(scoutPesel);
                Scout scout = GetScout(_dbContext, scoutPesel);

                string id = scout.IdentityId;
                string title = string.Format("{0} {1}", scout.Surname, scout.Name);
                string rankName = scout.GetRank() == null ? null : scout.GetRank().Name;
                string pesel = scout.PeselScout;
                string hostName = host == null ? "" : host.Name;

                scoutsInfo.Add(new TeamViewModel() { Id = id, Title = title, Rank = rankName, Host = hostName, Pesel = pesel });
            }

            scoutsInfo = scoutsInfo.OrderBy(si => si.Title).ToList();

            return View(scoutsInfo);
        }

        [HttpPost]
        public IActionResult TeamChangeName(string newName)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain }))
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            if (newName != null)
            {
                team.UpdateName(newName);
                sessionAccesser.CurrentTeamName = newName;
            }

            return Redirect("team");
        }

        [HttpPost]
        public IActionResult RemoveScout(string scoutId)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);
            Scout removedScout = GetScoutById(_dbContext, scoutId);

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain }))
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            if (team.ScoutHasOneOfRoles(removedScout.PeselScout, new() { TeamRoles.Scout }))
                team.RemoveScout(removedScout.PeselScout);

            return Redirect("team");
        }

        [HttpGet]
        public IActionResult AddScout()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain, TeamRoles.ViceCaptain }))
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            ViewBag.TeamName = sessionAccesser.CurrentTeamName;
            ViewBag.scoutWasAdded = sessionAccesser.ScoutWasAdded;
            sessionAccesser.ScoutWasAdded = false;

            return View();
        }

        [HttpPost]
        public IActionResult AddScout(AddScoutViewModel addScoutViewModel)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain, TeamRoles.ViceCaptain }))
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            if (ModelState.IsValid)
            {
                Scout addedScout = new Scout
                {
                    PeselScout = addScoutViewModel.Pesel,
                    Name = addScoutViewModel.Name,
                    Surname = addScoutViewModel.Surname,
                    SecondName = addScoutViewModel.SecondName,
                    MembershipNumber = addScoutViewModel.MembershipNumber,
                    Nationality = addScoutViewModel.Nationality,
                    Ns = addScoutViewModel.Ns
                };

                var user = new IdentityUser { UserName = addedScout.PeselScout, Email = null };
                _userManager.CreateAsync(user, "");

                addedScout.Identity = user;
                addedScout.IdentityId = user.Id;

                team.CreateScout(addedScout);

                sessionAccesser.ScoutWasAdded = true;

                return Redirect("addscout");
            }

            sessionAccesser.ScoutWasAdded = false;

            return View();
        }

        [HttpGet]
        public IActionResult EditScout(string scoutId)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            bool editedScoutIsInTheTeam = team.GetScouts().Select(s => s.IdentityId).Contains(scoutId);

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain, TeamRoles.ViceCaptain }) || !editedScoutIsInTheTeam)
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            ViewBag.scoutWasEdited = false;
            ViewBag.scoutEditionFailed = false;

            return View(GetScoutById(_dbContext, scoutId));
        }

        [HttpPost]
        public IActionResult EditScout(Scout scout)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);
            Scout editedScout = GetScout(_dbContext, scout.PeselScout);

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain, TeamRoles.ViceCaptain }))
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            if (ModelState.IsValid)
            {
                editedScout.Edit(scout);

                ViewBag.scoutWasEdited = true;
                ViewBag.scoutEditionFailed = false;

                return View(scout);
            }

            ViewBag.scoutWasEdited = false;
            ViewBag.scoutEditionFailed = true;

            return View(scout);
        }

        [HttpGet]
        public IActionResult ScoutData()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            bool scoutIsInTheTeam = team.GetScouts().Select(s => s.IdentityId).Contains(sessionAccesser.CurrentScoutId);

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain, TeamRoles.ViceCaptain }) || !scoutIsInTheTeam)
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            Scout scout = GetScoutById(_dbContext, sessionAccesser.CurrentScoutId);

            return View(scout);
        }

        [HttpPost]
        public IActionResult ScoutData(string scoutId)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            bool scoutIsInTheTeam = team.GetScouts().Select(s => s.IdentityId).Contains(scoutId);

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain, TeamRoles.ViceCaptain }) || !scoutIsInTheTeam)
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            sessionAccesser.CurrentScoutId = scoutId;

            return Redirect("ScoutData");
        }

        public IActionResult ScoutAchievements()
        {
            Scout scout = GetScoutById(_dbContext, sessionAccesser.CurrentScoutId);

            ViewBag.TeamName = sessionAccesser.CurrentTeamName;
            ViewBag.ScoutTitle = string.Format("{0} {1} - osiągnięcia", scout.Name, scout.Surname);

            List<ScoutRank> scoutRanks = scout.ScoutRanks.ToList();
            List<ScoutAchievement> scoutAchievements = scout.GetScoutAchievements();

            List<ScoutAchievementsViewModel> scoutAchievementsViewModels = new();

            foreach (ScoutRank scoutRank in scoutRanks)
            {
                scoutAchievementsViewModels.Add(
                    new ScoutAchievementsViewModel()
                    {
                        Type = "rank",
                        Rank = scoutRank.RankName,
                        AcquirementTime = scoutRank.DateAcquirement,
                    });
            }

            foreach (ScoutAchievement scoutAchievement in scoutAchievements)
            {
                Achievement achievement = _dbContext.Achievements.Find(scoutAchievement.AchievementIdAchievement);
                scoutAchievementsViewModels.Add(
                    new ScoutAchievementsViewModel()
                    {
                        Type = "ability",
                        Achievement = achievement.Type,
                        AcquirementTime = scoutAchievement.Date
                    });
            }

            return View(scoutAchievementsViewModels.OrderByDescending(savm => savm.AcquirementTime).ToList());
        }

        public IActionResult Hosts()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);
            List<Scout> scoutsThatAreNotInAnyHost = team.GetScoutsThatDoNotHaveAHost();
            List<string> peselsOfScoutsThatAreNotInAnyHost = scoutsThatAreNotInAnyHost.Select(s => s.PeselScout).ToList();
            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();

            ViewBag.UserRole = team.GetScoutRole(sessionAccesser.UserPesel);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            foreach (string pesel in peselsOfScoutsThatAreNotInAnyHost)
            {
                if (!(team.GetScoutRole(pesel) == TeamRoles.Scout))
                    scoutsThatAreNotInAnyHost.RemoveAll(s => s.PeselScout == pesel);
            }

            foreach (Scout scout in scoutsThatAreNotInAnyHost)
            {
                dropDownList_Scouts.Add(new SelectListItem
                {
                    Value = scout.PeselScout,
                    Text = string.Format("{0} {1}\t({2})", scout.Surname, scout.Name, scout.PeselScout)
                });
            }

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;

            List<int> hostIds = team.Hosts.OrderBy(h => h.Name).Select(h => h.IdHost).ToList();
            List<HostsViewModel_Host> hostsViewModels_Host = new List<HostsViewModel_Host>();

            foreach (int hostId in hostIds)
            {
                Host host = GetHost(_dbContext, hostId);
                Scout hostCaptain = host.GetCaptain();

                hostsViewModels_Host.Add(new HostsViewModel_Host()
                {
                    HostId = host.IdHost,
                    HostName = host.Name,
                    HostCaptainLabel = hostCaptain == null ? "zastęp nie ma zastępowego!" : string.Format("{0} {1}", hostCaptain.Name, hostCaptain.Surname)
                }
                );
            }

            HostsViewModel hostsVM = new HostsViewModel()
            {
                HostsViewModel_Hosts = hostsViewModels_Host
            };

            return View(hostsVM);
        }

        [HttpPost]
        public IActionResult Hosts(HostsViewModel hostsViewModel)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);
            List<Scout> scoutsThatAreNotInAnyHost = team.GetScoutsThatDoNotHaveAHost();
            List<string> peselsOfScoutsThatAreNotInAnyHost = scoutsThatAreNotInAnyHost.Select(s => s.PeselScout).ToList();
            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();

            ViewBag.UserRole = team.GetScoutRole(sessionAccesser.UserPesel);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            if (ModelState.IsValid && peselsOfScoutsThatAreNotInAnyHost.Contains(hostsViewModel.hostCaptainPesel))
            {
                if (hostsViewModel != null && hostsViewModel.addedHostName != null)
                {
                    sessionAccesser.AddedHostName = hostsViewModel.addedHostName;
                    sessionAccesser.AddedHostCaptainPesel = hostsViewModel.hostCaptainPesel;

                    return Redirect("addhost");
                }
            }

            foreach (string pesel in peselsOfScoutsThatAreNotInAnyHost)
            {
                if (!(team.GetScoutRole(pesel) == TeamRoles.Scout))
                    scoutsThatAreNotInAnyHost.RemoveAll(s => s.PeselScout == pesel);
            }

            foreach (var scout in scoutsThatAreNotInAnyHost)
            {
                dropDownList_Scouts.Add(new SelectListItem
                {
                    Value = scout.PeselScout,
                    Text = string.Format("{0} {1}\t({2})", scout.Surname, scout.Name, scout.PeselScout)
                });
            }

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;

            List<int> hostIds = team.Hosts.OrderBy(h => h.Name).Select(h => h.IdHost).ToList();
            List<HostsViewModel_Host> hostsViewModels_Host = new List<HostsViewModel_Host>();

            foreach (int hostId in hostIds)
            {
                Host host = GetHost(_dbContext, hostId);
                Scout hostCaptain = host.GetCaptain();

                hostsViewModels_Host.Add(new HostsViewModel_Host()
                {
                    HostId = host.IdHost,
                    HostName = host.Name,
                    HostCaptainLabel = hostCaptain == null ? "zastęp nie ma zastępowego!" : string.Format("{0} {1}", hostCaptain.Name, hostCaptain.Surname)
                }
                );
            }

            hostsViewModel.HostsViewModel_Hosts = hostsViewModels_Host;

            return View(hostsViewModel);
        }

        public IActionResult AddHost()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);
            Host host = new Host() { Name = sessionAccesser.AddedHostName };

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain, TeamRoles.ViceCaptain }))
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            team.CreateHost(host, sessionAccesser.AddedHostCaptainPesel);

            return Redirect("hosts");
        }

        [HttpPost]
        public IActionResult DeleteHost(int hostId)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            if (team.Hosts.Select(h => h.IdHost).Contains(hostId) && UserHasOneOfRoles(team, new() { TeamRoles.Captain }))
                team.RemoveHost(hostId);
            else
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            return Redirect("hosts");
        }

        public IActionResult Host()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);
            Host host = GetHost(_dbContext, sessionAccesser.CurrentHostId);

            ViewBag.UserRole = team.GetScoutRole(sessionAccesser.UserPesel);
            ViewBag.UserRoleInTheHost = host.GetScoutRole(sessionAccesser.UserPesel);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;
            ViewBag.HostName = sessionAccesser.CurrentHostName;

            List<Scout> scoutsInTheHost = host.GetScouts();
            List<Scout> scoutsThatAreNotInAnyHost = team.GetScoutsThatDoNotHaveAHost();
            List<string> peselsOfScoutsThatAreNotInAnyHost = scoutsThatAreNotInAnyHost.Select(s => s.PeselScout).ToList();

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<HostViewModel_Scout> hostViewModel_Scouts = new List<HostViewModel_Scout>();

            string hostCaptainPesel = null;

            if (host.GetCaptain() != null)
                hostCaptainPesel = host.GetCaptain().PeselScout;

            foreach (string pesel in peselsOfScoutsThatAreNotInAnyHost)
            {
                if (team.ScoutHasOneOfRoles(pesel, new() { TeamRoles.Captain, TeamRoles.ViceCaptain }))
                    scoutsThatAreNotInAnyHost.RemoveAll(s => s.PeselScout == pesel);
            }

            foreach (Scout scout in scoutsThatAreNotInAnyHost)
            {
                dropDownList_Scouts.Add(new SelectListItem
                {
                    Value = scout.PeselScout,
                    Text = string.Format("{0} {1}\t({2})", scout.Surname, scout.Name, scout.PeselScout)
                });
            }

            foreach (Scout scout in scoutsInTheHost)
            {
                string rank = null;

                if (_dbContext.ScoutRanks.Any(sr => sr.ScoutPeselScout == scout.PeselScout))
                    rank = _dbContext.ScoutRanks.Where(sr => sr.ScoutPeselScout == scout.PeselScout && sr.IsCurrent).First().RankName;

                hostViewModel_Scouts.Add(new HostViewModel_Scout()
                {
                    IdentityId = scout.IdentityId,
                    Rank = rank,
                    Title = string.Format("{0} {1}", scout.Surname, scout.Name),
                    IsHostCaptain = hostCaptainPesel == scout.PeselScout
                });
            }

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;

            HostViewModel _hostViewModel = new HostViewModel() { Scouts = hostViewModel_Scouts };

            return View(_hostViewModel);
        }

        [HttpPost]
        public IActionResult Host(int hostId, HostViewModel hostViewModel)
        {
            if (hostId > 0)
            {
                sessionAccesser.CurrentHostId = hostId;
                sessionAccesser.CurrentHostName = _dbContext.Hosts.Find(hostId).Name;
            }

            if (hostViewModel != null && hostViewModel.AddedScoutPesel != null)
                return RedirectToAction("addscouttohost", hostViewModel);

            return Redirect("host");
        }

        [HttpPost]
        public IActionResult AddScoutToHost(HostViewModel hostViewModel)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);
            Host host = GetHost(_dbContext, sessionAccesser.CurrentHostId);

            bool userIsCaptainOrViceCaptain = UserHasOneOfRoles(team, new() { TeamRoles.Captain, TeamRoles.ViceCaptain });
            bool userIsHostCaptain = host.GetScoutRole(sessionAccesser.UserPesel) == HostRoles.HostCaptain;

            if (!(userIsCaptainOrViceCaptain || userIsHostCaptain))
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            List<string> peselsOfScoutsThatAreNotInAnyHost = team.GetScoutsThatDoNotHaveAHost().Select(s => s.PeselScout).ToList();

            if (hostViewModel != null && hostViewModel.AddedScoutPesel != null && peselsOfScoutsThatAreNotInAnyHost.Contains(hostViewModel.AddedScoutPesel))
            {
                host.AddScout(hostViewModel.AddedScoutPesel);
            }

            return Redirect("host");
        }

        [HttpPost]
        public IActionResult RemoveScoutFromHost(string scoutId)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);
            Host host = GetHost(_dbContext, sessionAccesser.CurrentHostId);
            Scout scout = GetScoutById(_dbContext, scoutId);

            bool userIsCaptainOrViceCaptain = UserHasOneOfRoles(team, new() { TeamRoles.Captain, TeamRoles.ViceCaptain });
            bool userIsHostCaptain = host.GetScoutRole(sessionAccesser.UserPesel) == HostRoles.HostCaptain;
            bool scoutIsInTheTeam = team.GetScouts().Select(s => s.IdentityId).Contains(scoutId);
            bool hostIsInTheTeam = team.Hosts.Select(h => h.IdHost).Contains(host.IdHost);

            if (!(userIsCaptainOrViceCaptain || userIsHostCaptain) || !scoutIsInTheTeam || !hostIsInTheTeam)
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            host.RemoveScout(scout.PeselScout);

            return Redirect("host");
        }

        private bool UserHasOneOfRoles(Team team, List<string> roles)
        {
            return team.ScoutHasOneOfRoles(sessionAccesser.UserPesel, roles);
        }
    }
}
