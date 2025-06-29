using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netapi_template.Models
{
    [Table("Tasks")]
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        public bool Completed { get; set; } = false;
        
        [Required]
        [MaxLength(10)]
        public string Priority { get; set; } = "medium"; // low, medium, high
        
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign Key to User
        [Required]
        public int UserId { get; set; }
        
        // Navigation property
        public virtual User? User { get; set; }
    }
}
