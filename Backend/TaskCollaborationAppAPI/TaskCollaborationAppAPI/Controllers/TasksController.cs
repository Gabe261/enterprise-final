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
        public ActionResult<IEnumerable<TaskItem>> GetAllTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) // page number and size are placeholders for potential UI
        {
            var tasks = _unitOfWork.Tasks.GetAllTasks(pageNumber, pageSize);
            return Ok(tasks);
        }

        /* GET api/tasks/{id} == Get single task */
        [HttpGet("{id}")]
        public ActionResult<TaskItem> GetTaskById(int id)
        {
            var taskItem = _unitOfWork.Tasks.GetTaskById(id);
            if (taskItem == null)
            {
                return NotFound();
            }
            return Ok(taskItem);
        }

        /* POST api/tasks == Create new task */
        [HttpPost]
        public ActionResult AddTaskItem(TaskItem taskItem)
        {
            if (taskItem == null)
            {
                return BadRequest();
            }
            _unitOfWork.Tasks.AddTask(taskItem);
            _unitOfWork.Complete();
            return Ok();
        }

        /* PUT api/tasks/{id} == Update task */
        [HttpPut("{id}")]
        public ActionResult UpdateTaskItem(int id, TaskItem taskItem)
        {
            var modifiedTaskItem = _unitOfWork.Tasks.UpdateTaskById(id, taskItem);
            _unitOfWork.Complete();

            if (modifiedTaskItem != null)
            {
                return Ok(modifiedTaskItem);
            }
            else
            {
                return NotFound();
            }
        }

        /* DELETE api/tasks/{id} == Delete task */
        [HttpDelete("{id}")]
        public ActionResult DeleteTaskItem(int id) 
        {
            _unitOfWork.Tasks.DeleteTaskById(id);
            _unitOfWork.Complete();
            return Ok();
        }

        /* GET api/tasks/my == Get current user’s tasks */
        [HttpGet("my")]
        public ActionResult GetActiveUsersTasks()
        {
            // Use Session to Get Current User Id ??
            int userId = 1; // Placeholder for current user id
            var tasks = _unitOfWork.Tasks.GetTasksByUserId(userId);
            return Ok(tasks);
        }

        /* GET api/tasks/assigned == Get tasks assigned to current user */
        [HttpGet("assigned")]
        public ActionResult GetActiveUsersAssignedTasks()
        {
            // Use Session to Get Current User Id ??
            int userId = 1; // Placeholder for current user id
            var tasks = _unitOfWork.Tasks.GetTasksAssignedToUserId(userId);
            return Ok(tasks);
        }
    }
}
