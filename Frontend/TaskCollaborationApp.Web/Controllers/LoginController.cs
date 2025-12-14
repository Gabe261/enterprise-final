using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using TaskCollaborationApp.Web.Models;

namespace TaskCollaborationApp.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _configuration["ApiBaseUrl"];
            return View();
        }

        public IActionResult Callback(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value ?? "N/A";
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "N/A";
            var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "N/A";
            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "username")?.Value ?? "N/A";
            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "N/A";

            HttpContext.Session.SetString("JwtToken", token);
            HttpContext.Session.SetString("UserId", userId);
            HttpContext.Session.SetString("UserEmail", email);
            HttpContext.Session.SetString("Name", name);
            HttpContext.Session.SetString("UserName", userName);
            HttpContext.Session.SetString("Role", role);

            return RedirectToAction("Index", "Tasks");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // localStorage.clear()
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult UpdateToken([FromBody] TokenUpdateRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest("Token is required");
            }

            HttpContext.Session.SetString("JwtToken", request.Token);
            return Ok();
        }
    }
}
