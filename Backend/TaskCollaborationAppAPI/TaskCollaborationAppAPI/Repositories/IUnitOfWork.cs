namespace TaskCollaborationAppAPI.Repositories
{
    public interface IUnitOfWork
    {
        ITaskRepository Tasks { get; }
        IUserRepository Users { get; }

        int Complete();
    }
}
