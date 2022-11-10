using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using moja_druzyna.Data;
using moja_druzyna.Data.Session;

namespace moja_druzyna.Controllers
{
    public class ApiController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TeamController> _logger;

        private readonly SessionAccesser sessionAccesser;

        public ApiController(ApplicationDbContext dbContext, ILogger<TeamController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;

            sessionAccesser = new SessionAccesser(dbContext, httpContextAccessor);
        }

        [HttpGet]
        public JsonResult GetIsSidebarCollapsed()
        {
            bool isSidebarCollapsed = sessionAccesser.IsSidebarCollapsed;

            TempData["isSidebarCollapsed"] = isSidebarCollapsed;

            return new JsonResult(Ok(isSidebarCollapsed));
        }

        [HttpPost]
        public JsonResult ToggleIsSidebarCollapsed()
        {
            bool isSidebarCollapsed = sessionAccesser.IsSidebarCollapsed;

            sessionAccesser.IsSidebarCollapsed = !isSidebarCollapsed;
            TempData["isSidebarCollapsed"] = !isSidebarCollapsed;

            return new JsonResult(Ok());
        }
    }
}
