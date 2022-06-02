using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Models;
using System.Diagnostics;

namespace moja_druzyna.Controllers
{
    public class OtherController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<OtherController> _logger;

        public OtherController(ApplicationDbContext dbContext, ILogger<OtherController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IActionResult Settings()
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
