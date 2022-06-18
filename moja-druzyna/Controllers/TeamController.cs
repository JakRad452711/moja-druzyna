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
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            ICollection<TeamViewModel> scoutsInfo = new List<TeamViewModel>();

            int currentTeamId = sessionAccesser.CurrentTeamId;

            foreach (Scout scout in _dbContext.ScoutTeam.Where(scoutTeam => scoutTeam.TeamIdTeam == currentTeamId).Select(_scoutTeam => _scoutTeam.Scout))
            {
                string id = scout.IdentityId;
                string title = string.Format("{0} {1}", scout.Surname, scout.Name);
                string pesel = scout.PeselScout;
                string host = "nazwa zastępu";
                scoutsInfo.Add(new TeamViewModel() { Id = id, Title = title, Host = host, Pesel = pesel });
            }

            scoutsInfo = scoutsInfo.OrderBy(info => info.Title).ToList();

            return View(scoutsInfo);
        }

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

        public IActionResult RemoveScout(string scoutId)
        {
            Scout removedScout = _dbContext.Scouts.Where(scout => scout.IdentityId == scoutId).First();
            ScoutTeam scoutTeam = _dbContext.ScoutTeam
                .Where(scoutTeam => scoutTeam.ScoutPeselScout == removedScout.PeselScout)
                .First();

            
            if(scoutTeam.Role == "scout" && removedScout.IdentityId != sessionAccesser.UserId)
            {
                _dbContext.Scouts.Remove(removedScout);
                _dbContext.SaveChanges();
            }
            

            return Redirect("team");
        }

        public IActionResult AddScout()
        {
            ViewBag.scoutWasAdded = scoutWasAdded;
            scoutWasAdded = false;

            return View();
        }

        [HttpPost]
        public IActionResult AddScout(AddScoutViewModel addScoutViewModel)
        {
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

        public IActionResult EditScout(string scoutId)
        {
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

        public IActionResult Hosts()
        {
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
                string hostCaptainPesel = _dbContext.ScoutHost
                    .Where(scoutHost => scoutHost.HostIdHost == host.IdHost && scoutHost.Role == "captain")
                    .First()
                    .ScoutPeselScout;
                Scout hostCaptain = _dbContext.Scouts.Find(hostCaptainPesel);

                hostsViewModels_Host.Add(new HostsViewModel_Host() { 
                        HostId = host.IdHost, 
                        HostName = host.Name, 
                        HostCaptainLabel = string.Format("{0} {1}", hostCaptain.Name, hostCaptain.Surname)
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
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            List<string> peselsOfScoutsFromTheTeamThatAreNotInAnyHostFromTheTeam =
                modelManager.GetScoutsFromATeamThatAreNotInAnyHostFromTheTeam(sessionAccesser.CurrentTeamId)
                    .Select(scout => scout.PeselScout)
                    .ToList();

            if (ModelState.IsValid && peselsOfScoutsFromTheTeamThatAreNotInAnyHostFromTheTeam.Contains(hostsViewModel.hostCaptainPesel))
            {
                Team team = _dbContext.Teams.Find(sessionAccesser.CurrentTeamId);
                Scout hostCaptain = _dbContext.Scouts.Find(hostsViewModel.hostCaptainPesel);
                Host host = new Host() { Name = hostsViewModel.addedHostName, Team = team, TeamIdTeam = team.IdTeam };
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

        [HttpPost]
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

                hostViewModel_Scouts.Add(new HostViewModel_Scout()
                {
                    IdentityId = scout.IdentityId,
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

            List<ScoutHost> scoutHosts = _dbContext.ScoutHost.Where(scoutHost => scoutHost.HostIdHost == hostId).ToList();
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
                    HostIdHost = hostId,
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
