using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskCollaborationAppAPI.Models;
using TaskCollaborationAppAPI.Repositories;

namespace TaskCollaborationAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /* GET api/users == Get all users (for assignment) */
        [HttpGet]
        [Authorize]
        public ActionResult GetAllUsers()
        {
            var users = _unitOfWork.Users.GetAllUsers();
            return Ok(users);
        }

        /* GET api/users/{id} == Get user details */
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<User> GetUserById(int id)
        {
            var user = _unitOfWork.Users.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}
