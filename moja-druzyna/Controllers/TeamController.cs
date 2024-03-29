﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using moja_druzyna.Const;
using moja_druzyna.Data;
using moja_druzyna.Data.Session;
using moja_druzyna.Lib.PeselModule;
using moja_druzyna.Lib.PdfGeneration;
using moja_druzyna.Models;
using moja_druzyna.ViewModels.Team;
using System.Collections.Generic;
using System.Linq;
using static moja_druzyna.Models.Host;
using static moja_druzyna.Models.Scout;
using static moja_druzyna.Models.Team;
using static moja_druzyna.Models.Role;
using static moja_druzyna.ViewModels.Team.HostsViewModel;
using static moja_druzyna.ViewModels.Team.HostViewModel;
using static moja_druzyna.ViewModels.Team.RolesViewModel;

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
            ViewBag.TeamId = sessionAccesser.CurrentTeamId;

            List<TeamViewModel.TeamViewModelEntry> entries = new List<TeamViewModel.TeamViewModelEntry>();

            foreach (string scoutPesel in scoutPesels)
            {
                Host host = team.GetScoutsHost(scoutPesel);
                Scout scout = GetScout(_dbContext, scoutPesel);

                string id = scout.IdentityId;
                string title = string.Format("{0} {1}", scout.Surname, scout.Name);
                string rankName = scout.GetRank() == null ? null : scout.GetRank().Name;
                string pesel = scout.PeselScout;
                string hostName = host == null ? "" : host.Name;

                entries.Add(new TeamViewModel.TeamViewModelEntry() { Id = id, Title = title, Rank = rankName, Host = hostName, Pesel = pesel });
            }

            entries = entries.OrderBy(si => si.Title).ToList();
            TeamViewModel teamVM = new TeamViewModel(_dbContext, sessionAccesser.CurrentTeamId, entries);

            return View(teamVM);
        }

        public IActionResult TeamChangeName()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            if (sessionAccesser.OperationFailed == true)
                ViewBag.nameWasEmpty = true;

            sessionAccesser.OperationFailed = false;

            return View();
        }

        [HttpPost]
        public IActionResult TeamChangeName(string newName)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain }))
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            if (!string.IsNullOrEmpty(newName))
            {
                team.UpdateName(newName);
                sessionAccesser.CurrentTeamName = newName;
            }
            else
            {
                sessionAccesser.OperationFailed = true;
            }

            return Redirect("teamchangename");
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

            bool peselIsAvailable = _dbContext.Scouts.Find(addScoutViewModel.Pesel) == null;
            bool peselIsValid = new Pesel(addScoutViewModel.Pesel).IsValid();

            if (ModelState.IsValid && peselIsAvailable && peselIsValid)
            {
                Scout addedScout = new Scout
                {
                    PeselScout = addScoutViewModel.Pesel,
                    Name = addScoutViewModel.Name,
                    Surname = addScoutViewModel.Surname,
                    SecondName = addScoutViewModel.SecondName,
                    MembershipNumber = addScoutViewModel.MembershipNumber,
                    DateOfBirth = new Pesel(addScoutViewModel.Pesel).GetBirthday(),
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
            ViewBag.scoutAdditionFailed = true;

            if (!peselIsAvailable)
                ViewBag.peselIsTaken = true;

            if (!string.IsNullOrEmpty(addScoutViewModel.Pesel) && !peselIsValid)
                ViewBag.peselIsInvalid = true;

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

            ViewBag.UserRole = team.GetScoutRole(sessionAccesser.UserPesel);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

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
                    NumberOfScouts = host.GetScouts().Count(),
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

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain, TeamRoles.ViceCaptain }))
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            List<Scout> scoutsThatAreNotInAnyHost = team.GetScoutsThatDoNotHaveAHost();
            List<string> peselsOfScoutsThatAreNotInAnyHost = scoutsThatAreNotInAnyHost.Select(s => s.PeselScout).ToList();
            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();

            foreach (string pesel in peselsOfScoutsThatAreNotInAnyHost)
            {
                if (!(team.GetScoutRole(pesel) == TeamRoles.Scout))
                    scoutsThatAreNotInAnyHost.RemoveAll(s => s.PeselScout == pesel);
            }

            foreach (Scout scout in scoutsThatAreNotInAnyHost.OrderBy(s => s.Name).OrderBy(s => s.Surname))
            {
                dropDownList_Scouts.Add(new SelectListItem
                {
                    Value = scout.PeselScout,
                    Text = string.Format("{0} {1}\t({2})", scout.Surname, scout.Name, scout.PeselScout)
                });
            }

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;
            ViewBag.hostWasAdded = sessionAccesser.OperationSucceeded;

            sessionAccesser.OperationSucceeded = false;

            return View();
        }

        [HttpPost]
        public IActionResult AddHost(AddHostViewModel addHostViewModel)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain, TeamRoles.ViceCaptain }))
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            Host host = new Host() { Name = addHostViewModel.HostName };

            if (ModelState.IsValid)
            {
                team.CreateHost(host, addHostViewModel.HostCaptainPesel);

                sessionAccesser.OperationSucceeded = true;

                return Redirect("addhost");
            }

            List<Scout> scoutsThatAreNotInAnyHost = team.GetScoutsThatDoNotHaveAHost();
            List<string> peselsOfScoutsThatAreNotInAnyHost = scoutsThatAreNotInAnyHost.Select(s => s.PeselScout).ToList();
            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();

            foreach (string pesel in peselsOfScoutsThatAreNotInAnyHost)
            {
                if (!(team.GetScoutRole(pesel) == TeamRoles.Scout))
                    scoutsThatAreNotInAnyHost.RemoveAll(s => s.PeselScout == pesel);
            }

            foreach (Scout scout in scoutsThatAreNotInAnyHost.OrderBy(s => s.Name).OrderBy(s => s.Surname))
            {
                dropDownList_Scouts.Add(new SelectListItem
                {
                    Value = scout.PeselScout,
                    Text = string.Format("{0} {1}\t({2})", scout.Surname, scout.Name, scout.PeselScout)
                });
            }

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;
            ViewBag.hostAdditionFailed = true;

            return View(addHostViewModel);
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

            HostViewModel _hostViewModel = new HostViewModel() { HostId = host.IdHost, Scouts = hostViewModel_Scouts };

            return View(_hostViewModel);
        }

        [HttpPost]
        public IActionResult Host(int hostId)
        {
            sessionAccesser.CurrentHostId = hostId;
            sessionAccesser.CurrentHostName = _dbContext.Hosts.Find(hostId).Name;

            return RedirectToAction("Host");
        }

        public IActionResult AddScoutToHost(int hostId)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain, TeamRoles.ViceCaptain }))
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            List<Scout> scoutsThatAreNotInAnyHost = team.GetScoutsThatDoNotHaveAHost();
            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();

            foreach (Scout scout in scoutsThatAreNotInAnyHost.OrderBy(s => s.Name).OrderBy(s => s.Surname))
            {
                dropDownList_Scouts.Add(new SelectListItem
                {
                    Value = scout.PeselScout,
                    Text = string.Format("{0} {1}\t({2})", scout.Surname, scout.Name, scout.PeselScout)
                });
            }

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;
            ViewBag.hostWasAdded = sessionAccesser.OperationSucceeded;
            ViewBag.hostName = sessionAccesser.CurrentHostName;
            ViewBag.numberOfScoutsToAdd = scoutsThatAreNotInAnyHost.Count();

            sessionAccesser.OperationSucceeded = false;

            return View(new HostViewModel() { HostId = hostId });
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

                sessionAccesser.OperationSucceeded = true;
            }

            return Redirect("addscouttohost");
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

        public IActionResult Roles()
        {
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            var roles = GetRoles(_dbContext, sessionAccesser.CurrentTeamId);

            ICollection<RolesViewModel> rolesVM = new List<RolesViewModel>();

            foreach (KeyValuePair<string, string> role in roles)
            {
                var temp = "";

                if (role.Value != "scout")
                {
                    temp = TeamRoles.TeamRolesTranslationsWithPolishLetters[role.Value];

                    rolesVM.Add(new RolesViewModel()
                        {
                            ScoutName = role.Key,
                            RoleName = temp
                        }
                    );
                }
            }

            rolesVM = rolesVM.OrderBy(x => x.ScoutName).ToList();

            return View(rolesVM);
        }
    }
}
