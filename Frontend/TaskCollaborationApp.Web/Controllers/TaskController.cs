using Microsoft.AspNetCore.Mvc;

namespace TaskCollaborationApp.Web.Controllers
{
    public class TaskController : Controller
    {
        private readonly IConfiguration _configuration;

        public TaskController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _configuration["ApiBaseUrl"];

            ViewBag.Token = HttpContext.Session.GetString("JwtToken") ?? "No token";
            ViewBag.Email = HttpContext.Session.GetString("UserEmail") ?? "No email";
            ViewBag.Name = HttpContext.Session.GetString("Name") ?? "No name";
            ViewBag.UserName = HttpContext.Session.GetString("UserName") ?? "No username";
            ViewBag.Role = HttpContext.Session.GetString("Role") ?? "No role";

            return View();
        }
    }
}
