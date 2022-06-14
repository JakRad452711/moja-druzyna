using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Models;
using moja_druzyna.ViewModels;
using System;
using System.Diagnostics;

namespace moja_druzyna.Controllers
{
    public class DocumentsGeneratorsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DocumentsGeneratorsController> _logger;

        private static OrderFormViewModel orderFormViewModel = new OrderFormViewModel();

        public DocumentsGeneratorsController(ApplicationDbContext dbContext, ILogger<DocumentsGeneratorsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
