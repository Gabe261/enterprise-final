using Microsoft.AspNetCore.Mvc;

namespace TaskCollaborationApp.Web.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // localStorage.clear()
            return RedirectToAction("Login");
        }
    }
}
