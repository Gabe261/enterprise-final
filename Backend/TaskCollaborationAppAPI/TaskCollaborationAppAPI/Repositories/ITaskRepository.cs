using TaskCollaborationAppAPI.Models;

namespace TaskCollaborationAppAPI.Repositories
{
    public interface ITaskRepository
    {
        IEnumerable<TaskItem> GetAllTasks(int pageNumber, int pageSize);

        TaskItem GetTaskById(int id);

        void AddTask(TaskItem task);

        TaskItem UpdateTaskById(int id, TaskItem task);

        void DeleteTaskById(int id);

        IEnumerable<TaskItem> GetTasksByUserId(int userId);

        IEnumerable<TaskItem> GetTasksAssignedToUserId(int userId);
    }
}
