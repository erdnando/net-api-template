using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using netapi_template.DTOs;
using netapi_template.Models;
using netapi_template.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace netapi_template.Controllers;

/// <summary>
/// Controller for permission and module management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    #region Modules

    /// <summary>
    /// Get all modules with pagination and filtering
    /// </summary>
    [HttpGet("modules")]
    [SwaggerOperation(Summary = "Get all modules", Description = "Retrieve a paginated list of modules with optional filtering")]
    [SwaggerResponse(200, "Modules retrieved successfully", typeof(ApiResponse<PagedResult<ModuleDto>>))]
    public async Task<IActionResult> GetAllModules(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false)
    {
        var query = new PaginationQuery(page, pageSize, search, sortBy, sortDescending);
        var response = await _permissionService.GetAllModulesAsync(query);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Get module by ID
    /// </summary>
    [HttpGet("modules/{id}")]
    [SwaggerOperation(Summary = "Get module by ID", Description = "Retrieve a specific module by its ID")]
    [SwaggerResponse(200, "Module found", typeof(ApiResponse<ModuleDto>))]
    [SwaggerResponse(404, "Module not found")]
    public async Task<IActionResult> GetModuleById([FromRoute] int id)
    {
        var response = await _permissionService.GetModuleByIdAsync(id);
        
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Create a new module
    /// </summary>
    [HttpPost("modules")]
    [SwaggerOperation(Summary = "Create module", Description = "Create a new module")]
    [SwaggerResponse(201, "Module created successfully", typeof(ApiResponse<ModuleDto>))]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<IActionResult> CreateModule([FromBody] CreateModuleDto createModuleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<ModuleDto>(false, "Invalid request data", null, 
                ModelState.SelectMany(x => x.Value?.Errors?.Select(e => e.ErrorMessage) ?? []).ToList()));
        }

        var response = await _permissionService.CreateModuleAsync(createModuleDto);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(nameof(GetModuleById), new { id = response.Data!.Id }, response);
    }

    /// <summary>
    /// Update an existing module
    /// </summary>
    [HttpPut("modules/{id}")]
    [SwaggerOperation(Summary = "Update module", Description = "Update an existing module")]
    [SwaggerResponse(200, "Module updated successfully", typeof(ApiResponse<ModuleDto>))]
    [SwaggerResponse(404, "Module not found")]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<IActionResult> UpdateModule([FromRoute] int id, [FromBody] UpdateModuleDto updateModuleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<ModuleDto>(false, "Invalid request data", null, 
                ModelState.SelectMany(x => x.Value?.Errors?.Select(e => e.ErrorMessage) ?? []).ToList()));
        }

        var response = await _permissionService.UpdateModuleAsync(id, updateModuleDto);
        
        if (!response.Success)
        {
            return response.Message.Contains("not found") ? NotFound(response) : BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Delete a module (soft delete)
    /// </summary>
    [HttpDelete("modules/{id}")]
    [SwaggerOperation(Summary = "Delete module", Description = "Soft delete a module")]
    [SwaggerResponse(200, "Module deleted successfully", typeof(ApiResponse<bool>))]
    [SwaggerResponse(404, "Module not found")]
    [SwaggerResponse(400, "Cannot delete module")]
    public async Task<IActionResult> DeleteModule([FromRoute] int id)
    {
        var response = await _permissionService.DeleteModuleAsync(id);
        
        if (!response.Success)
        {
            return response.Message.Contains("not found") ? NotFound(response) : BadRequest(response);
        }

        return Ok(response);
    }

    #endregion

    #region User Permissions

    /// <summary>
    /// Get user permissions
    /// </summary>
    [HttpGet("users/{userId}")]
    [SwaggerOperation(Summary = "Get user permissions", Description = "Retrieve all permissions for a specific user")]
    [SwaggerResponse(200, "User permissions retrieved successfully", typeof(ApiResponse<List<UserPermissionDto>>))]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> GetUserPermissions([FromRoute] int userId)
    {
        var response = await _permissionService.GetPermissionsByUserIdAsync(userId);
        
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Update user permissions
    /// </summary>
    [HttpPut("users/{userId}")]
    [SwaggerOperation(Summary = "Update user permissions", Description = "Update all permissions for a specific user")]
    [SwaggerResponse(200, "User permissions updated successfully", typeof(ApiResponse<bool>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<IActionResult> UpdateUserPermissions([FromRoute] int userId, [FromBody] List<UpdateUserPermissionDto> permissions)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<bool>(false, "Invalid request data", false, 
                ModelState.SelectMany(x => x.Value?.Errors?.Select(e => e.ErrorMessage) ?? []).ToList()));
        }

        var response = await _permissionService.UpdateUserPermissionsAsync(userId, permissions);
        
        if (!response.Success)
        {
            return response.Message.Contains("not found") ? NotFound(response) : BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Remove specific user permission
    /// </summary>
    [HttpDelete("users/{userId}/modules/{moduleId}")]
    [SwaggerOperation(Summary = "Remove user permission", Description = "Remove a specific permission from a user")]
    [SwaggerResponse(200, "User permission removed successfully", typeof(ApiResponse<bool>))]
    [SwaggerResponse(404, "Permission not found")]
    public async Task<IActionResult> RemoveUserPermission([FromRoute] int userId, [FromRoute] int moduleId)
    {
        var response = await _permissionService.RemoveUserPermissionAsync(userId, moduleId);
        
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Check if user has specific permission for a module
    /// </summary>
    [HttpGet("users/{userId}/modules/{moduleCode}/check")]
    [SwaggerOperation(Summary = "Check user permission", Description = "Check if user has specific permission for a module")]
    [SwaggerResponse(200, "Permission check completed", typeof(ApiResponse<bool>))]
    [SwaggerResponse(404, "User or module not found")]
    public async Task<IActionResult> CheckUserPermission([FromRoute] int userId, [FromRoute] string moduleCode, [FromQuery] PermissionType requiredPermission = PermissionType.Read)
    {
        var response = await _permissionService.HasPermissionAsync(userId, moduleCode, requiredPermission);
        
        if (!response.Success && response.Message.Contains("not found"))
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Get user module permissions map
    /// </summary>
    [HttpGet("users/{userId}/modules")]
    [SwaggerOperation(Summary = "Get user module permissions", Description = "Get a map of all module permissions for a user")]
    [SwaggerResponse(200, "Module permissions retrieved successfully", typeof(ApiResponse<Dictionary<string, PermissionType>>))]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> GetUserModulePermissions([FromRoute] int userId)
    {
        var response = await _permissionService.GetUserModulePermissionsAsync(userId);
        
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Assign permission to user
    /// </summary>
    [HttpPost("users/{userId}/modules/{moduleId}")]
    [SwaggerOperation(Summary = "Assign permission", Description = "Assign a specific permission to a user for a module")]
    [SwaggerResponse(201, "Permission assigned successfully", typeof(ApiResponse<UserPermissionDto>))]
    [SwaggerResponse(400, "Invalid request or permission already exists")]
    [SwaggerResponse(404, "User or module not found")]
    public async Task<IActionResult> AssignPermission([FromRoute] int userId, [FromRoute] int moduleId, [FromBody] CreateUserPermissionDto createPermissionDto)
    {
        // Override the IDs from route parameters
        var permission = new CreateUserPermissionDto(userId, moduleId, createPermissionDto.PermissionType);
        
        var response = await _permissionService.AssignPermissionAsync(permission);
        
        if (!response.Success)
        {
            return response.Message.Contains("not found") ? NotFound(response) : BadRequest(response);
        }

        return CreatedAtAction(nameof(GetUserPermissions), new { userId = userId }, response);
    }

    #endregion
}
