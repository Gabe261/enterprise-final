using System.ComponentModel.DataAnnotations;

namespace TaskCollaborationAppAPI.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } 
        public string? Description { get; set; }
        [Required]
        public TaskStatusTypes Status { get; set; }

        public int CreatedById { get; set; }
        public int? AssignedToId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        [Required]
        public bool IsArchived { get; set; }
        public DateTime? ArchivedAt { get; set; }

    }
}
