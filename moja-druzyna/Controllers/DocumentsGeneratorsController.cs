using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Data.Session;
using moja_druzyna.Lib.Order;
using moja_druzyna.Lib.PdfGeneration;
using moja_druzyna.Models;
using moja_druzyna.ViewModels.DocumentsGenerators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using static moja_druzyna.Models.Team;
using static moja_druzyna.Models.Host;
using static moja_druzyna.Models.Scout;
using System.Threading.Tasks;
using static moja_druzyna.ViewModels.DocumentsGenerators.AttendanceViewModel;
using static moja_druzyna.ViewModels.DocumentsGenerators.OrdersViewModel;
using moja_druzyna.Const;

namespace moja_druzyna.Controllers
{
    public class DocumentsGeneratorsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DocumentsGeneratorsController> _logger;

        private readonly SessionAccesser sessionAccesser;
        private readonly ModelManager modelManager;

        public DocumentsGeneratorsController(ApplicationDbContext dbContext, 
            ILogger<DocumentsGeneratorsController> logger, 
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;

            sessionAccesser = new SessionAccesser(dbContext, httpContextAccessor);
            modelManager = new ModelManager(dbContext);
        }

        public IActionResult Orders()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            ViewBag.UserRole = team.GetScoutRole(sessionAccesser.UserPesel);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            List<OrderInfo> orderInfos = _dbContext.OrderInfos
                .Where(oi => oi.TeamIdTeam == sessionAccesser.CurrentTeamId)
                .ToList();

            List<OrdersViewModel_Order> ordersViewModel_Orders = new List<OrdersViewModel_Order>();

            foreach (OrderInfo orderInfo in orderInfos)
            {
                Scout creator = _dbContext.Scouts.Find(orderInfo.ScoutPeselScout);

                ordersViewModel_Orders.Add(new OrdersViewModel_Order()
                {
                    Id = orderInfo.Id,
                    Name = orderInfo.Name,
                    Creator = string.Format("{0} {1}", creator.Name, creator.Surname),
                    CreationDate = orderInfo.CreationDate
                });
            }

            OrdersViewModel ordersViewModel = new OrdersViewModel() { OrderViewModel_Orders = ordersViewModel_Orders.OrderByDescending(order => order.CreationDate).ToList() };

            return View(ordersViewModel);
        }

        [HttpPost]
        public IActionResult Orders(OrdersViewModel ordersViewModel)
        {
            if (ordersViewModel.AddedOrderName != null)
            {
                SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
                formOrder.Name = ordersViewModel.AddedOrderName;
                sessionAccesser.FormOrder = formOrder;

                return Redirect("ordergenerator");
            }

            return Redirect("orders");
        }

        public IActionResult Events()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            ViewBag.UserRole = team.GetScoutRole(sessionAccesser.UserPesel);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            List<Event> events = _dbContext.EventTeams.Where(eventTeam => eventTeam.TeamIdTeam == sessionAccesser.CurrentTeamId).OrderByDescending(et => et.TeamIdTeam).Select(_eventTeam => _eventTeam.Event).ToList();

            ViewBag.EventsList = events;

            return View();
        }

        [HttpPost]
        public IActionResult Events(Event evnt)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            ViewBag.UserRole = team.GetScoutRole(sessionAccesser.UserPesel);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            _dbContext.Events.Add(evnt);
            _dbContext.SaveChanges();

            EventTeam et = new EventTeam
            {
                EventIdEvent = evnt.IdEvent,
                TeamIdTeam = sessionAccesser.CurrentTeamId
            };

            _dbContext.EventTeams.Add(et);
            _dbContext.SaveChanges();

            List<Event> events = _dbContext.EventTeams.Where(eventTeam => eventTeam.TeamIdTeam == sessionAccesser.CurrentTeamId).Select(_eventTeam => _eventTeam.Event).ToList();

            ViewBag.EventsList = events;

            return RedirectToAction("events");
        }

        public IActionResult AttendanceListForm(int eventId)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            ViewBag.UserRole = team.GetScoutRole(sessionAccesser.UserPesel);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            Event evnt = _dbContext.Events.Where(ev => ev.IdEvent == eventId).First();

            ViewBag.Event = evnt;

            ICollection<AttendanceViewModel> scoutsInfo = new List<AttendanceViewModel>();

            int currentTeamId = sessionAccesser.CurrentTeamId;

            List<Scout> scouts = _dbContext.ScoutTeam.Where(scoutTeam => scoutTeam.TeamIdTeam == currentTeamId).Select(_scoutTeam => _scoutTeam.Scout).ToList();

            List<AttendanceViewModel_List> attendance = new List<AttendanceViewModel_List>();

            foreach (Scout scout in scouts)
            {
                Host _host = team.GetScoutsHost(scout.PeselScout);

                attendance.Add(new AttendanceViewModel_List()
                {
                    IdScout = scout.PeselScout,
                    Surname = scout.Surname,
                    Name = scout.Name,
                    Host = _host == null ? "" : _host.Name,
                    EventId = eventId,
                    IsPresent = _dbContext.AttendanceLists.Any(al => al.ScoutIdScout == scout.PeselScout && al.EventIdEvent == eventId)
                });
            }

            AttendanceViewModel attendanceViewModel = new AttendanceViewModel() { AttendanceViewModel_Lists = attendance.OrderBy(at => at.Surname).ToList(), EventId = eventId };

            return View("AttendanceListForm", attendanceViewModel);
        }

        [HttpPost]
        public IActionResult AttendanceListForm(AttendanceViewModel attendanceVM)
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            ViewBag.UserRole = team.GetScoutRole(sessionAccesser.UserPesel);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            Event evnt = _dbContext.Events.Where(ev => ev.IdEvent == attendanceVM.EventId).First();

            ViewBag.Event = evnt;

            foreach (var attendance in attendanceVM.AttendanceViewModel_Lists)
            {
                AttendanceList item = _dbContext.AttendanceLists.FirstOrDefault(al => al.EventIdEvent == attendance.EventId && al.ScoutIdScout == attendance.IdScout);

                if (attendance.IsPresent && item == null)
                {
                    AttendanceList attendanceList = new AttendanceList();
                    attendanceList.EventIdEvent = attendance.EventId;
                    attendanceList.TeamIdTeam = sessionAccesser.CurrentTeamId;
                    attendanceList.ScoutIdScout = attendance.IdScout;

                    _dbContext.AttendanceLists.Add(attendanceList);                    
                } else if (item != null)
                {
                    _dbContext.AttendanceLists.Remove(item);                  
                }
            }

            _dbContext.SaveChanges();

            return AttendanceListForm(attendanceVM.EventId);
        }

        [HttpPost]
        public IActionResult GenerateEmptyListPdf(int idEvent)
        {
            Event evnt = _dbContext.Events.Where(ev => ev.IdEvent == idEvent).First();

            int currentTeamId = sessionAccesser.CurrentTeamId;

            List<Scout> scouts = _dbContext.ScoutTeam.Where(scoutTeam => scoutTeam.TeamIdTeam == currentTeamId).Select(_scoutTeam => _scoutTeam.Scout).ToList();

            new GeneratorPdf().GenerateEmptyList(scouts, evnt);

            return AttendanceListForm(idEvent);
        }

        [HttpPost]
        public IActionResult GenerateListPdf(int idEvent)
        {
            Event evnt = _dbContext.Events.Where(ev => ev.IdEvent == idEvent).First();

            int currentTeamId = sessionAccesser.CurrentTeamId;

            List<Scout> attended = new List<Scout>();

            List<AttendanceList> attendanceLists = _dbContext.AttendanceLists.Where(al => al.EventIdEvent == evnt.IdEvent).ToList();

            foreach (var attendance in attendanceLists)
            {
                Scout scout = _dbContext.Scouts.Where(sc => sc.PeselScout == attendance.ScoutIdScout).First();
                attended.Add(scout);
            }

            new GeneratorPdf().GenerateEventList(evnt, attended);

            return AttendanceListForm(idEvent);
        }

        public IActionResult OrderGenerator()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain }))
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            return View();
        }

        public IActionResult OrderForm_Submit()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            if (!UserHasOneOfRoles(team, new() { TeamRoles.Captain }))
                return Redirect(WebsiteAddresses.AccessDeniedAddress);

            List<Layoff> layoffs = sessionAccesser.FormOrder.LayoffsSaved;
            List<Appointment> appointments = sessionAccesser.FormOrder.AppointmentsSaved;
            List<TrialClosing> trialClosings = sessionAccesser.FormOrder.TrialClosingsSaved;
            List<GamePointsEntry> gamePointsEntries = sessionAccesser.FormOrder.GamePointsEntriesSaved;
            List<Exclusion> exclusions = sessionAccesser.FormOrder.ExclusionsSaved;

            foreach (Layoff layoff in layoffs == null ? new() : layoffs)
                try
                {
                    team.Layoff(layoff);
                }
                catch(Lib.Exceptions.LayoffRoleMismatchException)
                {
                };

            foreach (Appointment appointment in appointments == null ? new() : appointments)
                team.Appoint(appointment);

            foreach (TrialClosing trialClosing in trialClosings == null ? new() : trialClosings)
                team.CloseATrial(trialClosing);

            foreach(GamePointsEntry gamePointsEntry in gamePointsEntries)
            {
                Points gamePoints = new()
                {
                    OrderId = "",
                    ScoutPeselScout = gamePointsEntry.ScoutPesel,
                    Ammount = gamePointsEntry.Points,
                    DateAcquirement = DateTime.Now
                };

                _dbContext.Points.Add(gamePoints);
            }
            _dbContext.SaveChanges();

            foreach (Exclusion exclusion in exclusions == null ? new() : exclusions)
                team.RemoveScout(exclusion.ScoutPesel);

            FormOrder formOrder = new FormOrder()
            {
                CreationDate = sessionAccesser.FormOrder.CreationDate,
                Location = sessionAccesser.FormOrder.CreationPlace,
                OrderNumber = sessionAccesser.FormOrder.Name,
                TrialClosings = sessionAccesser.FormOrder.TrialClosingsSaved,
                Appointments = sessionAccesser.FormOrder.AppointmentsSaved,
                Exclusions = sessionAccesser.FormOrder.ExclusionsSaved,
                Layoffs = sessionAccesser.FormOrder.LayoffsSaved,
                GamePointsEntries = sessionAccesser.FormOrder.GamePointsEntriesSaved,
                ReprimendsAndPraises = sessionAccesser.FormOrder.ReprimendsAndPraisesSaved,
                TrialOpenings = sessionAccesser.FormOrder.TrialOpenings,
                Other = sessionAccesser.FormOrder.Other
            };

            formOrder.OrderNumber = formOrder.OrderNumber.Replace("/", " ");
            team.AddOrder(formOrder, sessionAccesser.UserPesel, sessionAccesser.FormOrder.CreationPlace);

            sessionAccesser.FormOrder = new SessionFormOrderContext();

            return Redirect("orders");
        }

        [HttpPost]
        public IActionResult GenerateOrderPdf(int orderId)
        {
            OrderInfo orderInfo = _dbContext.OrderInfos.Find(orderId);
            Order order = _dbContext.Orders.Find(orderInfo.OrderId);

            FormOrder formOrder = JsonConvert.DeserializeObject<FormOrder>(order.Contents);
            
            formOrder.CreationDate = orderInfo.CreationDate;
            formOrder.Location = orderInfo.Location;
            formOrder.OrderNumber = orderInfo.Name;
            formOrder.TeamName = _dbContext.Teams.Find(orderInfo.TeamIdTeam).Name;

            List<Layoff> layoffs = formOrder.Layoffs == null ? new() : formOrder.Layoffs;
            List<Appointment> appointments = formOrder.Appointments == null ? new() : formOrder.Appointments;
            List<TrialClosing> trialClosings = formOrder.TrialClosings == null ? new() : formOrder.TrialClosings;
            List<TrialOpening> trialOpenings = formOrder.TrialOpenings == null ? new() : formOrder.TrialOpenings;
            List<ReprimendsAndPraises> reprimendsAndPraises = formOrder.ReprimendsAndPraises == null ? new() : formOrder.ReprimendsAndPraises;
            List<Exclusion> exclusions = formOrder.Exclusions == null ? new() : formOrder.Exclusions;
            
            foreach(Layoff layoff in layoffs)
                layoff.RoleName = TeamRoles.TeamRolesTranslations[layoff.Role];

            formOrder.Layoffs = layoffs;

            foreach (Appointment appointment in appointments)
                appointment.RoleName = TeamRoles.TeamRolesTranslations[appointment.Role];

            formOrder.Appointments = appointments;

            foreach(TrialClosing trialClosing in trialClosings)
            {
                if(trialClosing.TrialType == TrialTypes.ScoutCross)
                {
                    trialClosing.TrialType = "krzyz harcerski";
                    trialClosing.TrialName = "";
                }
                else
                {
                    if (trialClosing.TrialType == TrialTypes.Rank)
                    {
                        trialClosing.TrialType = "stopien";

                        if (ScoutRanks.ScoutRanksList.Contains(trialClosing.Rank))
                            trialClosing.TrialName = ScoutRanks.ScoutRanksTranslation[trialClosing.Rank];
                    }

                    if (trialClosing.TrialType == TrialTypes.Ability)
                    {
                        trialClosing.TrialType = "sprawnosc";

                        Achievement achievement = _dbContext.Achievements.Find(int.Parse(trialClosing.Ability));

                        if (achievement != null)
                            trialClosing.Ability = ScoutAbilities.ScoutAbilitiesTranslation[achievement.Type];
                    }
                }
            }

            formOrder.TrialClosings = trialClosings;

            foreach (TrialOpening trialOpening in trialOpenings)
            {
                if (trialOpening.TrialType == TrialTypes.ScoutCross)
                {
                    trialOpening.TrialType = "krzyz harcerski";
                    trialOpening.TrialName = "";
                }
                else
                {
                    if (trialOpening.TrialType == TrialTypes.Rank)
                    {
                        trialOpening.TrialType = "stopien";

                        if (ScoutRanks.ScoutRanksList.Contains(trialOpening.Rank))
                            trialOpening.TrialName = ScoutRanks.ScoutRanksTranslation[trialOpening.Rank];
                    }

                    if (trialOpening.TrialType == TrialTypes.Ability)
                    {
                        trialOpening.TrialType = "sprawnosc";

                        Achievement achievement = _dbContext.Achievements.Find(int.Parse(trialOpening.Ability));

                        if (achievement != null)
                            trialOpening.Ability = ScoutAbilities.ScoutAbilitiesTranslation[achievement.Type];
                    }
                }
            }

            formOrder.TrialOpenings = trialOpenings;

            foreach(ReprimendsAndPraises reprimendsAndPraise in reprimendsAndPraises)
            {
                if (reprimendsAndPraise.Type == "praise")
                    reprimendsAndPraise.Type = "pochwala";
                if (reprimendsAndPraise.Type == "reprimand")
                    reprimendsAndPraise.Type = "reprymenda";
                if (reprimendsAndPraise.Type == "distinction")
                    reprimendsAndPraise.Type = "wyróżnienie";
            }

            formOrder.ReprimendsAndPraises = reprimendsAndPraises;

            foreach (Exclusion exclusion in exclusions)
            {
                if (exclusion.Reason == "resignation")
                    exclusion.Reason = "rezygnacja";
                if (exclusion.Reason == "non-payment of contributions")
                    exclusion.Reason = "nieoplacenie skladek";
                if (exclusion.Reason == "banishment")
                    exclusion.Reason = "wydalenie";
                if (exclusion.Reason == "other")
                    exclusion.Reason = "inne";
            }

            formOrder.Exclusions = exclusions;

            formOrder.Location = "LOKALIZACJA";

            new GeneratorPdf().GenerateOrder(formOrder);

            return Redirect("orders");
        }

        public IActionResult Appointments()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            List<Scout> scoutsInTheTeam = team.GetScouts();
            List<string> peselsOfScoutsThatAreAlreadyInTheAppointment = sessionAccesser.FormOrder
                .Appointments
                .Select(a => a.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded =
                scoutsInTheTeam.Where(s => !peselsOfScoutsThatAreAlreadyInTheAppointment.Contains(s.PeselScout)).ToList();

            List<Host> hostsFromTheTeam = team.Hosts.ToList();

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<SelectListItem> dropDownList_Roles = new List<SelectListItem>()
            {
                new() {Text = "przyboczny", Value = TeamRoles.ViceCaptain},
                new() {Text = "chorąży drużyny", Value = TeamRoles.Ensign},
                new() {Text = "kwatermistrz", Value = TeamRoles.Quatermaster},
                new() {Text = "kronikarz", Value = TeamRoles.Chronicler},
                new() {Text = "zastępowy", Value = TeamRoles.HostCaptain}
            };
            List<SelectListItem> dropDownList_Hosts = new List<SelectListItem>();

            foreach (Scout scout in scoutsThatCanBeAdded)
            {
                dropDownList_Scouts.Add(new SelectListItem()
                {
                    Text = string.Format("{0} {1} ({2})", scout.Surname, scout.Name, scout.PeselScout),
                    Value = scout.IdentityId
                });
            }

            foreach (Host host in hostsFromTheTeam)
            {
                dropDownList_Hosts.Add(new SelectListItem()
                {
                    Text = host.Name,
                    Value = host.IdHost.ToString()
                });
            }

            int numberOfScoutsInTheTeam = scoutsInTheTeam.Count();

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;
            ViewBag.DropDownList_Roles = dropDownList_Roles;
            ViewBag.DropDownList_Hosts = dropDownList_Hosts;

            ViewBag.AreThereScoutsToAdd = !(peselsOfScoutsThatAreAlreadyInTheAppointment.Count() == (numberOfScoutsInTheTeam - 1));
            ViewBag.OrderName = sessionAccesser.FormOrder.Name;
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View(new AppointmentsViewModel() { Appointments = sessionAccesser.FormOrder.Appointments });
        }
        [HttpPost]
        public IActionResult Appointments(AppointmentsViewModel appointmentsViewModel)
        {
            appointmentsViewModel.Appointments.Reverse();

            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            formOrder.Appointments = appointmentsViewModel.Appointments;
            formOrder.AppointmentsSaved = appointmentsViewModel.Appointments;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("appointments");
        }
        public IActionResult AppointmentsRevert()
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            if (formOrder.AppointmentsSaved == null)
            {
                formOrder.AppointmentsSaved = new();
            }

            formOrder.Appointments = formOrder.AppointmentsSaved;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("appointments");
        }
        [HttpPost]
        public IActionResult AppointmentsAdd(AppointmentsViewModel appointmentsViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            List<Appointment> appointments = AddScoutEntryToFormOrderViewModel(appointmentsViewModel, formOrder.Appointments.ConvertAll(x => (IOrderElement)x))
                .ConvertAll(x => (Appointment)x);

            if (appointments != null)
            {
                formOrder.Appointments = appointments;
                sessionAccesser.FormOrder = formOrder;
            }

            sessionAccesser.FormOrder = formOrder;

            return Redirect("appointments");
        }
        [HttpPost]
        public IActionResult AppointmentsRemove(AppointmentsViewModel appointmentsViewModel, string scoutId)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            List<Appointment> appointments = RemoveScoutEntryFormOrderViewModel(appointmentsViewModel, scoutId).ConvertAll(x => (Appointment)x);

            if (appointments != null)
            {
                formOrder.Appointments = appointments;
                sessionAccesser.FormOrder = formOrder;
            }

            return Redirect("appointments");
        }

        public IActionResult Exclusions()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            List<Scout> scoutsInTheTeam = team.GetScouts();
            List<string> peselsOfScoutsThatAreAlreadyInTheExclusions = sessionAccesser.FormOrder
                .Exclusions
                .Select(e => e.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded =
                scoutsInTheTeam.Where(s => !peselsOfScoutsThatAreAlreadyInTheExclusions.Contains(s.PeselScout) && s.PeselScout != sessionAccesser.UserPesel).ToList();

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<SelectListItem> dropDownList_Reasons = new List<SelectListItem>()
            {
                new() {Text = "rezygnacja", Value = "resignation"},
                new() {Text = "niepłacenie składek", Value = "non-payment of contributions"},
                new() {Text = "wydalenie", Value = "banishment"},
                new() {Text = "inne", Value = "other"}
            };

            foreach (Scout scout in scoutsThatCanBeAdded)
            {
                dropDownList_Scouts.Add(new SelectListItem()
                {
                    Text = string.Format("{0} {1} ({2})", scout.Surname, scout.Name, scout.PeselScout),
                    Value = scout.IdentityId
                });
            }


            int numberOfScoutsInTheTeam = scoutsInTheTeam.Count();

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;
            ViewBag.DropDownList_Reasons = dropDownList_Reasons;

            ViewBag.AreThereScoutsToAdd = !(peselsOfScoutsThatAreAlreadyInTheExclusions.Count() == (numberOfScoutsInTheTeam - 1));
            ViewBag.OrderName = sessionAccesser.FormOrder.Name;
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View(new ExclusionsViewModel() { Exclusions = sessionAccesser.FormOrder.Exclusions });
        }
        [HttpPost]
        public IActionResult Exclusions(ExclusionsViewModel exclusionsViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            formOrder.Exclusions = exclusionsViewModel.Exclusions;
            formOrder.ExclusionsSaved = exclusionsViewModel.Exclusions;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("exclusions");
        }
        public IActionResult ExclusionsRevert()
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            if (formOrder.ExclusionsSaved == null)
            {
                formOrder.ExclusionsSaved = new();
            }

            formOrder.Exclusions = formOrder.ExclusionsSaved;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("exclusions");
        }
        [HttpPost]
        public IActionResult ExclusionsAdd(ExclusionsViewModel exclusionsViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            List<Exclusion> exclusions = AddScoutEntryToFormOrderViewModel(exclusionsViewModel, formOrder.Exclusions.ConvertAll(x => (IOrderElement)x))
                .ConvertAll(x => (Exclusion)x);

            if (exclusions != null)
            {
                formOrder.Exclusions = exclusions;
                sessionAccesser.FormOrder = formOrder;
            }

            return Redirect("exclusions");
        }
        [HttpPost]
        public IActionResult ExclusionsRemove(ExclusionsViewModel exclusionsViewModel, string scoutId)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            List<Exclusion> exclusions = RemoveScoutEntryFormOrderViewModel(exclusionsViewModel, scoutId).ConvertAll(x => (Exclusion)x);

            if(exclusions != null)
            {
                formOrder.Exclusions = exclusions;
                sessionAccesser.FormOrder = formOrder;
            }

            return Redirect("exclusions");
        }

        public IActionResult Layoffs()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            List<Scout> scoutsInTheTeam = team.GetScouts();
            List<string> peselsOfScoutsThatAreAlreadyInTheLayoffs = sessionAccesser.FormOrder
                .Layoffs
                .Select(l => l.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded =
                scoutsInTheTeam.Where(s => !peselsOfScoutsThatAreAlreadyInTheLayoffs.Contains(s.PeselScout) && s.PeselScout != sessionAccesser.UserPesel).ToList();

            List<Host> hostsFromTheTeam = team.Hosts.ToList();

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<SelectListItem> dropDownList_Roles = new List<SelectListItem>()
            {
                new() {Text = "przyboczny", Value = TeamRoles.ViceCaptain},
                new() {Text = "chorąży drużyny", Value = TeamRoles.Ensign},
                new() {Text = "kwatermistrz", Value = TeamRoles.Quatermaster},
                new() {Text = "kronikarz", Value = TeamRoles.Chronicler},
                new() {Text = "zastępowy", Value = TeamRoles.HostCaptain}
            };
            List<SelectListItem> dropDownList_Hosts = new List<SelectListItem>();

            foreach (Scout scout in scoutsThatCanBeAdded)
            {
                dropDownList_Scouts.Add(new SelectListItem()
                {
                    Text = string.Format("{0} {1} ({2})", scout.Surname, scout.Name, scout.PeselScout),
                    Value = scout.IdentityId
                });
            }

            foreach (Host host in hostsFromTheTeam)
            {
                dropDownList_Hosts.Add(new SelectListItem()
                {
                    Text = host.Name,
                    Value = host.IdHost.ToString()
                });
            }

            int numberOfScoutsInTheTeam = scoutsInTheTeam.Count();

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;
            ViewBag.DropDownList_Roles = dropDownList_Roles;
            ViewBag.DropDownList_Hosts = dropDownList_Hosts;

            ViewBag.AreThereScoutsToAdd = !(peselsOfScoutsThatAreAlreadyInTheLayoffs.Count() == (numberOfScoutsInTheTeam - 1));
            ViewBag.OrderName = sessionAccesser.FormOrder.Name;
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View(new LayoffsViewModel() { Layoffs = sessionAccesser.FormOrder.Layoffs });
        }
        [HttpPost]
        public IActionResult Layoffs(LayoffsViewModel layoffsViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            formOrder.Layoffs = layoffsViewModel.Layoffs;
            formOrder.LayoffsSaved = layoffsViewModel.Layoffs;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("layoffs");
        }
        public IActionResult LayoffsRevert()
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            if (formOrder.LayoffsSaved == null)
            {
                formOrder.LayoffsSaved = new();
            }

            formOrder.Layoffs = formOrder.LayoffsSaved;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("layoffs");
        }
        [HttpPost]
        public IActionResult LayoffsAdd(LayoffsViewModel layoffsViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            List<Layoff> layoffs = AddScoutEntryToFormOrderViewModel(layoffsViewModel, formOrder.Layoffs.ConvertAll(x => (IOrderElement)x))
                .ConvertAll(x => (Layoff)x);

            if (layoffs != null)
            {
                formOrder.Layoffs = layoffs;
                sessionAccesser.FormOrder = formOrder;
            }

            sessionAccesser.FormOrder = formOrder;

            return Redirect("layoffs");
        }
        [HttpPost]
        public IActionResult LayoffsRemove(LayoffsViewModel layoffsViewModel, string scoutId)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            List<Layoff> layoffs = RemoveScoutEntryFormOrderViewModel(layoffsViewModel, scoutId).ConvertAll(x => (Layoff)x);

            if (layoffs != null)
            {
                formOrder.Layoffs = layoffs;
                sessionAccesser.FormOrder = formOrder;
            }

            return Redirect("layoffs");
        }

        public IActionResult Other()
        {
            ViewBag.OrderName = sessionAccesser.FormOrder.Name;
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View(new OtherViewModel() { Other = sessionAccesser.FormOrder.Other });
        }
        [HttpPost]
        public IActionResult Other(OtherViewModel otherViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            formOrder.Other = otherViewModel.Other;
            formOrder.OtherSaved = otherViewModel.Other;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("other");
        }
        public IActionResult OtherRevert()
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            if (formOrder.Other == null)
            {
                formOrder.Other = new();
            }

            formOrder.Other = formOrder.OtherSaved;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("other");
        }

        public IActionResult GamePoints()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            List<Scout> scoutsInTheTeam = team.GetScouts();

            List<string> peselsOfScoutsThatAreAlreadyHaveAGamePointEntry = sessionAccesser.FormOrder
                .GamePointsEntries
                .Select(gpe => gpe.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded =
                scoutsInTheTeam.Where(s => !peselsOfScoutsThatAreAlreadyHaveAGamePointEntry.Contains(s.PeselScout)).ToList();

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();

            foreach (Scout scout in scoutsThatCanBeAdded)
            {
                dropDownList_Scouts.Add(new SelectListItem()
                {
                    Text = string.Format("{0} {1} ({2})", scout.Surname, scout.Name, scout.PeselScout),
                    Value = scout.IdentityId
                });
            }

            int numberOfScoutsInTheTeam = scoutsInTheTeam.Count();

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;
            ViewBag.OrderName = sessionAccesser.FormOrder.Name;
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            ViewBag.AreThereScoutsToAdd = !(peselsOfScoutsThatAreAlreadyHaveAGamePointEntry.Count() == numberOfScoutsInTheTeam);

            return View(new GamePointsViewModel() { GamePointEntries = sessionAccesser.FormOrder.GamePointsEntries });
        }
        [HttpPost]
        public IActionResult GamePoints(GamePointsViewModel gamePointsViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            formOrder.GamePointsEntries = gamePointsViewModel.GamePointEntries;
            formOrder.GamePointsEntriesSaved = gamePointsViewModel.GamePointEntries;

            sessionAccesser.FormOrder = formOrder;

            return RedirectToAction("gamepoints");
        }
        public IActionResult GamePointsRevert()
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            if (formOrder.GamePointsEntriesSaved == null)
            {
                formOrder.GamePointsEntriesSaved = new();
            }

            formOrder.GamePointsEntries = formOrder.GamePointsEntriesSaved;

            sessionAccesser.FormOrder = formOrder;

            return RedirectToAction("gamepoints");
        }
        [HttpPost]
        public IActionResult GamePointsAdd(GamePointsViewModel gamePointsViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            List<GamePointsEntry> gamePointsEntries = AddScoutEntryToFormOrderViewModel(gamePointsViewModel, formOrder.GamePointsEntries.ConvertAll(x => (IOrderElement)x))
                .ConvertAll(x => (GamePointsEntry)x);

            if (gamePointsEntries != null)
            {
                formOrder.GamePointsEntries = gamePointsEntries;
                sessionAccesser.FormOrder = formOrder;
            }

            sessionAccesser.FormOrder = formOrder;

            return RedirectToAction("gamepoints");
        }
        [HttpPost]
        public IActionResult GamePointsRemove(GamePointsViewModel gamePointsViewModel, string scoutId)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            List<GamePointsEntry> gamePointsEntries = RemoveScoutEntryFormOrderViewModel(gamePointsViewModel, scoutId).ConvertAll(x => (GamePointsEntry)x);

            if (gamePointsEntries != null)
            {
                formOrder.GamePointsEntries = gamePointsEntries;
                sessionAccesser.FormOrder = formOrder;
            }

            return RedirectToAction("gamepoints");
        }

        public IActionResult ReprimendsAndPraises()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            List<Scout> scoutsInTheTeam = team.GetScouts();
            List<string> peselsOfScoutsThatAreAlreadyInTheReprimendsAndPraises = sessionAccesser.FormOrder
                .ReprimendsAndPraises
                .Select(rap => rap.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded =
                scoutsInTheTeam.Where(s => !peselsOfScoutsThatAreAlreadyInTheReprimendsAndPraises.Contains(s.PeselScout)).ToList();

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<SelectListItem> dropDownList_Types = new List<SelectListItem>()
            {
                new() {Text = "pochwała", Value = "praise"},
                new() {Text = "nagana", Value = "reprimand"},
                new() {Text = "wyróżnienie", Value = "distinction"}
            };

            foreach (Scout scout in scoutsThatCanBeAdded)
            {
                dropDownList_Scouts.Add(new SelectListItem()
                {
                    Text = string.Format("{0} {1} ({2})", scout.Surname, scout.Name, scout.PeselScout),
                    Value = scout.IdentityId
                });
            }

            int numberOfScoutsInTheTeam = scoutsInTheTeam.Count();

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;
            ViewBag.DropDownList_Types = dropDownList_Types;

            ViewBag.AreThereScoutsToAdd = !(peselsOfScoutsThatAreAlreadyInTheReprimendsAndPraises.Count() == numberOfScoutsInTheTeam);
            ViewBag.OrderName = sessionAccesser.FormOrder.Name;
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View(new ReprimendsAndPraisesViewModel() { ReprimendsAndPraises = sessionAccesser.FormOrder.ReprimendsAndPraises });
        }
        [HttpPost]
        public IActionResult ReprimendsAndPraises(ReprimendsAndPraisesViewModel reprimendsAndPraisesViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            formOrder.ReprimendsAndPraises = reprimendsAndPraisesViewModel.ReprimendsAndPraises;
            formOrder.ReprimendsAndPraisesSaved = reprimendsAndPraisesViewModel.ReprimendsAndPraises;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("reprimendsandpraises");
        }
        public IActionResult ReprimendsAndPraisesRevert()
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            if (formOrder.ReprimendsAndPraisesSaved == null)
            {
                formOrder.ReprimendsAndPraisesSaved = new();
            }

            formOrder.ReprimendsAndPraises = formOrder.ReprimendsAndPraisesSaved;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("reprimendsandpraises");
        }
        [HttpPost]
        public IActionResult ReprimendsAndPraisesAdd(ReprimendsAndPraisesViewModel reprimendsAndPraisesViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            List<ReprimendsAndPraises> reprimendsAndPraises = AddScoutEntryToFormOrderViewModel(reprimendsAndPraisesViewModel, formOrder.ReprimendsAndPraises.ConvertAll(x => (IOrderElement)x))
                .ConvertAll(x => (ReprimendsAndPraises)x);

            if (reprimendsAndPraises != null)
            {
                formOrder.ReprimendsAndPraises = reprimendsAndPraises;
                sessionAccesser.FormOrder = formOrder;
            }

            sessionAccesser.FormOrder = formOrder;

            return Redirect("reprimendsandpraises");
        }
        [HttpPost]
        public IActionResult ReprimendsAndPraisesRemove(ReprimendsAndPraisesViewModel reprimendsAndPraisesViewModel, string scoutId)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            List<ReprimendsAndPraises> reprimendsAndPraises = RemoveScoutEntryFormOrderViewModel(reprimendsAndPraisesViewModel, scoutId).ConvertAll(x => (ReprimendsAndPraises)x);

            if (reprimendsAndPraises != null)
            {
                formOrder.ReprimendsAndPraises = reprimendsAndPraises;
                sessionAccesser.FormOrder = formOrder;
            }

            return Redirect("reprimendsandpraises");
        }

        public IActionResult TrialClosings()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            List<Scout> scoutsInTheTeam = team.GetScouts();
            List<string> peselsOfScoutsThatAreAlreadyInTheTrialClosings = sessionAccesser.FormOrder
                .TrialClosings
                .Select(tc => tc.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded =
                scoutsInTheTeam.Where(s => !peselsOfScoutsThatAreAlreadyInTheTrialClosings.Contains(s.PeselScout)).ToList();

            List<Host> hostsFromTheTeam = team.Hosts.ToList();

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<SelectListItem> dropDownList_TrialTypes = new List<SelectListItem>()
            {
                new() {Text = "krzyż harcerski", Value = TrialTypes.ScoutCross},
                new() {Text = "stopień", Value = TrialTypes.Rank},
                new() {Text = "sprawność", Value = TrialTypes.Ability}
            };
            List<SelectListItem> dropDownList_Ranks = new List<SelectListItem>()
            {
                new() {Text = "młodzik/ochotniczka", Value = ScoutRanks.Rank1},
                new() {Text = "wywiadowca/tropicielka", Value = ScoutRanks.Rank2},
                new() {Text = "odkrywca/pionierka", Value = ScoutRanks.Rank3},
                new() {Text = "ćwik/samarytanka", Value = ScoutRanks.Rank4},
                new() {Text = "harcerz orli/harcerka orla", Value = ScoutRanks.Rank5},
                new() {Text = "harcerz rzeczypospolitej/harcerka rzeczypospolitej", Value = ScoutRanks.Rank6}
            };
            List<SelectListItem> dropDownList_Abilities = new List<SelectListItem>()
            {
                new() {Text = "higienista", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Hygenist).First().IdAchievement.ToString()},
                new() {Text = "sanitariusz", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Paramedic).First().IdAchievement.ToString()},
                new() {Text = "ratownik", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Lifesaver).First().IdAchievement.ToString()},
                new() {Text = "ognik", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Glimmer).First().IdAchievement.ToString()},
                new() {Text = "strażnik ognia", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.FireGuard).First().IdAchievement.ToString()},
                new() {Text = "mistrz ognisk", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.FireplaceMaster).First().IdAchievement.ToString()},
                new() {Text = "znawca musztry", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.DrillExpert).First().IdAchievement.ToString()},
                new() {Text = "mistrz musztry", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.DrillMaster).First().IdAchievement.ToString()},
                new() {Text = "igiełka", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Needle).First().IdAchievement.ToString()},
                new() {Text = "krawiec", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Tailor).First().IdAchievement.ToString()},
                new() {Text = "młody pływak", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.YoungSwimmer).First().IdAchievement.ToString()},
                new() {Text = "pływak", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Swimmer).First().IdAchievement.ToString()},
                new() {Text = "pływak doskonały", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.ExcellentSwimmer).First().IdAchievement.ToString()},
                new() {Text = "internauta", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Internaut).First().IdAchievement.ToString()},
                new() {Text = "historyk rodzinny", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.FamilyHistorian).First().IdAchievement.ToString()},
                new() {Text = "europejczyk", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.European).First().IdAchievement.ToString()},
                new() {Text = "lider zdrowia", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.HealthLeader).First().IdAchievement.ToString()},
                new() {Text = "przyjaciel przyrody", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.NatureFriend).First().IdAchievement.ToString()},
                new() {Text = "fotograf", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Photograph).First().IdAchievement.ToString()},
            };

            foreach (Scout scout in scoutsThatCanBeAdded)
            {
                dropDownList_Scouts.Add(new SelectListItem()
                {
                    Text = string.Format("{0} {1} ({2})", scout.Surname, scout.Name, scout.PeselScout),
                    Value = scout.IdentityId
                });
            }

            int numberOfScoutsInTheTeam = scoutsInTheTeam.Count();

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;
            ViewBag.DropDownList_TrialTypes = dropDownList_TrialTypes;
            ViewBag.DropDownList_Ranks = dropDownList_Ranks;
            ViewBag.DropDownList_Abilities = dropDownList_Abilities;

            ViewBag.AreThereScoutsToAdd = !(peselsOfScoutsThatAreAlreadyInTheTrialClosings.Count() == numberOfScoutsInTheTeam);
            ViewBag.OrderName = sessionAccesser.FormOrder.Name;
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View(new TrialClosingsViewModel() { TrialClosings = sessionAccesser.FormOrder.TrialClosings });
        }
        [HttpPost]
        public IActionResult TrialClosings(TrialClosingsViewModel trialClosingsViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            formOrder.TrialClosings = trialClosingsViewModel.TrialClosings;
            formOrder.TrialClosingsSaved = trialClosingsViewModel.TrialClosings;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("trialclosings");
        }
        public IActionResult TrialClosingsRevert()
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            if (formOrder.TrialClosingsSaved == null)
            {
                formOrder.TrialClosingsSaved = new();
            }

            formOrder.TrialClosings = formOrder.TrialClosingsSaved;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("trialclosings");
        }
        [HttpPost]
        public IActionResult TrialClosingsAdd(TrialClosingsViewModel trialClosingsViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            List<TrialClosing> trialClosings = AddScoutEntryToFormOrderViewModel(trialClosingsViewModel, formOrder.TrialClosings.ConvertAll(x => (IOrderElement)x))
                .ConvertAll(x => (TrialClosing) x);

            if(trialClosings != null)
            {
                formOrder.TrialClosings = trialClosings;
                sessionAccesser.FormOrder = formOrder;
            }

            return Redirect("trialclosings");
        }
        [HttpPost]
        public IActionResult TrialClosingsRemove(TrialClosingsViewModel trialClosingsViewModel, string scoutId)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            List<TrialClosing> trialClosings = RemoveScoutEntryFormOrderViewModel(trialClosingsViewModel, scoutId).ConvertAll(x => (TrialClosing)x);

            if (trialClosings != null)
            {
                formOrder.TrialClosings = trialClosings;
                sessionAccesser.FormOrder = formOrder;
            }

            return Redirect("trialclosings");
        }

        public IActionResult TrialOpenings()
        {
            Team team = GetTeam(_dbContext, sessionAccesser.CurrentTeamId);

            List<string> peselsOfScoutsThatAreAlreadyInTheTrialOpenings = sessionAccesser.FormOrder
                .TrialOpenings
                .Select(to => to.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded =
                team.GetScouts().Where(s => !peselsOfScoutsThatAreAlreadyInTheTrialOpenings.Contains(s.PeselScout)).ToList();

            List<Host> hostsFromTheTeam = team.Hosts.ToList();

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<SelectListItem> dropDownList_TrialTypes = new List<SelectListItem>()
            {
                new() {Text = "krzyż harcerski", Value = TrialTypes.ScoutCross},
                new() {Text = "stopień", Value = TrialTypes.Rank},
                new() {Text = "sprawność", Value = TrialTypes.Ability}
            };
            List<SelectListItem> dropDownList_Ranks = new List<SelectListItem>()
            {
                new() {Text = "młodzik/ochotniczka", Value = ScoutRanks.Rank1},
                new() {Text = "wywiadowca/tropicielka", Value = ScoutRanks.Rank2},
                new() {Text = "odkrywca/pionierka", Value = ScoutRanks.Rank3},
                new() {Text = "ćwik/samarytanka", Value = ScoutRanks.Rank4},
                new() {Text = "harcerz orli/harcerka orla", Value = ScoutRanks.Rank5},
                new() {Text = "harcerz rzeczypospolitej/harcerka rzeczypospolitej", Value = ScoutRanks.Rank6}
            };
            List<SelectListItem> dropDownList_Abilities = new List<SelectListItem>()
            {
                new() {Text = "higienista", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Hygenist).First().IdAchievement.ToString()},
                new() {Text = "sanitariusz", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Paramedic).First().IdAchievement.ToString()},
                new() {Text = "ratownik", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Lifesaver).First().IdAchievement.ToString()},
                new() {Text = "ognik", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Glimmer).First().IdAchievement.ToString()},
                new() {Text = "strażnik ognia", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.FireGuard).First().IdAchievement.ToString()},
                new() {Text = "mistrz ognisk", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.FireplaceMaster).First().IdAchievement.ToString()},
                new() {Text = "znawca musztry", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.DrillExpert).First().IdAchievement.ToString()},
                new() {Text = "mistrz musztry", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.DrillMaster).First().IdAchievement.ToString()},
                new() {Text = "igiełka", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Needle).First().IdAchievement.ToString()},
                new() {Text = "krawiec", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Tailor).First().IdAchievement.ToString()},
                new() {Text = "młody pływak", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.YoungSwimmer).First().IdAchievement.ToString()},
                new() {Text = "pływak", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Swimmer).First().IdAchievement.ToString()},
                new() {Text = "pływak doskonały", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.ExcellentSwimmer).First().IdAchievement.ToString()},
                new() {Text = "internauta", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Internaut).First().IdAchievement.ToString()},
                new() {Text = "historyk rodzinny", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.FamilyHistorian).First().IdAchievement.ToString()},
                new() {Text = "europejczyk", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.European).First().IdAchievement.ToString()},
                new() {Text = "lider zdrowia", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.HealthLeader).First().IdAchievement.ToString()},
                new() {Text = "przyjaciel przyrody", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.NatureFriend).First().IdAchievement.ToString()},
                new() {Text = "fotograf", Value = _dbContext.Achievements.Where(a => a.Type == ScoutAbilities.Photograph).First().IdAchievement.ToString()},
            };

            foreach (Scout scout in scoutsThatCanBeAdded)
            {
                dropDownList_Scouts.Add(new SelectListItem()
                {
                    Text = string.Format("{0} {1} ({2})", scout.Surname, scout.Name, scout.PeselScout),
                    Value = scout.IdentityId
                });
            }

            int numberOfScoutsInTheTeam = team.GetScouts().Count();

            ViewBag.DropDownList_Scouts = dropDownList_Scouts;
            ViewBag.DropDownList_TrialTypes = dropDownList_TrialTypes;
            ViewBag.DropDownList_Ranks = dropDownList_Ranks;
            ViewBag.DropDownList_Abilities = dropDownList_Abilities;

            ViewBag.AreThereScoutsToAdd = !(peselsOfScoutsThatAreAlreadyInTheTrialOpenings.Count() == numberOfScoutsInTheTeam);
            ViewBag.OrderName = sessionAccesser.FormOrder.Name;
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View(new TrialOpeningsViewModel() { TrialOpenings = sessionAccesser.FormOrder.TrialOpenings });
        }
        [HttpPost]
        public IActionResult TrialOpenings(TrialOpeningsViewModel trialOpeningsViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            formOrder.TrialOpenings = trialOpeningsViewModel.TrialOpenings;
            formOrder.TrialOpeningsSaved = trialOpeningsViewModel.TrialOpenings;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("trialopenings");
        }
        public IActionResult TrialOpeningsRevert()
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            if (formOrder.TrialOpeningsSaved == null)
            {
                formOrder.TrialOpeningsSaved = new();
            }

            formOrder.TrialOpenings = formOrder.TrialOpeningsSaved;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("trialopenings");
        }
        [HttpPost]
        public IActionResult TrialOpeningsAdd(TrialOpeningsViewModel trialOpeningsViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            List<TrialOpening> trialOpenings = AddScoutEntryToFormOrderViewModel(trialOpeningsViewModel, formOrder.TrialOpenings.ConvertAll(x => (IOrderElement)x))
                .ConvertAll(x => (TrialOpening)x);

            if (trialOpenings != null)
            {
                formOrder.TrialOpenings = trialOpenings;
                sessionAccesser.FormOrder = formOrder;
            }

            sessionAccesser.FormOrder = formOrder;

            return Redirect("trialopenings");
        }
        [HttpPost]
        public IActionResult TrialOpeningsRemove(TrialOpeningsViewModel trialOpeningsViewModel, string scoutId)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            List<TrialOpening> trialOpenings = RemoveScoutEntryFormOrderViewModel(trialOpeningsViewModel, scoutId).ConvertAll(x => (TrialOpening)x);

            if (trialOpenings != null)
            {
                formOrder.TrialOpenings = trialOpenings;
                sessionAccesser.FormOrder = formOrder;
            }

            return Redirect("trialopenings");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<IOrderElement> AddScoutEntryToFormOrderViewModel(IFormOrderViewModel viewModel, List<IOrderElement> list)
        {
            Scout scout = GetScoutById(_dbContext, viewModel.GetScoutId());

            bool scoutIsInTheTeam = _dbContext.ScoutTeam.Any(st => st.TeamIdTeam == sessionAccesser.CurrentTeamId && st.ScoutPeselScout == scout.PeselScout);
            bool scoutIsNotInTheOrderElementList = !list.Select(e => e.GetScoutId()).ToList().Contains(viewModel.GetScoutId());

            if (scoutIsInTheTeam && scoutIsNotInTheOrderElementList)
            {
                viewModel.AddElement(scout.IdentityId, scout.PeselScout, scout.Name, scout.Surname);

                return viewModel.GetList();
            }

            return null;
        }

        private List<IOrderElement> RemoveScoutEntryFormOrderViewModel(IFormOrderViewModel viewModel, string scoutId)
        {
            List<string> idsOfScouts = viewModel.GetList()
                .Select(e => e.GetScoutId())
                .ToList();

            if (idsOfScouts.Contains(scoutId))
            {
                SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
                List<IOrderElement> list = viewModel.GetList();

                list.RemoveAll(a => a.GetScoutId() == scoutId);

                return list;
            }

            return null;
        }

        private bool UserHasOneOfRoles(Team team, List<string> roles)
        {
            return team.ScoutHasOneOfRoles(sessionAccesser.UserPesel, roles);
        }
    }
}
