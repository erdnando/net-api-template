using System.ComponentModel.DataAnnotations;
using netapi_template.Models;

namespace netapi_template.DTOs;

public record CreateUserDto(
    [Required][StringLength(100)] string FirstName,
    [Required][StringLength(100)] string LastName,
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password,
    [Required] int RoleId,
    UserStatus Status = UserStatus.Active,
    string? Avatar = null
);

public record UpdateUserDto(
    [StringLength(100)] string? FirstName,
    [StringLength(100)] string? LastName,
    [EmailAddress] string? Email,
    int? RoleId,
    UserStatus? Status,
    string? Avatar
);

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public RoleDto? Role { get; set; }
    public UserStatus Status { get; set; }
    public bool IsActive { get; set; }
    public string? Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<UserPermissionDto>? Permissions { get; set; } = new List<UserPermissionDto>();
}

public record LoginDto(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    
    public LoginResponseDto() { }
    
    public LoginResponseDto(string token, UserDto user)
    {
        Token = token;
        User = user;
    }
}

public record ChangePasswordDto(
    [Required] string CurrentPassword,
    [Required][MinLength(6)] string NewPassword
);

public record ResetPasswordDto(
    [Required][EmailAddress] string Email
);

public record SetPasswordDto(
    [Required] string Token,
    [Required][MinLength(6)] string NewPassword
);
