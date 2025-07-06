using AutoMapper;
using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.DTOs;
using netapi_template.Models;
using netapi_template.Services.Interfaces;

namespace netapi_template.Services;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(ApplicationDbContext context, IMapper mapper, ILogger<PermissionService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    // Module Management
    public async Task<ApiResponse<PagedResult<ModuleDto>>> GetAllModulesAsync(PaginationQuery? query = null)
    {
        try
        {
            query ??= new PaginationQuery();
            
            var modulesQuery = _context.Modules.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(query.Search))
            {
                modulesQuery = modulesQuery.Where(m => 
                    m.Name.Contains(query.Search) ||
                    m.Code.Contains(query.Search) ||
                    (m.Description != null && m.Description.Contains(query.Search)));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                modulesQuery = query.SortBy.ToLower() switch
                {
                    "name" => query.SortDescending ? modulesQuery.OrderByDescending(m => m.Name) : modulesQuery.OrderBy(m => m.Name),
                    "code" => query.SortDescending ? modulesQuery.OrderByDescending(m => m.Code) : modulesQuery.OrderBy(m => m.Code),
                    "createdat" => query.SortDescending ? modulesQuery.OrderByDescending(m => m.CreatedAt) : modulesQuery.OrderBy(m => m.CreatedAt),
                    _ => modulesQuery.OrderBy(m => m.Id)
                };
            }
            else
            {
                modulesQuery = modulesQuery.OrderBy(m => m.Id);
            }

            var totalRecords = await modulesQuery.CountAsync();
            var modules = await modulesQuery
                .Skip(query.Skip)
                .Take(query.Take)
                .ToListAsync();

            var moduleDtos = _mapper.Map<List<ModuleDto>>(modules);
            var pagedResult = PagedResult<ModuleDto>.Create(moduleDtos, query.Page, query.PageSize, totalRecords);
            
            return new ApiResponse<PagedResult<ModuleDto>>(true, "Modules retrieved successfully", pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving modules");
            return new ApiResponse<PagedResult<ModuleDto>>(false, "Error retrieving modules");
        }
    }

    public async Task<ApiResponse<ModuleDto>> GetModuleByIdAsync(int id)
    {
        try
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
            {
                return new ApiResponse<ModuleDto>(false, "Módulo no encontrado");
            }

            var moduleDto = _mapper.Map<ModuleDto>(module);
            return new ApiResponse<ModuleDto>(true, "Módulo encontrado", moduleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener módulo con ID {ModuleId}", id);
            return new ApiResponse<ModuleDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<ModuleDto>> CreateModuleAsync(CreatePermissionModuleDto createDto)
    {
        try
        {
            // Check if code already exists
            var existingModule = await _context.Modules
                .FirstOrDefaultAsync(m => m.Code == createDto.Code);
            
            if (existingModule != null)
            {
                return new ApiResponse<ModuleDto>(false, "El código de módulo ya existe");
            }

            var module = _mapper.Map<Module>(createDto);
            module.CreatedAt = DateTime.UtcNow;
            module.IsActive = true;

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();

            var moduleDto = _mapper.Map<ModuleDto>(module);
            return new ApiResponse<ModuleDto>(true, "Módulo creado exitosamente", moduleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear módulo");
            return new ApiResponse<ModuleDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<ModuleDto>> UpdateModuleAsync(int id, UpdatePermissionModuleDto updateDto)
    {
        try
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
            {
                return new ApiResponse<ModuleDto>(false, "Módulo no encontrado");
            }

            // Check code uniqueness if it's being updated
            if (!string.IsNullOrEmpty(updateDto.Code) && updateDto.Code != module.Code)
            {
                var existingModule = await _context.Modules
                    .FirstOrDefaultAsync(m => m.Code == updateDto.Code && m.Id != id);
                
                if (existingModule != null)
                {
                    return new ApiResponse<ModuleDto>(false, "El código de módulo ya existe");
                }
            }

            _mapper.Map(updateDto, module);
            module.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            var moduleDto = _mapper.Map<ModuleDto>(module);
            return new ApiResponse<ModuleDto>(true, "Módulo actualizado exitosamente", moduleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar módulo con ID {ModuleId}", id);
            return new ApiResponse<ModuleDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<bool>> DeleteModuleAsync(int id)
    {
        try
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
            {
                return new ApiResponse<bool>(false, "Módulo no encontrado");
            }

            // Check if there are any permissions associated with this module
            var hasPermissions = await _context.UserPermissions.AnyAsync(p => p.ModuleId == id);
            if (hasPermissions)
            {
                return new ApiResponse<bool>(false, "No se puede eliminar el módulo porque tiene permisos asociados");
            }

            // Soft delete
            module.IsDeleted = true;
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Módulo eliminado exitosamente", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar módulo con ID {ModuleId}", id);
            return new ApiResponse<bool>(false, "Error interno del servidor");
        }
    }

    // User Permissions Management
    public async Task<ApiResponse<PagedResult<UserPermissionDto>>> GetUserPermissionsAsync(int userId, PaginationQuery? query = null)
    {
        try
        {
            query ??= new PaginationQuery();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<PagedResult<UserPermissionDto>>(false, "Usuario no encontrado");
            }

            var permissionsQuery = _context.UserPermissions
                .Include(p => p.Module)
                .Where(p => p.UserId == userId)
                .AsQueryable();

            // Apply sorting
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                switch (query.SortBy.ToLower())
                {
                    case "modulename":
                        permissionsQuery = query.SortDescending
                            ? permissionsQuery.OrderByDescending(p => p.Module.Name)
                            : permissionsQuery.OrderBy(p => p.Module.Name);
                        break;
                    case "modulecode":
                        permissionsQuery = query.SortDescending
                            ? permissionsQuery.OrderByDescending(p => p.Module.Code)
                            : permissionsQuery.OrderBy(p => p.Module.Code);
                        break;
                    case "permissiontype":
                        permissionsQuery = query.SortDescending
                            ? permissionsQuery.OrderByDescending(p => p.PermissionType)
                            : permissionsQuery.OrderBy(p => p.PermissionType);
                        break;
                    default:
                        permissionsQuery = permissionsQuery.OrderBy(p => p.Module.Name);
                        break;
                }
            }
            else
            {
                permissionsQuery = permissionsQuery.OrderBy(p => p.Module.Name);
            }

            var totalCount = await permissionsQuery.CountAsync();
            var pageSize = query.PageSize > 0 ? query.PageSize : 10;
            var currentPage = query.Page > 0 ? query.Page : 1;

            var permissions = await permissionsQuery
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var permissionDtos = _mapper.Map<List<UserPermissionDto>>(permissions);

            var pagedResult = PagedResult<UserPermissionDto>.Create(
                permissionDtos,
                currentPage,
                pageSize,
                totalCount
            );

            return new ApiResponse<PagedResult<UserPermissionDto>>(true, "Permisos obtenidos exitosamente", pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener permisos del usuario con ID {UserId}", userId);
            return new ApiResponse<PagedResult<UserPermissionDto>>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<UserPermissionDto>> AssignPermissionAsync(CreateUserPermissionDto createDto)
    {
        try
        {
            // Check if user exists
            var user = await _context.Users.FindAsync(createDto.UserId);
            if (user == null)
            {
                return new ApiResponse<UserPermissionDto>(false, "Usuario no encontrado");
            }

            // Check if module exists
            var module = await _context.Modules.FindAsync(createDto.ModuleId);
            if (module == null)
            {
                return new ApiResponse<UserPermissionDto>(false, "Módulo no encontrado");
            }

            // Check if permission already exists
            var existingPermission = await _context.UserPermissions
                .FirstOrDefaultAsync(p => p.UserId == createDto.UserId && p.ModuleId == createDto.ModuleId);

            if (existingPermission != null)
            {
                return new ApiResponse<UserPermissionDto>(false, "El usuario ya tiene un permiso asignado para este módulo");
            }

            var permission = new UserPermission
            {
                UserId = createDto.UserId,
                ModuleId = createDto.ModuleId,
                PermissionType = createDto.PermissionType,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserPermissions.Add(permission);
            await _context.SaveChangesAsync();

            // Load the module for the response
            await _context.Entry(permission)
                .Reference(p => p.Module)
                .LoadAsync();

            var permissionDto = _mapper.Map<UserPermissionDto>(permission);
            return new ApiResponse<UserPermissionDto>(true, "Permiso asignado exitosamente", permissionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar permiso");
            return new ApiResponse<UserPermissionDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<UserPermissionDto>> UpdatePermissionAsync(int permissionId, UpdateUserPermissionDto updateDto)
    {
        try
        {
            var permission = await _context.UserPermissions
                .Include(p => p.Module)
                .FirstOrDefaultAsync(p => p.Id == permissionId);

            if (permission == null)
            {
                return new ApiResponse<UserPermissionDto>(false, "Permiso no encontrado");
            }

            permission.PermissionType = updateDto.PermissionType;
            permission.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var permissionDto = _mapper.Map<UserPermissionDto>(permission);
            return new ApiResponse<UserPermissionDto>(true, "Permiso actualizado exitosamente", permissionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar permiso con ID {PermissionId}", permissionId);
            return new ApiResponse<UserPermissionDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<bool>> RemovePermissionAsync(int permissionId)
    {
        try
        {
            var permission = await _context.UserPermissions.FindAsync(permissionId);
            if (permission == null)
            {
                return new ApiResponse<bool>(false, "Permiso no encontrado");
            }

            _context.UserPermissions.Remove(permission);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Permiso eliminado exitosamente", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar permiso con ID {PermissionId}", permissionId);
            return new ApiResponse<bool>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<bool>> RemoveAllUserPermissionsAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<bool>(false, "Usuario no encontrado");
            }

            var permissions = await _context.UserPermissions
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (!permissions.Any())
            {
                return new ApiResponse<bool>(true, "El usuario no tiene permisos asignados", true);
            }

            _context.UserPermissions.RemoveRange(permissions);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Todos los permisos del usuario eliminados exitosamente", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar todos los permisos del usuario con ID {UserId}", userId);
            return new ApiResponse<bool>(false, "Error interno del servidor");
        }
    }

    // Permission Queries
    public async Task<ApiResponse<List<UserPermissionDto>>> GetPermissionsByUserIdAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<List<UserPermissionDto>>(false, "Usuario no encontrado");
            }

            var permissions = await _context.UserPermissions
                .Include(p => p.Module)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var permissionDtos = _mapper.Map<List<UserPermissionDto>>(permissions);
            return new ApiResponse<List<UserPermissionDto>>(true, "Permisos obtenidos exitosamente", permissionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener permisos del usuario con ID {UserId}", userId);
            return new ApiResponse<List<UserPermissionDto>>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<List<UserPermissionDto>>> GetPermissionsByModuleIdAsync(int moduleId)
    {
        try
        {
            var module = await _context.Modules.FindAsync(moduleId);
            if (module == null)
            {
                return new ApiResponse<List<UserPermissionDto>>(false, "Módulo no encontrado");
            }

            var permissions = await _context.UserPermissions
                .Include(p => p.Module)
                .Where(p => p.ModuleId == moduleId)
                .ToListAsync();

            var permissionDtos = _mapper.Map<List<UserPermissionDto>>(permissions);
            return new ApiResponse<List<UserPermissionDto>>(true, "Permisos obtenidos exitosamente", permissionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener permisos del módulo con ID {ModuleId}", moduleId);
            return new ApiResponse<List<UserPermissionDto>>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<bool>> HasPermissionAsync(int userId, string moduleCode, PermissionType requiredPermission)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
                
            if (user == null)
            {
                return new ApiResponse<bool>(false, "Usuario no encontrado");
            }

            // Admin role has all permissions
            if (user.Role.IsSystemRole && user.Role.Name.Contains("Admin"))
            {
                return new ApiResponse<bool>(true, "El usuario tiene permisos por rol de administrador", true);
            }

            var module = await _context.Modules
                .FirstOrDefaultAsync(m => m.Code == moduleCode);
                
            if (module == null)
            {
                return new ApiResponse<bool>(false, "Módulo no encontrado");
            }

            var permission = await _context.UserPermissions
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ModuleId == module.Id);

            if (permission == null)
            {
                return new ApiResponse<bool>(false, "El usuario no tiene permisos para este módulo", false);
            }

            // Higher permission levels include lower levels (numeric comparison)
            // For example: Write (20) includes Read (10), Edit (30) includes Write (20) and Read (10)
            var hasPermission = (int)permission.PermissionType >= (int)requiredPermission;
            
            return new ApiResponse<bool>(
                hasPermission, 
                hasPermission 
                    ? "El usuario tiene los permisos necesarios" 
                    : "El usuario no tiene los permisos necesarios", 
                hasPermission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar permisos del usuario {UserId} para módulo {ModuleCode}", userId, moduleCode);
            return new ApiResponse<bool>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<Dictionary<string, PermissionType>>> GetUserModulePermissionsAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
                
            if (user == null)
            {
                return new ApiResponse<Dictionary<string, PermissionType>>(false, "Usuario no encontrado");
            }

            var permissions = await _context.UserPermissions
                .Include(p => p.Module)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var permissionsMap = permissions
                .ToDictionary(p => p.Module.Code, p => p.PermissionType);

            return new ApiResponse<Dictionary<string, PermissionType>>(
                true, 
                "Permisos de módulos obtenidos exitosamente", 
                permissionsMap);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener mapa de permisos de módulos para usuario {UserId}", userId);
            return new ApiResponse<Dictionary<string, PermissionType>>(false, "Error interno del servidor");
        }
    }

    // Bulk Operations
    public async Task<ApiResponse<List<UserPermissionDto>>> AssignMultiplePermissionsAsync(int userId, List<CreateUserPermissionDto> permissions)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<List<UserPermissionDto>>(false, "Usuario no encontrado");
            }

            var createdPermissions = new List<UserPermission>();

            foreach (var permissionDto in permissions)
            {
                // Check if module exists
                var module = await _context.Modules.FindAsync(permissionDto.ModuleId);
                if (module == null)
                {
                    continue; // Skip this permission if module doesn't exist
                }

                // Check if permission already exists
                var existingPermission = await _context.UserPermissions
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.ModuleId == permissionDto.ModuleId);

                if (existingPermission != null)
                {
                    // Update existing permission
                    existingPermission.PermissionType = permissionDto.PermissionType;
                    existingPermission.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new permission
                    var newPermission = new UserPermission
                    {
                        UserId = userId,
                        ModuleId = permissionDto.ModuleId,
                        PermissionType = permissionDto.PermissionType,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.UserPermissions.Add(newPermission);
                    createdPermissions.Add(newPermission);
                }
            }

            await _context.SaveChangesAsync();

            // Load modules for created permissions
            foreach (var permission in createdPermissions)
            {
                await _context.Entry(permission)
                    .Reference(p => p.Module)
                    .LoadAsync();
            }

            var result = _mapper.Map<List<UserPermissionDto>>(createdPermissions);
            return new ApiResponse<List<UserPermissionDto>>(true, "Permisos asignados exitosamente", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar múltiples permisos al usuario con ID {UserId}", userId);
            return new ApiResponse<List<UserPermissionDto>>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<bool>> UpdateUserPermissionsAsync(int userId, List<UpdateUserPermissionDto> permissions)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<bool>(false, "Usuario no encontrado");
            }

            foreach (var permissionDto in permissions)
            {
                if (permissionDto.Id == 0)
                {
                    // Buscar si ya existe un permiso para ese usuario y módulo
                    var existing = await _context.UserPermissions.FirstOrDefaultAsync(p => p.UserId == userId && p.ModuleId == permissionDto.ModuleId);
                    if (existing == null)
                    {
                        var newPermission = new UserPermission
                        {
                            UserId = userId,
                            ModuleId = permissionDto.ModuleId,
                            PermissionType = permissionDto.PermissionType,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.UserPermissions.Add(newPermission);
                    }
                    else
                    {
                        // Si ya existe, actualiza el tipo de permiso
                        existing.PermissionType = permissionDto.PermissionType;
                        existing.UpdatedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    var permission = await _context.UserPermissions
                        .FirstOrDefaultAsync(p => p.Id == permissionDto.Id && p.UserId == userId);
                    if (permission != null)
                    {
                        permission.PermissionType = permissionDto.PermissionType;
                        permission.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Permisos actualizados exitosamente", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar permisos del usuario con ID {UserId}", userId);
            return new ApiResponse<bool>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<bool>> RemoveUserPermissionAsync(int userId, int moduleId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<bool>(false, "Usuario no encontrado");
            }

            var module = await _context.Modules.FindAsync(moduleId);
            if (module == null)
            {
                return new ApiResponse<bool>(false, "Módulo no encontrado");
            }

            var permission = await _context.UserPermissions
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ModuleId == moduleId);
                
            if (permission == null)
            {
                return new ApiResponse<bool>(false, "El permiso no existe para este usuario y módulo");
            }

            _context.UserPermissions.Remove(permission);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Permiso eliminado exitosamente", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar permiso del usuario {UserId} para el módulo {ModuleId}", userId, moduleId);
            return new ApiResponse<bool>(false, "Error interno del servidor");
        }
    }
}
