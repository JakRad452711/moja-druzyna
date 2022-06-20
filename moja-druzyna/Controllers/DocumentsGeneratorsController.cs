using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Data.Session;
using moja_druzyna.Lib.Services.Order;
using moja_druzyna.Models;
using moja_druzyna.ViewModels;
using moja_druzyna.ViewModels.DocumentsGenerators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static moja_druzyna.ViewModels.DocumentsGenerators.OrdersViewModel;

namespace moja_druzyna.Controllers
{
    public class DocumentsGeneratorsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DocumentsGeneratorsController> _logger;

        private readonly SessionAccesser sessionAccesser;
        private readonly ModelManager modelManager;

        private static OrderFormViewModel orderFormViewModel = new OrderFormViewModel();

        public DocumentsGeneratorsController(ApplicationDbContext dbContext, ILogger<DocumentsGeneratorsController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            sessionAccesser = new SessionAccesser(dbContext, httpContextAccessor);
            modelManager = new ModelManager(dbContext);
        }

        public IActionResult Orders()
        {
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

        public IActionResult OrderGenerator()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Identity/Account/Login");

            return View();
        }

        [HttpGet]
        public IActionResult OrderForm()
        {

            return View(orderFormViewModel);
        }

        [HttpPost]
        public IActionResult OrderForm_Submit(OrderFormViewModel _orderFormViewModel)
        {
            orderFormViewModel = _orderFormViewModel;

            throw new NotImplementedException();

            return Redirect("OrderForm");

        }

        public IActionResult Appointments()
        {
            List<Scout> scoutsInTheTeam = modelManager.GetScoutsFromATeam(sessionAccesser.CurrentTeamId);
            List<string> peselsOfScoutsThatAreAlreadyInTheAppointment = sessionAccesser.FormOrder
                .Appointments
                .Select(appointment => appointment.ScoutPesel)
                .ToList();

            List<Scout> scoutsThatCanBeAdded = 
                scoutsInTheTeam.Where(scout => !peselsOfScoutsThatAreAlreadyInTheAppointment.Contains(scout.PeselScout)).ToList();

            List<Host> hostsFromTheTeam = modelManager.GetListOfHostsFromATeam(sessionAccesser.CurrentTeamId);

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<SelectListItem> dropDownList_Roles = new List<SelectListItem>()
            {
                new() {Text = "funkcja 1", Value = "1"},
                new() {Text = "funkcja 2", Value = "2"},
                new() {Text = "funkcja 3", Value = "3"}
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

            ViewBag.AreThereScoutsToAdd = !(peselsOfScoutsThatAreAlreadyInTheAppointment.Count() == numberOfScoutsInTheTeam);
            ViewBag.OrderName = sessionAccesser.FormOrder.Name;
            ViewBag.TeamName = sessionAccesser.CurrentTeamName;

            return View(new AppointmentsViewModel() { Appointments = sessionAccesser.FormOrder.Appointments });
        }
        [HttpPost]
        public IActionResult Appointments(AppointmentsViewModel appointmentsViewModel)
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;
            formOrder.Appointments = appointmentsViewModel.Appointments;
            formOrder.AppointmentsSaved = appointmentsViewModel.Appointments;

            sessionAccesser.FormOrder = formOrder;

            return Redirect("appointments");
        }
        public IActionResult AppointmentsRevert()
        {
            SessionFormOrderContext formOrder = sessionAccesser.FormOrder;

            if(formOrder.AppointmentsSaved == null)
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
                scoutsInTheTeam.Where(scout => !peselsOfScoutsThatAreAlreadyInTheExclusions.Contains(scout.PeselScout)).ToList();

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<SelectListItem> dropDownList_Reasons = new List<SelectListItem>()
            {
                new() {Text = "powód 1", Value = "1"},
                new() {Text = "powód 2", Value = "2"},
                new() {Text = "powód 3", Value = "3"}
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

            ViewBag.AreThereScoutsToAdd = !(peselsOfScoutsThatAreAlreadyInTheExclusions.Count() == numberOfScoutsInTheTeam);
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
                scoutsInTheTeam.Where(scout => !peselsOfScoutsThatAreAlreadyInTheLayoffs.Contains(scout.PeselScout)).ToList();

            List<Host> hostsFromTheTeam = modelManager.GetListOfHostsFromATeam(sessionAccesser.CurrentTeamId);

            List<SelectListItem> dropDownList_Scouts = new List<SelectListItem>();
            List<SelectListItem> dropDownList_Roles = new List<SelectListItem>()
            {
                new() {Text = "funkcja 1", Value = "1"},
                new() {Text = "funkcja 2", Value = "2"},
                new() {Text = "funkcja 3", Value = "3"}
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

            ViewBag.AreThereScoutsToAdd = !(peselsOfScoutsThatAreAlreadyInTheLayoffs.Count() == numberOfScoutsInTheTeam);
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
                new() {Text = "rodzaj 1", Value = "1"},
                new() {Text = "rodzaj 2", Value = "2"},
                new() {Text = "rodzaj 3", Value = "3"}
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
                new() {Text = "typ 1", Value = "1"},
                new() {Text = "typ 2", Value = "2"},
                new() {Text = "typ 3", Value = "3"}
            };
            List<SelectListItem> dropDownList_Ranks = new List<SelectListItem>()
            {
                new() {Text = "stopień 1", Value = "1"},
                new() {Text = "stopień 2", Value = "2"},
                new() {Text = "stopień 3", Value = "3"}
            };
            List<SelectListItem> dropDownList_Abilities = new List<SelectListItem>()
            {
                new() {Text = "zdoność 1", Value = "1"},
                new() {Text = "zdoność 2", Value = "2"},
                new() {Text = "zdoność 3", Value = "3"}
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
                new() {Text = "typ 1", Value = "1"},
                new() {Text = "typ 2", Value = "2"},
                new() {Text = "typ 3", Value = "3"}
            };
            List<SelectListItem> dropDownList_Ranks = new List<SelectListItem>()
            {
                new() {Text = "stopień 1", Value = "1"},
                new() {Text = "stopień 2", Value = "2"},
                new() {Text = "stopień 3", Value = "3"}
            };
            List<SelectListItem> dropDownList_Abilities = new List<SelectListItem>()
            {
                new() {Text = "zdoność 1", Value = "1"},
                new() {Text = "zdoność 2", Value = "2"},
                new() {Text = "zdoność 3", Value = "3"}
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
    }
}
