
using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<TaskDto> GetAllTasks(int pageNumber, int pageSize)
        {
            return _context.Tasks
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Where(t => !t.IsArchived)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => MapToDto(t))
            .ToList();
        }

        public TaskDto? GetTaskById(int id)
        {
            var task = _context.Tasks
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .FirstOrDefault(t => t.Id == id);

            return MapToDto(task);
        }

        public IEnumerable<TaskDto> GetTasksByUserId(int userId)
        {
            return _context.Tasks
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Where(t => t.CreatedById == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => MapToDto(t))
            .ToList();
        }

        public IEnumerable<TaskDto> GetTasksAssignedToUserId(int userId)
        {
            return _context.Tasks
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Where(t => t.AssignedToId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => MapToDto(t))
            .ToList();
        }

        public TaskItem AddTask(TaskItem task)
        {
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;
            task.IsArchived = false;

            _context.Tasks.Add(task);
            return task;
        }

        public TaskItem? UpdateTaskById(int id, UpdateTaskDto updateTaskDto)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null) return null;

            if (!string.IsNullOrEmpty(updateTaskDto.Title))
                task.Title = updateTaskDto.Title;

            if (updateTaskDto.Description != null)
                task.Description = updateTaskDto.Description;

            if (!string.IsNullOrEmpty(updateTaskDto.Status))
            {
                if (Enum.TryParse<TaskStatusTypes>(updateTaskDto.Status, out var status))
                    task.Status = status;
            }

            if (updateTaskDto.AssignedToId.HasValue)
                task.AssignedToId = updateTaskDto.AssignedToId.Value == 0 ? null : updateTaskDto.AssignedToId;

            task.UpdatedAt = DateTime.UtcNow;

            return task;
        }

        public bool DeleteTaskById(int id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;

            task.IsArchived = true;
            task.ArchivedAt = DateTime.UtcNow;

            return true;
        }

        private TaskDto MapToDto(TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                CreatedById = task.CreatedById,
                CreatedByName = task.CreatedBy?.Name,
                CreatedByEmail = task.CreatedBy?.Email,
                AssignedToId = task.AssignedToId,
                AssignedToName = task.AssignedTo?.Name,
                AssignedToEmail = task.AssignedTo?.Email,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                IsArchived = task.IsArchived
            };
        }
    }
}
