using TaskCollaborationAppAPI.Data;

namespace TaskCollaborationAppAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public ITaskRepository Tasks { get; private set; }
        public IUserRepository Users { get; private set; }

        public UnitOfWork(AppDbContext context, ITaskRepository taskRepository, IUserRepository userRepository)
        {
            _context = context;
            Tasks = taskRepository;
            Users = userRepository;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }
    }
}
