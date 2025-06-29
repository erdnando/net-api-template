using netapi_template.DTOs;

namespace netapi_template.Services.Interfaces;

public interface IRoleService
{
    Task<ApiResponse<PagedResult<RoleDto>>> GetAllRolesAsync(PaginationQuery? query = null);
    Task<ApiResponse<RoleDto>> GetRoleByIdAsync(int id);
    Task<ApiResponse<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto);
    Task<ApiResponse<RoleDto>> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto);
    Task<ApiResponse<bool>> DeleteRoleAsync(int id);
}
