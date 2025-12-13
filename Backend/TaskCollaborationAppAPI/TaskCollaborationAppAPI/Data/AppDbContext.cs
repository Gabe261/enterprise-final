using Microsoft.EntityFrameworkCore;
using TaskCollaborationAppAPI.Models;

namespace TaskCollaborationAppAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<TaskItem> Tasks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set up User-Task table relations
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.CreatedBy)
                .WithMany()
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Gabriel",
                    Email = "gabrielsiewert1@gmail.com",
                    Username = "GabrielS",
                    PasswordHash = "hashed_password_1",
                    Role = "Admin",
                    CreatedAt = new DateTime(2025, 11, 13)
                },
                new User
                {
                    Id = 2,
                    Name = "Dany",
                    Email = "iamgo910@gmail.com",
                    Username = "DanyK",
                    PasswordHash = "hashed_password_2",
                    Role = "User",
                    CreatedAt = new DateTime(2025, 11, 14)
                },
                new User
                {
                    Id = 3,
                    Name = "John",
                    Email = "johnsmith@gmail.com",
                    Username = "JohnS",
                    PasswordHash = "hashed_password_3",
                    Role = "User",
                    CreatedAt = new DateTime(2025, 11, 15)
                },
                new User
                {
                    Id = 4,
                    Name = "Greg",
                    Email = "gregroberts@gmail.com",
                    Username = "GregR",
                    PasswordHash = "hashed_password_4",
                    Role = "User",
                    CreatedAt = new DateTime(2025, 11, 16)
                },
                new User
                {
                    Id = 5,
                    Name = "Alice",
                    Email = "alicejohnson@gmail.com",
                    Username = "AliceJ",
                    PasswordHash = "hashed_password_5",
                    Role = "User",
                    CreatedAt = new DateTime(2025, 11, 17)
                }
            );
            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem
                {
                    Id = 1,
                    Title = "Initial Task",
                    Description = "This is the first task in the system.",
                    Status = TaskStatusTypes.Development,
                    CreatedById = 1,
                    AssignedToId = 2,
                    CreatedAt = new DateTime(2025, 11, 13),
                    UpdatedAt = new DateTime(2025, 11, 14),
                    IsArchived = false,
                    ArchivedAt = null
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Second Task",
                    Description = "This is the second task in the system.",
                    Status = TaskStatusTypes.ToDo,
                    CreatedById = 1,
                    AssignedToId = 1,
                    CreatedAt = new DateTime(2025, 11, 15),
                    UpdatedAt = new DateTime(2025, 11, 16),
                    IsArchived = false,
                    ArchivedAt = new DateTime(2025, 11, 20)
                },
                new TaskItem
                {
                    Id = 3,
                    Title = "Third Task",
                    Description = "This is the third task in the system.",
                    Status = TaskStatusTypes.Done,
                    CreatedById = 1,
                    AssignedToId = 2,
                    CreatedAt = new DateTime(2025, 11, 17),
                    UpdatedAt = new DateTime(2025, 11, 18),
                    IsArchived = false,
                    ArchivedAt = null
                },
                new TaskItem
                {
                    Id = 4,
                    Title = "Fourth Task",
                    Description = "This is the fourth task in the system.",
                    Status = TaskStatusTypes.Review,
                    CreatedById = 1,
                    AssignedToId = null,
                    CreatedAt = new DateTime(2025, 12, 10),
                    UpdatedAt = new DateTime(2025, 12, 12),
                    IsArchived = false,
                    ArchivedAt = null
                }
            );
        }
    }
}
