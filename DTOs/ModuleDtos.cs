using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace netapi_template.DTOs;

public class ModuleResponseDto
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(255)]
    public string? Description { get; set; }
    [Required]
    [MaxLength(100)]
    public string Path { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Icon { get; set; } = string.Empty;
    public bool AdminOnly { get; set; } = false;
    public int Order { get; set; }
}

public class CreateModuleDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(255)]
    public string? Description { get; set; }
    [Required]
    [MaxLength(100)]
    public string Path { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Icon { get; set; } = string.Empty;
    public bool AdminOnly { get; set; } = false;
    public int Order { get; set; }
    [Required]
    [MaxLength(50)]
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}

public class UpdateModuleDto : CreateModuleDto {}
