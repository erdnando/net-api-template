using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netapi_template.Models;

[Table("UserPermissions")]
public class UserPermission
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ModuleId { get; set; }

    [Required]
    public int PermissionType { get; set; } = 0; // 0=None, 10=Read, 20=Write

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Module Module { get; set; } = null!;
}
