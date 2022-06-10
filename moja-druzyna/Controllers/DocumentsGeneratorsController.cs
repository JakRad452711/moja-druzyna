using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Models;
using System.Diagnostics;

namespace moja_druzyna.Controllers
{
    public class DocumentsGeneratorsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DocumentsGeneratorsController> _logger;

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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
