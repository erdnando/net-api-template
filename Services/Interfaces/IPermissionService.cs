using netapi_template.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace netapi_template.Services.Interfaces
{
    public interface IPermissionService
    {
        // Module CRUD
        Task<ApiResponse<PagedResult<ModuleDto>>> GetAllModulesAsync(PaginationQuery query);
        Task<ApiResponse<ModuleDto>> GetModuleByIdAsync(int id);
        Task<ApiResponse<ModuleDto>> CreateModuleAsync(CreatePermissionModuleDto dto);
        Task<ApiResponse<ModuleDto>> UpdateModuleAsync(int id, UpdatePermissionModuleDto dto);
        Task<ApiResponse<bool>> DeleteModuleAsync(int id);

        // User Permissions CRUD
        Task<ApiResponse<List<UserPermissionDto>>> GetPermissionsByUserIdAsync(int userId);
        Task<ApiResponse<bool>> UpdateUserPermissionsAsync(int userId, List<UpdateUserPermissionDto> permissions);
        Task<ApiResponse<bool>> RemoveUserPermissionAsync(int userId, int moduleId);
        Task<ApiResponse<bool>> HasPermissionAsync(int userId, string moduleCode);
        Task<ApiResponse<Dictionary<string, int>>> GetUserModulePermissionsAsync(int userId);
        Task<ApiResponse<UserPermissionDto>> AssignPermissionAsync(CreateUserPermissionDto dto);
    }
}
