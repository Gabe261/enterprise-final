using TaskCollaborationAppAPI.Data;
using TaskCollaborationAppAPI.Models;

namespace TaskCollaborationAppAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User GetUserById(int id)
        {
            User existingUser = _context.Users.Find(id);
            User userDetails = new User
            {
                Id = existingUser.Id,
                Name = existingUser.Name,
                Email = existingUser.Email,
                Username = existingUser.Username,
                PasswordHash = "###########",       // Blocking password
                Role = existingUser.Role,
                CreatedAt = existingUser.CreatedAt
            };
            return userDetails;
        }
    }
}
