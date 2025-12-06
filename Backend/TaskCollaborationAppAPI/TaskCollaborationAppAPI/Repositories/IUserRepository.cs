using TaskCollaborationAppAPI.Models;

namespace TaskCollaborationAppAPI.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllUsers();

        User GetUserById(int id);
    }
}
