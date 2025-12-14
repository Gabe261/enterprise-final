using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using TaskCollaborationAppAPI.Hubs;
using TaskCollaborationAppAPI.Models;
using TaskCollaborationAppAPI.Repositories;

namespace TaskCollaborationAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<TaskHub> _hub;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public TasksController(IUnitOfWork unitOfWork, IHubContext<TaskHub> hub, IMemoryCache cache, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _hub = hub;
            _cache = cache;
            _configuration = configuration;
        }

        /* GET api/tasks == Get all tasks (with pagination) */
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAllTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            const string cacheKey = "tasksList";
            if(!_cache.TryGetValue(cacheKey, out IEnumerable<TaskDto> cachedTasks))
            {
                Response.Headers.Add("X-Cache", "MISS");

                int cacheExpireTime = _configuration.GetValue<int>("CacheExpirationMinutes");

                await Task.Delay(2000); // Simulate delay for demonstration

                IEnumerable<TaskDto> tasksFromDb = _unitOfWork.Tasks.GetAllTasks(pageNumber, pageSize);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(cacheExpireTime));

                _cache.Set(cacheKey, tasksFromDb, cacheEntryOptions);
                cachedTasks = tasksFromDb;
            } 
            else
            {
                Response.Headers.Add("X-Cache", "HIT");
            }
                return Ok(cachedTasks);
        }

        /* GET api/tasks/{id} == Get single task */
        [HttpGet("{id}")]
        [Authorize]
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
        [Authorize]
        public async Task<ActionResult> AddTaskItem(TaskDto taskDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (taskDto.CreatedById == 0)
                return Unauthorized();

            if (!Enum.TryParse<TaskStatusTypes>(taskDto.Status, out var status))
                return BadRequest("Invalid status value.");

            int userId = int.Parse(userIdClaim.Value);

            var taskItem = new TaskItem
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = status,
                CreatedById = taskDto.CreatedById,
                AssignedToId = taskDto.AssignedToId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsArchived = false
            };

            _unitOfWork.Tasks.AddTask(taskItem);
            _unitOfWork.Complete();

            /* Notification for task created */
            await _hub.Clients.All.SendAsync("TaskCreated", taskDto);

            /* Notification for assignee */
            if (taskDto.AssignedToId != 0)
            {
                await _hub.Clients.All.SendAsync("TaskAssigned", taskDto);
            }

            return Ok(new { id = taskItem.Id });
        }

        /* PUT api/tasks/{id} == Update task */
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<TaskDto>> UpdateTaskItem(int id, UpdateTaskDto updateTaskDto)
        {
            var modifiedTaskItem = _unitOfWork.Tasks.UpdateTaskById(id, updateTaskDto);
            _unitOfWork.Complete();

            if (modifiedTaskItem != null)
            {
                await _hub.Clients.All.SendAsync("TaskUpdated", modifiedTaskItem);

                if (updateTaskDto.AssignedToId != 0)
                {
                    await _hub.Clients.All.SendAsync("TaskAssigned", modifiedTaskItem);
                }

                return Ok(modifiedTaskItem);
            }
            else
            {
                return NotFound();
            }
        }

        /* DELETE api/tasks/{id} == Delete task */
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTaskItem(int id) 
        {
            var task = _unitOfWork.Tasks.GetTaskById(id);
            if (task == null) return NotFound();

            _unitOfWork.Tasks.DeleteTaskById(id);
            _unitOfWork.Complete();

            await _hub.Clients.All.SendAsync("TaskDeleted", task.Id, task.Title);

            return Ok();
        }

        /* GET api/tasks/my == Get current user’s tasks */
        [HttpGet("my")]
        [Authorize]
        public ActionResult GetActiveUsersTasks()
        {
            // Use Session to Get Current User Id ??
            int userId = 1; // Placeholder for current user id
            var tasks = _unitOfWork.Tasks.GetTasksByUserId(userId);
            return Ok(tasks);
        }

        /* GET api/tasks/assigned == Get tasks assigned to current user */
        [HttpGet("assigned")]
        [Authorize]
        public ActionResult GetActiveUsersAssignedTasks()
        {
            // Use Session to Get Current User Id ??
            int userId = 1; // Placeholder for current user id
            var tasks = _unitOfWork.Tasks.GetTasksAssignedToUserId(userId);
            return Ok(tasks);
        }
    }
}
