using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using netapi_template.Attributes;
using netapi_template.Data;
using netapi_template.DTOs;
using netapi_template.Models;
using netapi_template.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace netapi_template.Controllers;

/// <summary>
/// Controller for role management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RolesController> _logger;

    public RolesController(IRoleService roleService, ApplicationDbContext context, ILogger<RolesController> logger)
    {
        _roleService = roleService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all roles with pagination and filtering
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all roles", Description = "Retrieve a paginated list of roles with optional filtering")]
    [SwaggerResponse(200, "Roles retrieved successfully", typeof(ApiResponse<PagedResult<RoleDto>>))]
    public async Task<IActionResult> GetAllRoles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false)
    {
        var query = new PaginationQuery(page, pageSize, search, sortBy, sortDescending);
        var response = await _roleService.GetAllRolesAsync(query);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Get role by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get role by ID", Description = "Retrieve a specific role by its ID")]
    [SwaggerResponse(200, "Role found", typeof(ApiResponse<RoleDto>))]
    [SwaggerResponse(404, "Role not found")]
    public async Task<IActionResult> GetRoleById([FromRoute] int id)
    {
        var response = await _roleService.GetRoleByIdAsync(id);
        
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    [HttpPost]
    // Skip any attribute that might interfere with direct execution
    [SwaggerOperation(Summary = "Create role", Description = "Create a new role")]
    [SwaggerResponse(201, "Role created successfully", typeof(ApiResponse<RoleDto>))]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
    {
        try
        {
            // Manual basic validation
            if (string.IsNullOrEmpty(createRoleDto.Name))
            {
                return BadRequest(new ApiResponse<RoleDto>(false, "Role name is required"));
            }
            
            if (createRoleDto.Name.Length > 100)
            {
                return BadRequest(new ApiResponse<RoleDto>(false, "Role name must not exceed 100 characters"));
            }
            
            // Use a direct database check to bypass the validator
            if (await _context.Roles.AnyAsync(r => r.Name == createRoleDto.Name && !r.IsDeleted))
            {
                return BadRequest(new ApiResponse<RoleDto>(false, "Role name already exists"));
            }
            
            // Create the role directly using DbContext
            var role = new Role
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description ?? string.Empty,
                IsSystemRole = false,
                CreatedAt = DateTime.UtcNow
            };
            
            // Add with entity state tracking
            var entry = _context.Roles.Add(role);
            
            // Save with explicit transaction to ensure persistence
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                _logger.LogInformation("Role created successfully: {RoleName} with ID {RoleId}", role.Name, role.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to save role to database: {Message}", ex.Message);
                throw;
            }
            
            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsSystemRole = role.IsSystemRole,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            };
            
            return CreatedAtAction(nameof(GetRoleById), new { id = roleDto.Id }, 
                new ApiResponse<RoleDto>(true, "Role created successfully", roleDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role: {Message}", ex.Message);
            return BadRequest(new ApiResponse<RoleDto>(false, $"Error creating role: {ex.Message}"));
        }
    }

    /// <summary>
    /// Update an existing role
    /// </summary>
    [HttpPut("{id}")]
    [DisableValidation]
    [SwaggerOperation(Summary = "Update role", Description = "Update an existing role")]
    [SwaggerResponse(200, "Role updated successfully", typeof(ApiResponse<RoleDto>))]
    [SwaggerResponse(404, "Role not found")]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<IActionResult> UpdateRole([FromRoute] int id, [FromBody] UpdateRoleDto updateRoleDto)
    {
        // Skip automatic validation as it's handled in the service
        var response = await _roleService.UpdateRoleAsync(id, updateRoleDto);
        
        if (!response.Success)
        {
            return response.Message.Contains("not found") ? NotFound(response) : BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Delete a role (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete role", Description = "Soft delete a role")]
    [SwaggerResponse(200, "Role deleted successfully", typeof(ApiResponse<bool>))]
    [SwaggerResponse(404, "Role not found")]
    [SwaggerResponse(400, "Cannot delete role")]
    public async Task<IActionResult> DeleteRole([FromRoute] int id)
    {
        var response = await _roleService.DeleteRoleAsync(id);
        
        if (!response.Success)
        {
            return response.Message.Contains("not found") ? NotFound(response) : BadRequest(response);
        }

        return Ok(response);
    }
}
