
using TaskCollaborationAppAPI.Data;
using TaskCollaborationAppAPI.Models;

namespace TaskCollaborationAppAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<TaskItem> GetAllTasks(int pageNumber, int pageSize)
        {
            // TODO: implement page number and page size
            return _context.Tasks.ToList();
        }

        public TaskItem GetTaskById(int id)
        {
            return _context.Tasks.Find(id);
        }

        public void AddTask(TaskItem task)
        {
            _context.Tasks.Add(task);
        }

        public TaskItem UpdateTaskById(int id, TaskItem task)
        {
            if(id == task.Id)
            {
                _context.Tasks.Update(task);
                return task;
            }
            return null;
        }

        public void DeleteTaskById(int id)
        {
            var task = _context.Tasks.Find(id);
            if(task != null)
            {
                _context.Tasks.Remove(task);
            }
        }

        public IEnumerable<TaskItem> GetTasksByUserId(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return Enumerable.Empty<TaskItem>();
            }

            return _context.Tasks.Where(t => t.CreatedById == userId).ToList();
        }

        public IEnumerable<TaskItem> GetTasksAssignedToUserId(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return Enumerable.Empty<TaskItem>();
            }

            return _context.Tasks.Where(t => t.AssignedToId == userId).ToList();
        }
    }
}
