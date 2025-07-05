using netapi_template.DTOs;
using netapi_template.Models;

namespace netapi_template.Services.Interfaces;

public interface IPermissionService
{
    // Module Management
    Task<ApiResponse<PagedResult<ModuleDto>>> GetAllModulesAsync(PaginationQuery? query = null);
    Task<ApiResponse<ModuleDto>> GetModuleByIdAsync(int id);
    Task<ApiResponse<ModuleDto>> CreateModuleAsync(CreatePermissionModuleDto createDto);
    Task<ApiResponse<ModuleDto>> UpdateModuleAsync(int id, UpdatePermissionModuleDto updateDto);
    Task<ApiResponse<bool>> DeleteModuleAsync(int id);

    // User Permissions Management
    Task<ApiResponse<PagedResult<UserPermissionDto>>> GetUserPermissionsAsync(int userId, PaginationQuery? query = null);
    Task<ApiResponse<UserPermissionDto>> AssignPermissionAsync(CreateUserPermissionDto createDto);
    Task<ApiResponse<UserPermissionDto>> UpdatePermissionAsync(int permissionId, UpdateUserPermissionDto updateDto);
    Task<ApiResponse<bool>> RemovePermissionAsync(int permissionId);
    Task<ApiResponse<bool>> RemoveAllUserPermissionsAsync(int userId);

    // Permission Queries
    Task<ApiResponse<List<UserPermissionDto>>> GetPermissionsByUserIdAsync(int userId);
    Task<ApiResponse<List<UserPermissionDto>>> GetPermissionsByModuleIdAsync(int moduleId);
    Task<ApiResponse<bool>> HasPermissionAsync(int userId, string moduleCode, PermissionType requiredPermission);
    Task<ApiResponse<Dictionary<string, PermissionType>>> GetUserModulePermissionsAsync(int userId);
    Task<ApiResponse<bool>> RemoveUserPermissionAsync(int userId, int moduleId);

    // Bulk Operations
    Task<ApiResponse<List<UserPermissionDto>>> AssignMultiplePermissionsAsync(int userId, List<CreateUserPermissionDto> permissions);
    Task<ApiResponse<bool>> UpdateUserPermissionsAsync(int userId, List<UpdateUserPermissionDto> permissions);
}
