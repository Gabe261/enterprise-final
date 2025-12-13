using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskCollaborationApp.Web.Models;

namespace TaskCollaborationApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.Token = HttpContext.Session.GetString("JwtToken") ?? "No token";
            ViewBag.Email = HttpContext.Session.GetString("UserEmail") ?? "No email";
            ViewBag.Name = HttpContext.Session.GetString("UserName") ?? "No name";

            ViewBag.ApiBaseUrl = _configuration["ApiBaseUrl"];

            return View();
        }
    }
}
