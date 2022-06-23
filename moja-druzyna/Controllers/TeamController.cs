using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Data.Session;
using moja_druzyna.Models;
using moja_druzyna.ViewModels.Team;
using System.Collections.Generic;
using System.Linq;
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

        private static bool scoutWasAdded = false;

        public TeamController(ApplicationDbContext applicationDbContext, ILogger<TeamController> logger, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _dbContext = applicationDbContext;
            _logger = logger;
            _userManager = userManager;

            sessionAccesser = new SessionAccesser(applicationDbContext, httpContextAccessor);
            modelManager    = new ModelManager(applicationDbContext);
        }

        public IActionResult Team()
        {
            ViewBag.UserRole = modelManager.GetScoutRoleInATeam(sessionAccesser.UserPesel, sessionAccesser.CurrentTeamId);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            ICollection<TeamViewModel> scoutsInfo = new List<TeamViewModel>();

            int currentTeamId = sessionAccesser.CurrentTeamId;

            List<Scout> scouts = _dbContext.ScoutTeam.Where(scoutTeam => scoutTeam.TeamIdTeam == currentTeamId).Select(_scoutTeam => _scoutTeam.Scout).ToList();

            foreach (Scout scout in scouts)
            {
                Host _host = modelManager.GetScoutsHostFromATeam(scout.PeselScout, sessionAccesser.CurrentTeamId);

                string id = scout.IdentityId;
                string title = string.Format("{0} {1}", scout.Surname, scout.Name);
                string rank = null;
                string pesel = scout.PeselScout;
                string host = _host == null ? "" : _host.Name;

                if(_dbContext.ScoutRanks.Where(scoutRank => scoutRank.ScoutPeselScout == scout.PeselScout).Count() != 0)
                    rank = _dbContext.ScoutRanks.Where(scoutRank => scoutRank.ScoutPeselScout == scout.PeselScout && scoutRank.IsCurrent).First().RankName;

                scoutsInfo.Add(new TeamViewModel() { Id = id, Title = title, Rank = rank, Host = host, Pesel = pesel });
            }

            scoutsInfo = scoutsInfo.OrderBy(info => info.Title).ToList();

            return View(scoutsInfo);
        }

        [Authorize(Roles = "captain")]
        public IActionResult TeamChangeName(string newName)
        {
            if (newName != null)
            {
                Team team = _dbContext.Teams.Find(sessionAccesser.CurrentTeamId);

                team.Name = newName;

                _dbContext.Teams.Update(team);
                _dbContext.SaveChanges();

                sessionAccesser.CurrentTeamName = newName;
            }

            return Redirect("team");
        }

        [Authorize(Roles = "captain")]
        public IActionResult RemoveScout(string scoutId)
        {
            Scout removedScout = _dbContext.Scouts.Where(scout => scout.IdentityId == scoutId).First();
            ScoutTeam scoutTeam = _dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.ScoutPeselScout == removedScout.PeselScout)
                .First();
            Parent parent = _dbContext.Parents.Find(removedScout.PeselScout);

            
            if(scoutTeam.Role == "scout" && removedScout.IdentityId != sessionAccesser.UserId)
            {
                List<int> idsOfHostsFromTheTeam = modelManager.GetListOfHostsFromATeam(sessionAccesser.CurrentTeamId).Select(host => host.IdHost).ToList();
                List<ScoutHost> scoutHostsFromTheTeam = _dbContext.ScoutHost
                    .Where(scoutHost => idsOfHostsFromTheTeam
                    .Contains(scoutHost.HostIdHost) && scoutHost.ScoutPeselScout == removedScout.PeselScout)
                    .ToList();

                if (scoutHostsFromTheTeam != null && scoutHostsFromTheTeam.Count() > 0)
                    _dbContext.ScoutHost.Remove(scoutHostsFromTheTeam.First());
                
                _dbContext.Scouts.Remove(removedScout);
                _dbContext.Parents.Remove(parent);
                _dbContext.SaveChanges();
            }

            return Redirect("team");
        }

        [Authorize(Roles = "captain,vice captain")]
        public IActionResult AddScout()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;
            ViewBag.scoutWasAdded = scoutWasAdded;
            scoutWasAdded = false;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "captain,vice captain")]
        public IActionResult AddScout(AddScoutViewModel addScoutViewModel)
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            if (ModelState.IsValid)
            {
                Scout addedScout = new Scout
                {
                    PeselScout       = addScoutViewModel.Pesel,
                    Name             = addScoutViewModel.Name,
                    Surname          = addScoutViewModel.Surname,
                    SecondName       = addScoutViewModel.SecondName,
                    MembershipNumber = addScoutViewModel.MembershipNumber,
                    Nationality      = addScoutViewModel.Nationality,
                    Ns               = addScoutViewModel.Ns
                };

                var user = new IdentityUser { UserName = addedScout.PeselScout, Email = null };
                _userManager.CreateAsync(user, "");

                addedScout.Identity = user;
                addedScout.IdentityId = user.Id;

                modelManager.CreateScoutAccount(sessionAccesser.CurrentTeamId, addedScout);

                scoutWasAdded = true;

                return Redirect("addscout");
            }

            scoutWasAdded = false;

            return View();
        }

        [Authorize(Roles = "captain,vice captain")]
        public IActionResult EditScout(string scoutId)
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            Scout editedScout = _dbContext.Scouts
                .Where(scout => scout.IdentityId == scoutId)
                .First();

            editedScout.Adress = _dbContext.Adresses
                .Where(address => address.ScoutPeselScout == editedScout.PeselScout)
                .First();

            ViewBag.scoutWasEdited = false;
            ViewBag.scoutEditionFailed = false;

            return View(editedScout);
        }

        [HttpPost]
        [Authorize(Roles = "captain,vice captain")]
        public IActionResult EditScout(Scout scout)
        {
            if (ModelState.IsValid)
            {
                modelManager.EditScout(scout);

                ViewBag.scoutWasEdited = true;
                ViewBag.scoutEditionFailed = false;

                return View(scout);
            }

            ViewBag.scoutWasEdited = false;
            ViewBag.scoutEditionFailed = true;

            return View(scout);
        }

        public IActionResult ScoutData()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            string pesel = modelManager.GetScoutPesel(sessionAccesser.CurrentScoutId);
            Scout scout = _dbContext.Scouts.Find(pesel);

            return View(scout);
        }

        [HttpPost]
        public IActionResult ScoutData(string scoutId)
        {
            sessionAccesser.CurrentScoutId = scoutId;

            return Redirect("ScoutData");
        }

        public IActionResult ScoutAchievements()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            string pesel = modelManager.GetScoutPesel(sessionAccesser.CurrentScoutId);
            Scout scout = _dbContext.Scouts.Find(pesel);

            ViewBag.ScoutTitle = string.Format("{0} {1} - osiągnięcia", scout.Name, scout.Surname);

            List<ScoutRank> scoutRanks = _dbContext.ScoutRanks
                .Where(scoutRank => scoutRank.ScoutPeselScout == pesel)
                .ToList();

            List<ScoutAchievement> scoutAchievements = _dbContext.ScoutAchievements
                .Where(scoutAchievement => scoutAchievement.ScoutPeselScout == pesel)
                .ToList();

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

            foreach(ScoutAchievement scoutAchievement in scoutAchievements)
            {
                Achievement achievement = _dbContext.Achievements.Find(scoutAchievement.AchievementIdAchievement);
                scoutAchievementsViewModels.Add(
                    new ScoutAchievementsViewModel()
                    {
                        Type = "ability",
                        Achievement = achievement.Type,
                        AcquirementTime = scoutAchievement.Date
                    });

                _logger.LogInformation(achievement.Type);
            }

            return View(scoutAchievementsViewModels.OrderByDescending(scoutAchievementVM => scoutAchievementVM.AcquirementTime).ToList());
        }

        public IActionResult Hosts()
        {
            ViewBag.UserRole = modelManager.GetScoutRoleInATeam(sessionAccesser.UserPesel, sessionAccesser.CurrentTeamId);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            List<Scout> scoutsThatAreNotInAnyHostFromTheTeam = 
                modelManager.GetScoutsFromATeamThatAreNotInAnyHostFromTheTeam(sessionAccesser.CurrentTeamId);

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();

            foreach (var scout in scoutsThatAreNotInAnyHostFromTheTeam)
            {
                dropDownList_Scouts.Add(new SelectListItem 
                { 
                    Value = scout.PeselScout, 
                    Text = string.Format("{0} {1}\t({2})", scout.Surname, scout.Name, scout.PeselScout) 
                });
            }

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;

            List<Host> hosts = modelManager.GetListOfHostsFromATeam(sessionAccesser.CurrentTeamId)
                    .OrderBy(host => host.Name)
                    .ToList();
            List<HostsViewModel_Host> hostsViewModels_Host = new List<HostsViewModel_Host>();

            foreach(Host host in hosts)
            {
                string hostCaptainPesel;
                Scout hostCaptain = new();

                if (_dbContext.ScoutHost.Where(scoutHost => scoutHost.HostIdHost == host.IdHost && scoutHost.Role == "captain").Count() != 0)
                {
                    hostCaptainPesel = _dbContext.ScoutHost
                    .Where(scoutHost => scoutHost.HostIdHost == host.IdHost && scoutHost.Role == "captain")
                    .First()
                    .ScoutPeselScout;
                    hostCaptain = _dbContext.Scouts.Find(hostCaptainPesel);
                }

                hostsViewModels_Host.Add(new HostsViewModel_Host() { 
                        HostId = host.IdHost, 
                        HostName = host.Name, 
                        HostCaptainLabel = string.Format("{0} {1}", (hostCaptain.Name == null ? "zastęp nie ma zastępowego!" : hostCaptain.Name), hostCaptain.Surname)
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
            ViewBag.UserRole = modelManager.GetScoutRoleInATeam(sessionAccesser.UserPesel, sessionAccesser.CurrentTeamId);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            List<string> peselsOfScoutsFromTheTeamThatAreNotInAnyHostFromTheTeam =
                modelManager.GetScoutsFromATeamThatAreNotInAnyHostFromTheTeam(sessionAccesser.CurrentTeamId)
                    .Select(scout => scout.PeselScout)
                    .ToList();

            if (ModelState.IsValid && peselsOfScoutsFromTheTeamThatAreNotInAnyHostFromTheTeam.Contains(hostsViewModel.hostCaptainPesel))
            {
                if (hostsViewModel != null && hostsViewModel.addedHostName != null)
                {
                    sessionAccesser.AddedHostName = hostsViewModel.addedHostName;
                    sessionAccesser.AddedHostCaptainPesel = hostsViewModel.hostCaptainPesel;

                    return Redirect("addhost");
                }
            }

            List<Scout> scoutsFromTheTeamThatAreNotInAnyHostFromTheTeam =
                modelManager.GetScoutsFromATeamThatAreNotInAnyHostFromTheTeam(sessionAccesser.CurrentTeamId);

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();

            foreach (var scout in scoutsFromTheTeamThatAreNotInAnyHostFromTheTeam)
            {
                dropDownList_Scouts.Add(new SelectListItem
                {
                    Value = scout.PeselScout,
                    Text = string.Format("{0} {1}\t({2})", scout.Surname, scout.Name, scout.PeselScout)
                });
            }

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;

            List<Host> hosts = modelManager.GetListOfHostsFromATeam(sessionAccesser.CurrentTeamId)
                    .OrderBy(host => host.Name)
                    .ToList();
            List<HostsViewModel_Host> hostsViewModels_Host = new List<HostsViewModel_Host>();

            foreach (Host host in hosts)
            {
                string hostCaptainPesel = _dbContext.ScoutHost
                    .Where(scoutHost => scoutHost.HostIdHost == host.IdHost && scoutHost.Role == "captain")
                    .First()
                    .ScoutPeselScout;
                Scout hostCaptain = _dbContext.Scouts.Find(hostCaptainPesel);

                hostsViewModels_Host.Add(new HostsViewModel_Host()
                {
                    HostId = host.IdHost,
                    HostName = host.Name,
                    HostCaptainLabel = string.Format("{0} {1}", hostCaptain.Name, hostCaptain.Surname)
                }
                );
            }

            hostsViewModel.HostsViewModel_Hosts = hostsViewModels_Host;

            return View(hostsViewModel);
        }

        [Authorize(Roles = "captain,vice captain")]
        public IActionResult AddHost()
        {
            Team team = _dbContext.Teams.Find(sessionAccesser.CurrentTeamId);
            Scout hostCaptain = _dbContext.Scouts.Find(sessionAccesser.AddedHostCaptainPesel);
            Host host = new Host() { Name = sessionAccesser.AddedHostName, Team = team, TeamIdTeam = team.IdTeam };
            ScoutHost scoutHost = new ScoutHost()
            {
                Host = host,
                HostIdHost = host.IdHost,
                Scout = hostCaptain,
                ScoutPeselScout = hostCaptain.PeselScout,
                Role = "captain"
            };
            ScoutTeam scoutTeam = _dbContext.ScoutTeam
                .Where(_scoutTeam => _scoutTeam.ScoutPeselScout == hostCaptain.PeselScout && _scoutTeam.TeamIdTeam == team.IdTeam)
                .First();

            team.Hosts.Add(host);
            team.ScoutTeam.Add(scoutTeam);
            hostCaptain.ScoutHost.Add(scoutHost);
            host.ScoutHost.Add(scoutHost);
            scoutTeam.Role = "host captain";

            _dbContext.Teams.Update(team);
            _dbContext.Scouts.Update(hostCaptain);
            _dbContext.Hosts.Add(host);
            _dbContext.ScoutHost.Add(scoutHost);
            _dbContext.ScoutTeam.Update(scoutTeam);
            _dbContext.SaveChanges();

            return Redirect("hosts");
        }

        [HttpPost]
        [Authorize(Roles = "captain")]
        public IActionResult DeleteHost(int hostId)
        {
            List<Host> hostsInTheTeam = modelManager.GetListOfHostsFromATeam(sessionAccesser.CurrentTeamId);
            List<int> idsOfhostsInTheTeam = hostsInTheTeam.Select(host => host.IdHost).ToList();

            if (idsOfhostsInTheTeam.Contains(hostId))
            {
                Host deletedHost = _dbContext.Hosts.Find(hostId);
                List<ScoutHost> scoutHosts = _dbContext.ScoutHost.Where(scoutHost => scoutHost.HostIdHost == hostId).ToList();
                List<string> peselsOfScoutsInTheHost = modelManager.GetScoutsFromAHost(hostId).Select(scout => scout.PeselScout).ToList();
                ScoutTeam hostCaptainScoutTeam = _dbContext.ScoutTeam
                    .Where(scoutTeam => peselsOfScoutsInTheHost.Contains(scoutTeam.ScoutPeselScout) && scoutTeam.Role == "host captain")
                    .First();

                hostCaptainScoutTeam.Role = "scout";

                _dbContext.Hosts.Remove(deletedHost);
                _dbContext.ScoutHost.RemoveRange(scoutHosts);
                _dbContext.ScoutTeam.Update(hostCaptainScoutTeam);
                _dbContext.SaveChanges();
            }

            return Redirect("hosts");
        }

        public IActionResult Host()
        {
            ViewBag.UserRole = modelManager.GetScoutRoleInATeam(sessionAccesser.UserPesel, sessionAccesser.CurrentTeamId);
            ViewBag.UserRoleInTheHost = modelManager.GetUserRoleInAHost(sessionAccesser.UserPesel, sessionAccesser.CurrentHostId);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;
            ViewBag.HostName = sessionAccesser.CurrentHostName;

            List<Scout> scoutsInTheHost = modelManager.GetScoutsFromAHost(sessionAccesser.CurrentHostId);

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();

            List<Scout> scoutsFromTheTeamThatAreNotInAnyHostFromTheTeam =
                modelManager.GetScoutsFromATeamThatAreNotInAnyHostFromTheTeam(sessionAccesser.CurrentTeamId);

            List<HostViewModel_Scout> hostViewModel_Scouts = new List<HostViewModel_Scout>();
            
            List<ScoutHost> scoutHosts = 
                _dbContext.ScoutHost.Where(scoutHost => scoutHost.HostIdHost == sessionAccesser.CurrentHostId).ToList();


            foreach (var scout in scoutsFromTheTeamThatAreNotInAnyHostFromTheTeam)
            {
                dropDownList_Scouts.Add(new SelectListItem
                {
                    Value = scout.PeselScout,
                    Text = string.Format("{0} {1}\t({2})", scout.Surname, scout.Name, scout.PeselScout)
                });
            }

            foreach(var scout in scoutsInTheHost)
            {
                bool isHostCaptain = scoutHosts.Where(scoutHost => scoutHost.ScoutPeselScout == scout.PeselScout)
                        .First()
                        .Role == "captain";
                string rank = null;

                if (_dbContext.ScoutRanks.Where(scoutRank => scoutRank.ScoutPeselScout == scout.PeselScout).Count() != 0)
                    rank = _dbContext.ScoutRanks.Where(scoutRank => scoutRank.ScoutPeselScout == scout.PeselScout && scoutRank.IsCurrent).First().RankName;

                hostViewModel_Scouts.Add(new HostViewModel_Scout()
                {
                    IdentityId = scout.IdentityId,
                    Rank = rank,
                    Title = string.Format("{0} {1}", scout.Surname, scout.Name),
                    IsHostCaptain = isHostCaptain
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
        [Authorize(Roles = "captain,vice captain,host captain")]
        public IActionResult AddScoutToHost(HostViewModel hostViewModel)
        {
            List<ScoutHost> scoutHosts = _dbContext.ScoutHost.Where(scoutHost => scoutHost.HostIdHost == sessionAccesser.CurrentHostId).ToList();
            List<Scout> scoutsInTheHost = _dbContext.Scouts
                .Where(scout => scoutHosts.Select(scoutHost => scoutHost.ScoutPeselScout)
                    .ToList()
                    .Contains(scout.PeselScout))
                .ToList();

            List<string> peselsOfScoutsFromTheTeamThatAreNotInAnyHostFromTheTeam =
                modelManager.GetScoutsFromATeamThatAreNotInAnyHostFromTheTeam(sessionAccesser.CurrentTeamId)
                    .Select(scout => scout.PeselScout)
                    .ToList();

            if (hostViewModel != null && hostViewModel.AddedScoutPesel != null && peselsOfScoutsFromTheTeamThatAreNotInAnyHostFromTheTeam.Contains(hostViewModel.AddedScoutPesel))
            {
                Scout addedScout = _dbContext.Scouts.Find(hostViewModel.AddedScoutPesel);
                Host host = _dbContext.Hosts.Find(sessionAccesser.CurrentHostId);
                ScoutHost newScoutHost = new ScoutHost()
                {
                    Host = host,
                    HostIdHost = sessionAccesser.CurrentHostId,
                    Scout = addedScout,
                    ScoutPeselScout = addedScout.PeselScout,
                    Role = "scout"
                };

                addedScout.ScoutHost.Add(newScoutHost);
                host.ScoutHost.Add(newScoutHost);

                _dbContext.Scouts.Update(addedScout);
                _dbContext.Hosts.Update(host);
                _dbContext.ScoutHost.Add(newScoutHost);
                _dbContext.SaveChanges();
            }

            return Redirect("host");
        }

        [HttpPost]
        [Authorize(Roles = "captain,vice captain,host captain")]
        public IActionResult RemoveScoutFromHost(string scoutId)
        {
            Scout removedScout = _dbContext.Scouts.Where(scout => scout.IdentityId == scoutId).First();
            ScoutHost scoutHost = _dbContext.ScoutHost
                .Where(scoutHost => scoutHost.HostIdHost == sessionAccesser.CurrentHostId && scoutHost.ScoutPeselScout == removedScout.PeselScout)
                .First();

            if (scoutHost != null && scoutHost.Role == "scout")
            {
                _dbContext.ScoutHost.Remove(scoutHost);
                _dbContext.SaveChanges();
            }

                return Redirect("host");
        }
    }
}
