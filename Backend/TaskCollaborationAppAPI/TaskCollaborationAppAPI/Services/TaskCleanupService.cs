using System.Diagnostics;
using TaskCollaborationAppAPI.Data;

namespace TaskCollaborationAppAPI.Services
{
    public class TaskCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public TaskCleanupService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var threshold = DateTime.UtcNow.AddSeconds(-5);

                    var oldCompletedTasks = db.Tasks
                        .Where(t => t.Status == Models.TaskStatusTypes.Done && t.UpdatedAt <= threshold)
                        .ToList();

                    foreach (var task in oldCompletedTasks)
                    {
                        task.IsArchived = true;
                        task.ArchivedAt = DateTime.UtcNow;
                        db.SaveChanges();
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }
}
