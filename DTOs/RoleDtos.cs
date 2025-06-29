using System.ComponentModel.DataAnnotations;

namespace netapi_template.DTOs;

public record CreateRoleDto(
    [Required][StringLength(100)] string Name,
    [StringLength(255)] string? Description = null
);

public record UpdateRoleDto(
    [StringLength(100)] string? Name,
    [StringLength(255)] string? Description
);

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
