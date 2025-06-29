using netapi_template.DTOs;

namespace netapi_template.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse<UserDto>> GetUserByIdAsync(int id);
    Task<ApiResponse<PagedResult<UserDto>>> GetAllUsersAsync(PaginationQuery? query = null);
    Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
    Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
    Task<ApiResponse<bool>> DeleteUserAsync(int id);
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto);
    Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    Task<ApiResponse<UserDto>> GetUserByEmailAsync(string email);
}
