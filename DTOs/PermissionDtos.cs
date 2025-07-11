using System.ComponentModel.DataAnnotations;

namespace netapi_template.DTOs;

public class UserPermissionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ModuleId { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public string ModuleCode { get; set; } = string.Empty;
    public int PermissionType { get; set; } // Usar int para compatibilidad
    public DateTime CreatedAt { get; set; }
    
    public UserPermissionDto() { }
    
    public UserPermissionDto(int id, int userId, int moduleId, string moduleName, string moduleCode, int permissionType, DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        ModuleId = moduleId;
        ModuleName = moduleName;
        ModuleCode = moduleCode;
        PermissionType = permissionType;
        CreatedAt = createdAt;
    }
}

public record CreateUserPermissionDto(
    [Required] int UserId,
    [Required] int ModuleId,
    [Required] int PermissionType
);

public record UpdateUserPermissionDto(
    int Id,
    [Required] int ModuleId,
    [Required] int PermissionType
);

public record UpdateUserPermissionsDto(
    [Required] int UserId,
    [Required] List<ModulePermissionDto> Permissions
);

public record ModulePermissionDto(
    [Required] int ModuleId,
    [Required] int PermissionType
);

public class ModuleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public ModuleDto() { }
    
    public ModuleDto(int id, string name, string? description, string code, bool isActive, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        Name = name;
        Description = description;
        Code = code;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}

public record CreatePermissionModuleDto(
    [Required][StringLength(100)] string Name,
    [StringLength(255)] string? Description,
    [Required][StringLength(100)] string Code,
    bool? IsActive
);

public record UpdatePermissionModuleDto(
    [StringLength(100)] string? Name,
    [StringLength(255)] string? Description,
    [StringLength(100)] string? Code,
    bool? IsActive
);
