using TaskCollaborationAppAPI.Models;

namespace TaskCollaborationAppAPI.Repositories
{
    public interface ITaskRepository
    {
        IEnumerable<TaskDto> GetAllTasks(int pageNumber, int pageSize);
        TaskDto? GetTaskById(int id);
        IEnumerable<TaskDto> GetTasksByUserId(int userId);
        IEnumerable<TaskDto> GetTasksAssignedToUserId(int userId);
        TaskItem AddTask(TaskItem task);
        TaskItem? UpdateTaskById(int id, UpdateTaskDto updateTaskDto);
        bool DeleteTaskById(int id);
    }
}
