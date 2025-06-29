using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netapi_template.Models;

[Table("Users")]
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public int RoleId { get; set; }

    [Required]
    public UserStatus Status { get; set; } = UserStatus.Active;

    [MaxLength(255)]
    public string? Avatar { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastLoginAt { get; set; }

    // Computed property
    [NotMapped]
    public bool IsActive => Status == UserStatus.Active;

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    // Soft delete property
    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
