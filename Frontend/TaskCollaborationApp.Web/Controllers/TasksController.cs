using Microsoft.AspNetCore.Mvc;

namespace TaskCollaborationApp.Web.Controllers
{
    public class TasksController : Controller
    {
        private readonly IConfiguration _configuration;

        public TasksController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            SetViewBagData();
            return View();
        }

        public IActionResult Details(int id)
        {
            SetViewBagData();
            ViewBag.TaskId = id;
            return View();
        }

        public IActionResult Create()
        {
            SetViewBagData();
            return View();
        }

        public IActionResult Edit(int id)
        {
            SetViewBagData();
            ViewBag.TaskId = id;
            return View();
        }

        private void SetViewBagData()
        {
            ViewBag.ApiBaseUrl = _configuration["ApiBaseUrl"];
            ViewBag.Token = HttpContext.Session.GetString("JwtToken") ?? "No token";
            ViewBag.Name = HttpContext.Session.GetString("Name") ?? "";
            ViewBag.Email = HttpContext.Session.GetString("Email") ?? "";
            ViewBag.UserName = HttpContext.Session.GetString("UserName") ?? "";
            ViewBag.Role = HttpContext.Session.GetString("Role") ?? "";
        }

    }
}
