using Microsoft.AspNetCore.Authorization;
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
using moja_druzyna.ViewModels;
using moja_druzyna.ViewModels.DocumentsGenerators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static moja_druzyna.ViewModels.DocumentsGenerators.OrdersViewModel;

namespace moja_druzyna.Controllers
{
    public class DocumentsGeneratorsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DocumentsGeneratorsController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly SessionAccesser sessionAccesser;
        private readonly ModelManager modelManager;

        private static OrderFormViewModel orderFormViewModel = new OrderFormViewModel();

        public DocumentsGeneratorsController(ApplicationDbContext dbContext, 
            ILogger<DocumentsGeneratorsController> logger, 
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;

            sessionAccesser = new SessionAccesser(dbContext, httpContextAccessor);
            modelManager = new ModelManager(dbContext);
        }

        [Authorize(Roles = "captain,vice captain,host captain,ensign,quatermaster,chronicler,scout")]
        public IActionResult Orders()
        {
            ViewBag.UserRole = modelManager.GetScoutRoleInATeam(sessionAccesser.UserPesel, sessionAccesser.CurrentTeamId);
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            List<OrderInfo> orderInfos = _dbContext.OrderInfos
                .Where(orderInfo => orderInfo.TeamIdTeam == sessionAccesser.CurrentTeamId)
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
        [Authorize(Roles = "captain,vice captain,host captain,ensign,quatermaster,chronicler,scout")]
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

        [Authorize(Roles = "captain")]
        public IActionResult OrderGenerator()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            return View();
        }

        [Authorize(Roles = "captain")]
        public async Task<IActionResult> OrderForm_Submit()
        {   
            List<Layoff> layoffs = sessionAccesser.FormOrder.LayoffsSaved;
            List<Appointment> appointments = sessionAccesser.FormOrder.AppointmentsSaved;
            List<TrialClosing> trialClosings = sessionAccesser.FormOrder.TrialClosingsSaved;
            List<Exclusion> exclusions = sessionAccesser.FormOrder.ExclusionsSaved;
            int teamId = sessionAccesser.CurrentTeamId;

            foreach (Layoff layoff in layoffs == null ? new() : layoffs)
            {
                if(layoff.UpdateDb(_dbContext, sessionAccesser.CurrentTeamId, true))
                {
                    await SetRole(layoff.ScoutId, "scout");
                }
            }

            foreach (Appointment appointment in appointments == null ? new() : appointments)
            {
                if (appointment.UpdateDb(_dbContext, sessionAccesser.CurrentTeamId, true, _logger))
                {
                    await SetRole(appointment.ScoutId, appointment.Role);
                }
            }

            foreach (TrialClosing trialClosing in trialClosings == null ? new() : trialClosings)
            {
                trialClosing.UpdateDb(_dbContext, sessionAccesser.CurrentTeamId, true, _logger);
            }

            foreach (Exclusion exclusion in exclusions == null ? new() : exclusions)
            {
                exclusion.UpdateDb(_dbContext, sessionAccesser.CurrentTeamId, true);
            }

            if(_dbContext.ScoutTeam.Where(scoutTeam => scoutTeam.TeamIdTeam == sessionAccesser.CurrentTeamId && scoutTeam.Role == "captain").Count() == 0)
            {
                ScoutTeam scoutTeam = _dbContext.ScoutTeam
                    .Where(scoutTeam => scoutTeam.ScoutPeselScout == sessionAccesser.UserPesel && scoutTeam.TeamIdTeam == sessionAccesser.CurrentTeamId)
                    .First();
                ScoutHost scoutHost = _dbContext.ScoutHost
                    .Where(scoutHost => scoutHost.ScoutPeselScout == sessionAccesser.UserPesel)
                    .First();

                scoutTeam.Role = "captain";

                _dbContext.ScoutTeam.Update(scoutTeam);
                _dbContext.ScoutHost.Remove(scoutHost);
                _dbContext.SaveChanges();
            }

            FormOrder formOrder = new FormOrder()
            {
                CreationDate = sessionAccesser.FormOrder.CreationDate,
                Location = sessionAccesser.FormOrder.CreationPlace,
                OrderNumber = sessionAccesser.FormOrder.Name,
                TrialClosings = sessionAccesser.FormOrder.TrialClosingsSaved,
                Appointments = sessionAccesser.FormOrder.AppointmentsSaved,
                Exclusions = sessionAccesser.FormOrder.ExclusionsSaved,
                Layoffs = sessionAccesser.FormOrder.LayoffsSaved,
                ReprimendsAndPraises = sessionAccesser.FormOrder.ReprimendsAndPraisesSaved,
                TrialOpenings = sessionAccesser.FormOrder.TrialOpenings,
                Other = sessionAccesser.FormOrder.Other
            };

            string formOrderJSON = JsonConvert.SerializeObject(formOrder);

            Order order = new Order() { Contents = formOrderJSON };
            OrderInfo orderInfo = new OrderInfo()
            {
                Order = order,
                Scout = _dbContext.Scouts.Find(sessionAccesser.UserPesel),
                ScoutPeselScout = sessionAccesser.UserPesel,
                Name = formOrder.OrderNumber,
                Team = _dbContext.Teams.Find(sessionAccesser.CurrentTeamId),
                Location = sessionAccesser.FormOrder.CreationPlace,
                CreationDate = DateTime.Now
            };

            _dbContext.Orders.Add(order);
            _dbContext.OrderInfos.Add(orderInfo);
            _dbContext.SaveChanges();

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
            {
                if (layoff.Role == "captain")
                    layoff.RoleName = "Druzynowy";
                if (layoff.Role == "vice captain")
                    layoff.RoleName = "Przyboczny";
                if (layoff.Role == "host captain")
                    layoff.RoleName = "Zastepowy";
                if (layoff.Role == "ensign")
                    layoff.RoleName = "Chorazy druzyny";
                if (layoff.Role == "quatermaster")
                    layoff.RoleName = "Kwatermistrz";
                if (layoff.Role == "chronicler")
                    layoff.RoleName = "Kronikarz";
            }

            formOrder.Layoffs = layoffs;

            foreach (Appointment appointment in appointments)
            {
                if (appointment.Role == "captain")
                    appointment.RoleName = "Drużynowy";
                if (appointment.Role == "vice captain")
                    appointment.RoleName = "Przyboczny";
                if (appointment.Role == "host captain")
                    appointment.RoleName = "Zastępowy";
                if (appointment.Role == "ensign")
                    appointment.RoleName = "Chorazy druzyny";
                if (appointment.Role == "quatermaster")
                    appointment.RoleName = "Kwatermistrz";
                if (appointment.Role == "chronicler")
                    appointment.RoleName = "Kronikarz";
            }

            formOrder.Appointments = appointments;

            foreach(TrialClosing trialClosing in trialClosings)
            {
                if(trialClosing.TrialType == "scout cross")
                {
                    trialClosing.TrialType = "krzyz harcerski";
                    trialClosing.TrialName = "";
                }
                else
                {
                    if (trialClosing.TrialType == "rank")
                        trialClosing.TrialType = "stopien";

                    if (trialClosing.TrialType == "ability")
                        trialClosing.TrialType = "sprawnosc";

                    if (trialClosing.Rank == "1")
                        trialClosing.TrialName = "mlodzik";

                    if (trialClosing.Rank == "2")
                        trialClosing.TrialName = "wywiadowca";

                    if (trialClosing.Rank == "3")
                        trialClosing.TrialName = "odkrywca";

                    if (trialClosing.Rank == "4")
                        trialClosing.TrialName = "cwik";

                    if (trialClosing.Rank == "5")
                        trialClosing.TrialName = "harcerz orli";

                    if (trialClosing.Rank == "6")
                        trialClosing.TrialName = "harcerz Rzeczypospolitej";

                    if (trialClosing.Ability == "hygenist")
                        trialClosing.TrialName = "higienista";
                    if (trialClosing.Ability == "paramedic")
                        trialClosing.TrialName = "sanitariusz";
                    if (trialClosing.Ability == "lifesaver")
                        trialClosing.TrialName = "ratownik";
                    if (trialClosing.Ability == "glimmer")
                        trialClosing.TrialName = "ognik";
                    if (trialClosing.Ability == "fire guard")
                        trialClosing.TrialName = "straznik ognia";
                    if (trialClosing.Ability == "fireplace master")
                        trialClosing.TrialName = "mistrz ognisk";
                    if (trialClosing.Ability == "drill expert")
                        trialClosing.TrialName = "znawca musztry";
                    if (trialClosing.Ability == "drill master")
                        trialClosing.TrialName = "mistrz musztry";
                    if (trialClosing.Ability == "needle")
                        trialClosing.TrialName = "mlody plywak";
                    if (trialClosing.Ability == "tailor")
                        trialClosing.TrialName = "krawiec";
                    if (trialClosing.Ability == "young swimmer")
                        trialClosing.TrialName = "mlody plywak";
                    if (trialClosing.Ability == "swimmer")
                        trialClosing.TrialName = "plywak";
                    if (trialClosing.Ability == "excellent swimmer")
                        trialClosing.TrialName = "plywak doskonaly";
                    if (trialClosing.Ability == "internaut")
                        trialClosing.TrialName = "internauta";
                    if (trialClosing.Ability == "family historian")
                        trialClosing.TrialName = "historyk rodzinny";
                    if (trialClosing.Ability == "european")
                        trialClosing.TrialName = "europejczyk";
                    if (trialClosing.Ability == "health leader")
                        trialClosing.TrialName = "lider zdrowia";
                    if (trialClosing.Ability == "nature friend")
                        trialClosing.TrialName = "przyjaciel przyrody";
                    if (trialClosing.Ability == "photograph")
                        trialClosing.TrialName = "fotograf";
                }
            }

            formOrder.TrialClosings = trialClosings;

            foreach (TrialOpening trialOpening in trialOpenings)
            {
                if (trialOpening.TrialType == "scout cross")
                {
                    trialOpening.TrialType = "krzyz harcerski";
                    trialOpening.TrialName = "";
                }
                else
                {
                    if (trialOpening.TrialType == "rank")
                        trialOpening.TrialType = "stopien";

                    if (trialOpening.TrialType == "ability")
                        trialOpening.TrialType = "sprawnosc";

                    if (trialOpening.Rank == "1")
                        trialOpening.TrialName = "mlodzik";

                    if (trialOpening.Rank == "2")
                        trialOpening.TrialName = "wywiadowca";

                    if (trialOpening.Rank == "3")
                        trialOpening.TrialName = "odkrywca";

                    if (trialOpening.Rank == "4")
                        trialOpening.TrialName = "cwik";

                    if (trialOpening.Rank == "5")
                        trialOpening.TrialName = "harcerz orli";

                    if (trialOpening.Rank == "6")
                        trialOpening.TrialName = "harcerz Rzeczypospolitej";

                    if (trialOpening.Ability == "hygenist")
                        trialOpening.TrialName = "higienista";
                    if (trialOpening.Ability == "paramedic")
                        trialOpening.TrialName = "sanitariusz";
                    if (trialOpening.Ability == "lifesaver")
                        trialOpening.TrialName = "ratownik";
                    if (trialOpening.Ability == "glimmer")
                        trialOpening.TrialName = "ognik";
                    if (trialOpening.Ability == "fire guard")
                        trialOpening.TrialName = "strażnik ognia";
                    if (trialOpening.Ability == "fireplace master")
                        trialOpening.TrialName = "mistrz ognisk";
                    if (trialOpening.Ability == "drill expert")
                        trialOpening.TrialName = "znawca musztry";
                    if (trialOpening.Ability == "drill master")
                        trialOpening.TrialName = "mistrz musztry";
                    if (trialOpening.Ability == "needle")
                        trialOpening.TrialName = "mlody pływak";
                    if (trialOpening.Ability == "tailor")
                        trialOpening.TrialName = "krawiec";
                    if (trialOpening.Ability == "young swimmer")
                        trialOpening.TrialName = "mlody plywak";
                    if (trialOpening.Ability == "swimmer")
                        trialOpening.TrialName = "plywak";
                    if (trialOpening.Ability == "excellent swimmer")
                        trialOpening.TrialName = "plywak doskonaly";
                    if (trialOpening.Ability == "internaut")
                        trialOpening.TrialName = "internauta";
                    if (trialOpening.Ability == "family historian")
                        trialOpening.TrialName = "historyk rodzinny";
                    if (trialOpening.Ability == "european")
                        trialOpening.TrialName = "europejczyk";
                    if (trialOpening.Ability == "health leader")
                        trialOpening.TrialName = "lider zdrowia";
                    if (trialOpening.Ability == "nature friend")
                        trialOpening.TrialName = "przyjaciel przyrody";
                    if (trialOpening.Ability == "photograph")
                        trialOpening.TrialName = "fotograf";
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

            formOrder.Location = "Warszawa";

            new GeneratorPdf().GenerateOrder(formOrder);

            return Redirect("orders");
        }

        public IActionResult Appointments()
        {
            List<Scout> scoutsInTheTeam = modelManager.GetScoutsFromATeam(sessionAccesser.CurrentTeamId);
            List<string> peselsOfScoutsThatAreAlreadyInTheAppointment = sessionAccesser.FormOrder
                .Appointments
                .Select(appointment => appointment.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded =
                scoutsInTheTeam.Where(scout => !peselsOfScoutsThatAreAlreadyInTheAppointment.Contains(scout.PeselScout) && scout.PeselScout != sessionAccesser.UserPesel).ToList();

            List<Host> hostsFromTheTeam = modelManager.GetListOfHostsFromATeam(sessionAccesser.CurrentTeamId);

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<SelectListItem> dropDownList_Roles = new List<SelectListItem>()
            {
                new() {Text = "przyboczny", Value = "vice captain"},
                new() {Text = "chorąży drużyny", Value = "ensign"},
                new() {Text = "kwatermistrz", Value = "quatermaster"},
                new() {Text = "kronikarz", Value = "chronicler"},
                new() {Text = "zastępowy", Value = "host captain"}
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

            int numberOfScoutsInTheTeam = modelManager.GetScoutsFromATeam(sessionAccesser.CurrentTeamId).Count();

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
            string scoutPesel = modelManager.GetScoutPesel(appointmentsViewModel.AddedScoutId);

            bool scoutIsInTheTeam = modelManager.ScoutIsInTheTeam(scoutPesel, sessionAccesser.CurrentTeamId);
            bool scoutIsNotInTheAppointentList = !formOrder.Appointments
                .Select(appointment => appointment.ScoutId)
                .ToList()
                .Contains(appointmentsViewModel.AddedScoutId);

            if (scoutIsInTheTeam && scoutIsNotInTheAppointentList)
            {
                Scout scout = _dbContext.Scouts.Where(_scout => _scout.IdentityId == appointmentsViewModel.AddedScoutId).First();

                appointmentsViewModel.Appointments.Add(
                    new Appointment()
                    {
                        ScoutId = scout.IdentityId,
                        ScoutPesel = scout.PeselScout,
                        ScoutName = scout.Name,
                        ScoutSurname = scout.Surname
                    });

                formOrder.Appointments = appointmentsViewModel.Appointments;
            }

            sessionAccesser.FormOrder = formOrder;

            return Redirect("appointments");
        }
        [HttpPost]
        public IActionResult AppointmentsRemove(AppointmentsViewModel appointmentsViewModel, string scoutId)
        {
            List<string> idsOfScoutsFromTheAppointments = appointmentsViewModel.Appointments
                .Select(appointment => appointment.ScoutId)
                .ToList();

            if (idsOfScoutsFromTheAppointments.Contains(scoutId))
            {
                SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
                appointmentsViewModel.Appointments.RemoveAll(appointment => appointment.ScoutId == scoutId);
                formOrder.Appointments = appointmentsViewModel.Appointments;

                sessionAccesser.FormOrder = formOrder;
            }

            return Redirect("appointments");
        }

        public IActionResult Exclusions()
        {
            List<Scout> scoutsInTheTeam = modelManager.GetScoutsFromATeam(sessionAccesser.CurrentTeamId);
            List<string> peselsOfScoutsThatAreAlreadyInTheExclusions = sessionAccesser.FormOrder
                .Exclusions
                .Select(exclusion => exclusion.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded =
                scoutsInTheTeam.Where(scout => !peselsOfScoutsThatAreAlreadyInTheExclusions.Contains(scout.PeselScout) && scout.PeselScout != sessionAccesser.UserPesel).ToList();

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


            int numberOfScoutsInTheTeam = modelManager.GetScoutsFromATeam(sessionAccesser.CurrentTeamId).Count();

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
            string scoutPesel = modelManager.GetScoutPesel(exclusionsViewModel.AddedScoutId);

            bool scoutIsInTheTeam = modelManager.ScoutIsInTheTeam(scoutPesel, sessionAccesser.CurrentTeamId);
            bool scoutIsNotInTheAppointentList = !formOrder.Exclusions
                .Select(exclusion => exclusion.ScoutId)
                .ToList()
                .Contains(exclusionsViewModel.AddedScoutId);

            if (scoutIsInTheTeam && scoutIsNotInTheAppointentList)
            {
                Scout scout = _dbContext.Scouts.Where(_scout => _scout.IdentityId == exclusionsViewModel.AddedScoutId).First();

                exclusionsViewModel.Exclusions.Add(
                    new Exclusion()
                    {
                        ScoutId = scout.IdentityId,
                        ScoutPesel = scout.PeselScout,
                        ScoutName = scout.Name,
                        ScoutSurname = scout.Surname
                    });

                formOrder.Exclusions = exclusionsViewModel.Exclusions;
            }

            sessionAccesser.FormOrder = formOrder;

            return Redirect("exclusions");
        }
        [HttpPost]
        public IActionResult ExclusionsRemove(ExclusionsViewModel exclusionsViewModel, string scoutId)
        {
            List<string> idsOfScoutsFromTheAppointments = exclusionsViewModel.Exclusions
                .Select(exclusion => exclusion.ScoutId)
                .ToList();

            if (idsOfScoutsFromTheAppointments.Contains(scoutId))
            {
                SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
                exclusionsViewModel.Exclusions.RemoveAll(appointment => appointment.ScoutId == scoutId);
                formOrder.Exclusions = exclusionsViewModel.Exclusions;

                sessionAccesser.FormOrder = formOrder;
            }

            return Redirect("exclusions");
        }

        public IActionResult Layoffs()
        {
            List<Scout> scoutsInTheTeam = modelManager.GetScoutsFromATeam(sessionAccesser.CurrentTeamId);
            List<string> peselsOfScoutsThatAreAlreadyInTheLayoffs = sessionAccesser.FormOrder
                .Layoffs
                .Select(layoff => layoff.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded =
                scoutsInTheTeam.Where(scout => !peselsOfScoutsThatAreAlreadyInTheLayoffs.Contains(scout.PeselScout) && scout.PeselScout != sessionAccesser.UserPesel).ToList();

            List<Host> hostsFromTheTeam = modelManager.GetListOfHostsFromATeam(sessionAccesser.CurrentTeamId);

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<SelectListItem> dropDownList_Roles = new List<SelectListItem>()
            {
                new() {Text = "przyboczny", Value = "vice captain"},
                new() {Text = "chorąży drużyny", Value = "ensign"},
                new() {Text = "kwatermistrz", Value = "quatermaster"},
                new() {Text = "kronikarz", Value = "chronicler"},
                new() {Text = "zastępowy", Value = "host captain"}
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

            int numberOfScoutsInTheTeam = modelManager.GetScoutsFromATeam(sessionAccesser.CurrentTeamId).Count();

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
            string scoutPesel = modelManager.GetScoutPesel(layoffsViewModel.AddedScoutId);

            bool scoutIsInTheTeam = modelManager.ScoutIsInTheTeam(scoutPesel, sessionAccesser.CurrentTeamId);
            bool scoutIsNotInTheAppointentList = !formOrder.Layoffs
                .Select(layoff => layoff.ScoutId)
                .ToList()
                .Contains(layoffsViewModel.AddedScoutId);

            if (scoutIsInTheTeam && scoutIsNotInTheAppointentList)
            {
                Scout scout = _dbContext.Scouts.Where(_scout => _scout.IdentityId == layoffsViewModel.AddedScoutId).First();

                layoffsViewModel.Layoffs.Add(
                    new Layoff()
                    {
                        ScoutId = scout.IdentityId,
                        ScoutPesel = scout.PeselScout,
                        ScoutName = scout.Name,
                        ScoutSurname = scout.Surname
                    });

                formOrder.Layoffs = layoffsViewModel.Layoffs;
            }

            sessionAccesser.FormOrder = formOrder;

            return Redirect("layoffs");
        }
        [HttpPost]
        public IActionResult LayoffsRemove(LayoffsViewModel layoffsViewModel, string scoutId)
        {
            List<string> idsOfScoutsFromTheAppointments = layoffsViewModel.Layoffs
                .Select(layoff => layoff.ScoutId)
                .ToList();

            if (idsOfScoutsFromTheAppointments.Contains(scoutId))
            {
                SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
                layoffsViewModel.Layoffs.RemoveAll(layoff => layoff.ScoutId == scoutId);
                formOrder.Layoffs = layoffsViewModel.Layoffs;

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

        public IActionResult ReprimendsAndPraises()
        {
            List<Scout> scoutsInTheTeam = modelManager.GetScoutsFromATeam(sessionAccesser.CurrentTeamId);
            List<string> peselsOfScoutsThatAreAlreadyInTheReprimendsAndPraises = sessionAccesser.FormOrder
                .ReprimendsAndPraises
                .Select(reprimendOrPrise => reprimendOrPrise.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded =
                scoutsInTheTeam.Where(scout => !peselsOfScoutsThatAreAlreadyInTheReprimendsAndPraises.Contains(scout.PeselScout)).ToList();

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

            int numberOfScoutsInTheTeam = modelManager.GetScoutsFromATeam(sessionAccesser.CurrentTeamId).Count();

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
            string scoutPesel = modelManager.GetScoutPesel(reprimendsAndPraisesViewModel.AddedScoutId);

            bool scoutIsInTheTeam = modelManager.ScoutIsInTheTeam(scoutPesel, sessionAccesser.CurrentTeamId);
            bool scoutIsNotInTheAppointentList = !formOrder.ReprimendsAndPraises
                .Select(reprimendOrPraise => reprimendOrPraise.ScoutId)
                .ToList()
                .Contains(reprimendsAndPraisesViewModel.AddedScoutId);

            if (scoutIsInTheTeam && scoutIsNotInTheAppointentList)
            {
                Scout scout = _dbContext.Scouts.Where(_scout => _scout.IdentityId == reprimendsAndPraisesViewModel.AddedScoutId).First();

                reprimendsAndPraisesViewModel.ReprimendsAndPraises.Add(
                    new ReprimendsAndPraises()
                    {
                        ScoutId = scout.IdentityId,
                        ScoutPesel = scout.PeselScout,
                        ScoutName = scout.Name,
                        ScoutSurname = scout.Surname
                    });

                formOrder.ReprimendsAndPraises = reprimendsAndPraisesViewModel.ReprimendsAndPraises;
            }

            sessionAccesser.FormOrder = formOrder;

            return Redirect("reprimendsandpraises");
        }
        [HttpPost]
        public IActionResult ReprimendsAndPraisesRemove(ReprimendsAndPraisesViewModel reprimendsAndPraisesViewModel, string scoutId)
        {
            List<string> idsOfScoutsFromTheAppointments = reprimendsAndPraisesViewModel.ReprimendsAndPraises
                .Select(reprimendOrPraise => reprimendOrPraise.ScoutId)
                .ToList();

            if (idsOfScoutsFromTheAppointments.Contains(scoutId))
            {
                SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
                reprimendsAndPraisesViewModel.ReprimendsAndPraises.RemoveAll(reprimendOrPraise => reprimendOrPraise.ScoutId == scoutId);
                formOrder.ReprimendsAndPraises = reprimendsAndPraisesViewModel.ReprimendsAndPraises;

                sessionAccesser.FormOrder = formOrder;
            }

            return Redirect("reprimendsandpraises");
        }

        public IActionResult TrialClosings()
        {
            List<Scout> scoutsInTheTeam = modelManager.GetScoutsFromATeam(sessionAccesser.CurrentTeamId);
            List<string> peselsOfScoutsThatAreAlreadyInTheTrialClosings = sessionAccesser.FormOrder
                .TrialClosings
                .Select(trialClosing => trialClosing.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded =
                scoutsInTheTeam.Where(scout => !peselsOfScoutsThatAreAlreadyInTheTrialClosings.Contains(scout.PeselScout)).ToList();

            List<Host> hostsFromTheTeam = modelManager.GetListOfHostsFromATeam(sessionAccesser.CurrentTeamId);

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<SelectListItem> dropDownList_TrialTypes = new List<SelectListItem>()
            {
                new() {Text = "krzyż harcerski", Value = "scout cross"},
                new() {Text = "stopień", Value = "rank"},
                new() {Text = "sprawność", Value = "ability"}
            };
            List<SelectListItem> dropDownList_Ranks = new List<SelectListItem>()
            {
                new() {Text = "młodzik/ochotniczka", Value = "1"},
                new() {Text = "wywiadowca/tropicielka", Value = "2"},
                new() {Text = "odkrywca/pionierka", Value = "3"},
                new() {Text = "ćwik/samarytanka", Value = "4"},
                new() {Text = "harcerz orli/harcerka orla", Value = "5"},
                new() {Text = "harcerz rzeczypospolitej/harcerka rzeczypospolitej", Value = "6"}
            };
            List<SelectListItem> dropDownList_Abilities = new List<SelectListItem>()
            {
                new() {Text = "higienista", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "hygenist").First().IdAchievement.ToString()},
                new() {Text = "sanitariusz", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "paramedic").First().IdAchievement.ToString()},
                new() {Text = "ratownik", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "lifesaver").First().IdAchievement.ToString()},
                new() {Text = "ognik", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "glimmer").First().IdAchievement.ToString()},
                new() {Text = "strażnik ognia", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "fire guard").First().IdAchievement.ToString()},
                new() {Text = "mistrz ognisk", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "fireplace master").First().IdAchievement.ToString()},
                new() {Text = "znawca musztry", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "drill expert").First().IdAchievement.ToString()},
                new() {Text = "mistrz musztry", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "drill master").First().IdAchievement.ToString()},
                new() {Text = "igiełka", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "needle").First().IdAchievement.ToString()},
                new() {Text = "krawiec", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "tailor").First().IdAchievement.ToString()},
                new() {Text = "młody pływak", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "young swimmer").First().IdAchievement.ToString()},
                new() {Text = "pływak", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "swimmer").First().IdAchievement.ToString()},
                new() {Text = "pływak doskonały", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "excellent swimmer").First().IdAchievement.ToString()},
                new() {Text = "internauta", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "internaut").First().IdAchievement.ToString()},
                new() {Text = "historyk rodzinny", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "family historian").First().IdAchievement.ToString()},
                new() {Text = "europejczyk", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "european").First().IdAchievement.ToString()},
                new() {Text = "lider zdrowia", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "health leader").First().IdAchievement.ToString()},
                new() {Text = "przyjaciel przyrody", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "nature friend").First().IdAchievement.ToString()},
                new() {Text = "fotograf", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "photograph").First().IdAchievement.ToString()},
            };

            foreach (Scout scout in scoutsThatCanBeAdded)
            {
                dropDownList_Scouts.Add(new SelectListItem()
                {
                    Text = string.Format("{0} {1} ({2})", scout.Surname, scout.Name, scout.PeselScout),
                    Value = scout.IdentityId
                });
            }

            int numberOfScoutsInTheTeam = modelManager.GetScoutsFromATeam(sessionAccesser.CurrentTeamId).Count();

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
            string scoutPesel = modelManager.GetScoutPesel(trialClosingsViewModel.AddedScoutId);

            bool scoutIsInTheTeam = modelManager.ScoutIsInTheTeam(scoutPesel, sessionAccesser.CurrentTeamId);
            bool scoutIsNotInTheAppointentList = !formOrder.TrialClosings
                .Select(trialClosing => trialClosing.ScoutId)
                .ToList()
                .Contains(trialClosingsViewModel.AddedScoutId);

            if (scoutIsInTheTeam && scoutIsNotInTheAppointentList)
            {
                Scout scout = _dbContext.Scouts.Where(_scout => _scout.IdentityId == trialClosingsViewModel.AddedScoutId).First();

                trialClosingsViewModel.TrialClosings.Add(
                    new TrialClosing()
                    {
                        ScoutId = scout.IdentityId,
                        ScoutPesel = scout.PeselScout,
                        ScoutName = scout.Name,
                        ScoutSurname = scout.Surname
                    });

                formOrder.TrialClosings = trialClosingsViewModel.TrialClosings;
            }

            sessionAccesser.FormOrder = formOrder;

            return Redirect("trialclosings");
        }
        [HttpPost]
        public IActionResult TrialClosingsRemove(TrialClosingsViewModel trialClosingsViewModel, string scoutId)
        {
            List<string> idsOfScoutsFromTheAppointments = trialClosingsViewModel.TrialClosings
                .Select(trialClosing => trialClosing.ScoutId)
                .ToList();

            if (idsOfScoutsFromTheAppointments.Contains(scoutId))
            {
                SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
                trialClosingsViewModel.TrialClosings.RemoveAll(trialClosing => trialClosing.ScoutId == scoutId);
                formOrder.TrialClosings = trialClosingsViewModel.TrialClosings;

                sessionAccesser.FormOrder = formOrder;
            }

            return Redirect("trialclosings");
        }

        public IActionResult TrialOpenings()
        {
            List<Scout> scoutsInTheTeam = modelManager.GetScoutsFromATeam(sessionAccesser.CurrentTeamId);
            List<string> peselsOfScoutsThatAreAlreadyInTheTrialOpenings = sessionAccesser.FormOrder
                .TrialOpenings
                .Select(trialOpening => trialOpening.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded =
                scoutsInTheTeam.Where(scout => !peselsOfScoutsThatAreAlreadyInTheTrialOpenings.Contains(scout.PeselScout)).ToList();

            List<Host> hostsFromTheTeam = modelManager.GetListOfHostsFromATeam(sessionAccesser.CurrentTeamId);

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<SelectListItem> dropDownList_TrialTypes = new List<SelectListItem>()
            {
                new() {Text = "krzyż harcerski", Value = "scout cross"},
                new() {Text = "stopień", Value = "rank"},
                new() {Text = "sprawność", Value = "ability"}
            };
            List<SelectListItem> dropDownList_Ranks = new List<SelectListItem>()
            {
                new() {Text = "młodzik/ochotniczka", Value = "1"},
                new() {Text = "wywiadowca/tropicielka", Value = "2"},
                new() {Text = "odkrywca/pionierka", Value = "3"},
                new() {Text = "ćwik/samarytanka", Value = "4"},
                new() {Text = "harcerz orli/harcerka orla", Value = "5"},
                new() {Text = "harcerz rzeczypospolitej/harcerka rzeczypospolitej", Value = "6"}
            };
            List<SelectListItem> dropDownList_Abilities = new List<SelectListItem>()
            {
                new() {Text = "higienista", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "hygenist").First().IdAchievement.ToString()},
                new() {Text = "sanitariusz", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "paramedic").First().IdAchievement.ToString()},
                new() {Text = "ratownik", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "lifesaver").First().IdAchievement.ToString()},
                new() {Text = "ognik", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "glimmer").First().IdAchievement.ToString()},
                new() {Text = "strażnik ognia", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "fire guard").First().IdAchievement.ToString()},
                new() {Text = "mistrz ognisk", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "fireplace master").First().IdAchievement.ToString()},
                new() {Text = "znawca musztry", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "drill expert").First().IdAchievement.ToString()},
                new() {Text = "mistrz musztry", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "drill master").First().IdAchievement.ToString()},
                new() {Text = "igiełka", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "needle").First().IdAchievement.ToString()},
                new() {Text = "krawiec", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "tailor").First().IdAchievement.ToString()},
                new() {Text = "młody pływak", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "young swimmer").First().IdAchievement.ToString()},
                new() {Text = "pływak", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "swimmer").First().IdAchievement.ToString()},
                new() {Text = "pływak doskonały", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "excellent swimmer").First().IdAchievement.ToString()},
                new() {Text = "internauta", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "internaut").First().IdAchievement.ToString()},
                new() {Text = "historyk rodzinny", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "family historian").First().IdAchievement.ToString()},
                new() {Text = "europejczyk", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "european").First().IdAchievement.ToString()},
                new() {Text = "lider zdrowia", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "health leader").First().IdAchievement.ToString()},
                new() {Text = "przyjaciel przyrody", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "nature friend").First().IdAchievement.ToString()},
                new() {Text = "fotograf", Value = _dbContext.Achievements.Where(achievement => achievement.Type == "photograph").First().IdAchievement.ToString()},
            };

            foreach (Scout scout in scoutsThatCanBeAdded)
            {
                dropDownList_Scouts.Add(new SelectListItem()
                {
                    Text = string.Format("{0} {1} ({2})", scout.Surname, scout.Name, scout.PeselScout),
                    Value = scout.IdentityId
                });
            }

            int numberOfScoutsInTheTeam = modelManager.GetScoutsFromATeam(sessionAccesser.CurrentTeamId).Count();

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
            string scoutPesel = modelManager.GetScoutPesel(trialOpeningsViewModel.AddedScoutId);

            bool scoutIsInTheTeam = modelManager.ScoutIsInTheTeam(scoutPesel, sessionAccesser.CurrentTeamId);
            bool scoutIsNotInTheAppointentList = !formOrder.TrialOpenings
                .Select(trialOpening => trialOpening.ScoutId)
                .ToList()
                .Contains(trialOpeningsViewModel.AddedScoutId);

            if (scoutIsInTheTeam && scoutIsNotInTheAppointentList)
            {
                Scout scout = _dbContext.Scouts.Where(_scout => _scout.IdentityId == trialOpeningsViewModel.AddedScoutId).First();

                trialOpeningsViewModel.TrialOpenings.Add(
                    new TrialOpening()
                    {
                        ScoutId = scout.IdentityId,
                        ScoutPesel = scout.PeselScout,
                        ScoutName = scout.Name,
                        ScoutSurname = scout.Surname
                    });

                formOrder.TrialOpenings = trialOpeningsViewModel.TrialOpenings;
            }

            sessionAccesser.FormOrder = formOrder;

            return Redirect("trialopenings");
        }
        [HttpPost]
        public IActionResult TrialOpeningsRemove(TrialOpeningsViewModel trialOpeningsViewModel, string scoutId)
        {
            List<string> idsOfScoutsFromTheAppointments = trialOpeningsViewModel.TrialOpenings
                .Select(trialOpening => trialOpening.ScoutId)
                .ToList();

            if (idsOfScoutsFromTheAppointments.Contains(scoutId))
            {
                SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
                trialOpeningsViewModel.TrialOpenings.RemoveAll(trialOpening => trialOpening.ScoutId == scoutId);
                formOrder.TrialOpenings = trialOpeningsViewModel.TrialOpenings;

                sessionAccesser.FormOrder = formOrder;
            }

            return Redirect("trialopenings");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> SetRole(string userId, string role)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userId);
            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction("personaldata", "profilecontroller");
        }
    }
}
