using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskCollaborationAppAPI.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string? Description { get; set; }
        public TaskStatusTypes Status { get; set; }
        public int CreatedById { get; set; }
        public int? AssignedToId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? ArchivedAt { get; set; }

    }
}
