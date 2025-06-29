using AutoMapper;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.DTOs;
using netapi_template.Helpers;
using netapi_template.Models;
using netapi_template.Services.Interfaces;

namespace netapi_template.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private readonly JwtHelper _jwtHelper;
    private readonly ISecurityLoggerService _securityLogger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(
        ApplicationDbContext context, 
        IMapper mapper, 
        ILogger<UserService> logger, 
        JwtHelper jwtHelper,
        ISecurityLoggerService securityLogger,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _jwtHelper = jwtHelper;
        _securityLogger = securityLogger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<UserDto>> GetUserByIdAsync(int id)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
                
            if (user == null)
            {
                return new ApiResponse<UserDto>(false, "Usuario no encontrado");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return new ApiResponse<UserDto>(true, "Usuario encontrado", userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario con ID {UserId}", id);
            return new ApiResponse<UserDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<PagedResult<UserDto>>> GetAllUsersAsync(PaginationQuery? query = null)
    {
        try
        {
            var queryable = _context.Users.Include(u => u.Role).AsQueryable();

            // Apply filters if provided
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Search))
                {
                    queryable = queryable.Where(u => 
                        u.FirstName.Contains(query.Search) ||
                        u.LastName.Contains(query.Search) ||
                        u.Email.Contains(query.Search));
                }

                if (!string.IsNullOrEmpty(query.SortBy))
                {
                    switch (query.SortBy.ToLower())
                    {
                        case "firstname":
                        case "nombre":
                            queryable = query.SortDescending
                                ? queryable.OrderByDescending(u => u.FirstName)
                                : queryable.OrderBy(u => u.FirstName);
                            break;
                        case "lastname":
                        case "apellidos":
                            queryable = query.SortDescending
                                ? queryable.OrderByDescending(u => u.LastName)
                                : queryable.OrderBy(u => u.LastName);
                            break;
                        case "email":
                            queryable = query.SortDescending
                                ? queryable.OrderByDescending(u => u.Email)
                                : queryable.OrderBy(u => u.Email);
                            break;
                        case "createdat":
                        case "fechaalta":
                            queryable = query.SortDescending
                                ? queryable.OrderByDescending(u => u.CreatedAt)
                                : queryable.OrderBy(u => u.CreatedAt);
                            break;
                        default:
                            queryable = queryable.OrderBy(u => u.Id);
                            break;
                    }
                }
                else
                {
                    queryable = queryable.OrderBy(u => u.Id);
                }
            }
            else
            {
                queryable = queryable.OrderBy(u => u.Id);
            }

            var totalCount = await queryable.CountAsync();
            var pageSize = query?.PageSize ?? 10;
            var currentPage = query?.Page ?? 1;
            
            var users = await queryable
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDtos = _mapper.Map<List<UserDto>>(users);
            
            var pagedResult = PagedResult<UserDto>.Create(
                userDtos,
                currentPage,
                pageSize,
                totalCount
            );

            return new ApiResponse<PagedResult<UserDto>>(true, "Usuarios obtenidos exitosamente", pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            return new ApiResponse<PagedResult<UserDto>>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == createUserDto.Email);
            
            if (existingUser != null)
            {
                return new ApiResponse<UserDto>(false, "El email ya está en uso");
            }

            // Check if role exists
            var role = await _context.Roles.FindAsync(createUserDto.RoleId);
            if (role == null)
            {
                return new ApiResponse<UserDto>(false, "El rol especificado no existe");
            }

            var user = _mapper.Map<User>(createUserDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
            user.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Load the user with role for response
            var createdUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            var userDto = _mapper.Map<UserDto>(createdUser);
            return new ApiResponse<UserDto>(true, "Usuario creado exitosamente", userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            return new ApiResponse<UserDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
                
            if (user == null)
            {
                return new ApiResponse<UserDto>(false, "Usuario no encontrado");
            }

            // Check if email is being changed and already exists
            if (updateUserDto.Email != user.Email)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == updateUserDto.Email && u.Id != id);
                
                if (existingUser != null)
                {
                    return new ApiResponse<UserDto>(false, "El email ya está en uso");
                }
            }

            // Check if role exists
            if (updateUserDto.RoleId.HasValue)
            {
                var role = await _context.Roles.FindAsync(updateUserDto.RoleId.Value);
                if (role == null)
                {
                    return new ApiResponse<UserDto>(false, "El rol especificado no existe");
                }
            }

            _mapper.Map(updateUserDto, user);
            
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            return new ApiResponse<UserDto>(true, "Usuario actualizado exitosamente", userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario con ID {UserId}", id);
            return new ApiResponse<UserDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return new ApiResponse<bool>(false, "Usuario no encontrado");
            }

            // Check if user has permissions that need to be removed first
            var hasPermissions = await _context.UserPermissions
                .AnyAsync(up => up.UserId == id);
                
            if (hasPermissions)
            {
                // Remove all user permissions first
                var permissions = await _context.UserPermissions
                    .Where(up => up.UserId == id)
                    .ToListAsync();
                _context.UserPermissions.RemoveRange(permissions);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Usuario eliminado exitosamente", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario con ID {UserId}", id);
            return new ApiResponse<bool>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.GetClientIpAddress() ?? "Unknown";
        var userAgent = _httpContextAccessor.HttpContext?.GetUserAgent() ?? "Unknown";
        
        try
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _securityLogger.LogFailedLogin(loginDto.Email, ipAddress, userAgent);
                return new ApiResponse<LoginResponseDto>(false, "Credenciales inválidas");
            }

            if (user.Status != UserStatus.Active)
            {
                _securityLogger.LogFailedLogin(loginDto.Email, ipAddress, userAgent);
                _securityLogger.LogSuspiciousActivity(loginDto.Email, "Login attempt with inactive account", ipAddress);
                return new ApiResponse<LoginResponseDto>(false, "Usuario inactivo");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            var token = _jwtHelper.GenerateToken(user);
            var userDto = _mapper.Map<UserDto>(user);

            var response = new LoginResponseDto(token, userDto);

            _securityLogger.LogSuccessfulLogin(user.Email, ipAddress, userAgent);
            return new ApiResponse<LoginResponseDto>(true, "Login exitoso", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el login para email {Email}", loginDto.Email);
            _securityLogger.LogFailedLogin(loginDto.Email, ipAddress, userAgent);
            return new ApiResponse<LoginResponseDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.GetClientIpAddress() ?? "Unknown";
        
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<bool>(false, "Usuario no encontrado");
            }

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                _securityLogger.LogPasswordChange(user.Email, ipAddress, false);
                return new ApiResponse<bool>(false, "Contraseña actual incorrecta");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _context.SaveChangesAsync();

            _securityLogger.LogPasswordChange(user.Email, ipAddress, true);
            return new ApiResponse<bool>(true, "Contraseña cambiada exitosamente", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar contraseña para usuario {UserId}", userId);
            return new ApiResponse<bool>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<UserDto>> GetUserByEmailAsync(string email)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
                
            if (user == null)
            {
                return new ApiResponse<UserDto>(false, "Usuario no encontrado");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return new ApiResponse<UserDto>(true, "Usuario encontrado", userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario por email {Email}", email);
            return new ApiResponse<UserDto>(false, "Error interno del servidor");
        }
    }
}
