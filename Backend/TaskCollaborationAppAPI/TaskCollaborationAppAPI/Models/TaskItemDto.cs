namespace TaskCollaborationAppAPI.Models
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }

        public int CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedByEmail { get; set; }

        public int? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public string? AssignedToEmail { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsArchived { get; set; }
    }

    public class CreateTaskDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public int? AssignedToId { get; set; }
    }

    public class UpdateTaskDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int? AssignedToId { get; set; }
    }
}
