using FluentValidation;
using netapi_template.DTOs;
using netapi_template.Data;
using Microsoft.EntityFrameworkCore;

namespace netapi_template.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    private readonly ApplicationDbContext _context;

    public CreateUserDtoValidator(ApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MustAsync(BeUniqueEmail).WithMessage("Email already exists");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role is required")
            .MustAsync(BeValidRole).WithMessage("Invalid role specified");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
    }

    private async Task<bool> BeValidRole(int roleId, CancellationToken cancellationToken)
    {
        return await _context.Roles.AnyAsync(r => r.Id == roleId && !r.IsDeleted, cancellationToken);
    }
}

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    private readonly ApplicationDbContext _context;

    public UpdateUserDtoValidator(ApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.LastName));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MustAsync(BeUniqueEmailForUpdate).WithMessage("Email already exists")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.RoleId)
            .MustAsync(BeValidRole).WithMessage("Invalid role specified")
            .When(x => x.RoleId.HasValue);
    }

    private async Task<bool> BeUniqueEmailForUpdate(UpdateUserDto dto, string email, CancellationToken cancellationToken)
    {
        // This would need the user ID in a real scenario, simplified for now
        return !await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
    }

    private async Task<bool> BeValidRole(int? roleId, CancellationToken cancellationToken)
    {
        if (!roleId.HasValue) return true;
        return await _context.Roles.AnyAsync(r => r.Id == roleId.Value && !r.IsDeleted, cancellationToken);
    }
}

public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
{
    private readonly ApplicationDbContext _context;

    public CreateRoleDtoValidator(ApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required")
            .MaximumLength(100).WithMessage("Role name must not exceed 100 characters")
            .MustAsync(BeUniqueRoleName).WithMessage("Role name already exists");

        RuleFor(x => x.Description)
            .MaximumLength(255).WithMessage("Description must not exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }

    private async Task<bool> BeUniqueRoleName(string name, CancellationToken cancellationToken)
    {
        return !await _context.Roles.AnyAsync(r => r.Name == name && !r.IsDeleted, cancellationToken);
    }
}

public class UpdateUserPermissionsDtoValidator : AbstractValidator<UpdateUserPermissionsDto>
{
    private readonly ApplicationDbContext _context;

    public UpdateUserPermissionsDtoValidator(ApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required")
            .MustAsync(BeValidUser).WithMessage("Invalid user specified");

        RuleFor(x => x.Permissions)
            .NotNull().WithMessage("Permissions list is required");

        RuleForEach(x => x.Permissions)
            .SetValidator(new ModulePermissionDtoValidator(_context));
    }

    private async Task<bool> BeValidUser(int userId, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
    }
}

public class ModulePermissionDtoValidator : AbstractValidator<ModulePermissionDto>
{
    private readonly ApplicationDbContext _context;

    public ModulePermissionDtoValidator(ApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.ModuleId)
            .NotEmpty().WithMessage("Module ID is required")
            .MustAsync(BeValidModule).WithMessage("Invalid module specified");

        RuleFor(x => x.PermissionType)
            .IsInEnum().WithMessage("Invalid permission type");
    }

    private async Task<bool> BeValidModule(int moduleId, CancellationToken cancellationToken)
    {
        return await _context.Modules.AnyAsync(m => m.Id == moduleId && !m.IsDeleted, cancellationToken);
    }
}
