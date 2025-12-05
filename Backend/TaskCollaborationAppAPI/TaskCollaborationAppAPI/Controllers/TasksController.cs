using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskCollaborationAppAPI.Models;
using TaskCollaborationAppAPI.Repositories;

namespace TaskCollaborationAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TasksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /* GET api/tasks == Get all tasks (with pagination) */
        [HttpGet]
        public ActionResult<IEnumerable<TaskItem>> GetAllTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var tasks = _unitOfWork.Tasks.GetAllTasks(pageNumber, pageSize);
            return Ok(tasks);
        }

        /* GET api/tasks/{id} == Get single task */

        /* POST api/tasks == Create new task */

        /* PUT api/tasks/{id} == Update task */

        /* DELETE api/tasks/{id} == Delete task */

        /* GET api/tasks/my == Get current user’s tasks */

        /* GET api/tasks/assigned == Get tasks assigned to current user */

    }
}
