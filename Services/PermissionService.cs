using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.DTOs;
using netapi_template.Models;
using netapi_template.Services.Interfaces;
using AutoMapper;

namespace netapi_template.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PermissionService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Module CRUD
        public async Task<ApiResponse<PagedResult<ModuleDto>>> GetAllModulesAsync(PaginationQuery query)
        {
            var modulesQuery = _context.Modules.AsQueryable();
            if (!string.IsNullOrEmpty(query.Search))
                modulesQuery = modulesQuery.Where(m => m.Name.Contains(query.Search));
            var total = await modulesQuery.CountAsync();
            var items = await modulesQuery.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync();
            var dtos = _mapper.Map<List<ModuleDto>>(items);
            var paged = PagedResult<ModuleDto>.Create(dtos, query.Page, query.PageSize, total);
            return new ApiResponse<PagedResult<ModuleDto>>(true, "Modules retrieved", paged);
        }

        public async Task<ApiResponse<ModuleDto>> GetModuleByIdAsync(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
                return new ApiResponse<ModuleDto>(false, "Module not found");
            return new ApiResponse<ModuleDto>(true, "Module found", _mapper.Map<ModuleDto>(module));
        }

        public async Task<ApiResponse<ModuleDto>> CreateModuleAsync(CreatePermissionModuleDto dto)
        {
            var module = _mapper.Map<Module>(dto);
            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
            return new ApiResponse<ModuleDto>(true, "Module created", _mapper.Map<ModuleDto>(module));
        }

        public async Task<ApiResponse<ModuleDto>> UpdateModuleAsync(int id, UpdatePermissionModuleDto dto)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
                return new ApiResponse<ModuleDto>(false, "Module not found");
            _mapper.Map(dto, module);
            await _context.SaveChangesAsync();
            return new ApiResponse<ModuleDto>(true, "Module updated", _mapper.Map<ModuleDto>(module));
        }

        public async Task<ApiResponse<bool>> DeleteModuleAsync(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
                return new ApiResponse<bool>(false, "Module not found");
            module.IsDeleted = true;
            await _context.SaveChangesAsync();
            return new ApiResponse<bool>(true, "Module deleted", true);
        }

        // User Permissions CRUD
        public async Task<ApiResponse<List<UserPermissionDto>>> GetPermissionsByUserIdAsync(int userId)
        {
            var permissions = await _context.UserPermissions
                .Include(up => up.Module)
                .Where(up => up.UserId == userId)
                .ToListAsync();
            var dtos = _mapper.Map<List<UserPermissionDto>>(permissions);
            return new ApiResponse<List<UserPermissionDto>>(true, "Permissions retrieved", dtos);
        }

        public async Task<ApiResponse<bool>> UpdateUserPermissionsAsync(int userId, List<UpdateUserPermissionDto> permissions)
        {
            // Obtener todos los permisos actuales del usuario
            var userPermissions = await _context.UserPermissions.Where(up => up.UserId == userId).ToListAsync();
            var idsEnviados = permissions.Select(p => p.Id).ToList();

            // Eliminar los permisos que ya no están en la lista enviada
            var permisosAEliminar = userPermissions.Where(up => !idsEnviados.Contains(up.Id)).ToList();
            _context.UserPermissions.RemoveRange(permisosAEliminar);

            foreach (var perm in permissions)
            {
                if (perm.Id != 0)
                {
                    // Update: buscar el permiso existente y actualizarlo
                    var entity = userPermissions.FirstOrDefault(up => up.Id == perm.Id);
                    if (entity != null)
                    {
                        entity.PermissionType = perm.PermissionType;
                        entity.ModuleId = perm.ModuleId;
                        entity.UpdatedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    // Insert: crear nuevo permiso
                    var entity = new UserPermission
                    {
                        UserId = userId,
                        ModuleId = perm.ModuleId,
                        PermissionType = perm.PermissionType,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.UserPermissions.Add(entity);
                }
            }
            await _context.SaveChangesAsync();
            return new ApiResponse<bool>(true, "Permissions updated", true);
        }

        public async Task<ApiResponse<bool>> RemoveUserPermissionAsync(int userId, int moduleId)
        {
            var perm = await _context.UserPermissions.FirstOrDefaultAsync(up => up.UserId == userId && up.ModuleId == moduleId);
            if (perm == null)
                return new ApiResponse<bool>(false, "Permission not found");
            _context.UserPermissions.Remove(perm);
            await _context.SaveChangesAsync();
            return new ApiResponse<bool>(true, "Permission removed", true);
        }

        public async Task<ApiResponse<bool>> HasPermissionAsync(int userId, string moduleCode)
        {
            var module = await _context.Modules.FirstOrDefaultAsync(m => m.Code == moduleCode);
            if (module == null)
                return new ApiResponse<bool>(false, "Module not found");
            var has = await _context.UserPermissions.AnyAsync(up => up.UserId == userId && up.ModuleId == module.Id);
            return new ApiResponse<bool>(true, "Permission check", has);
        }

        public async Task<ApiResponse<Dictionary<string, int>>> GetUserModulePermissionsAsync(int userId)
        {
            var permissions = await _context.UserPermissions
                .Include(up => up.Module)
                .Where(up => up.UserId == userId)
                .ToListAsync();
            var dict = permissions.ToDictionary(
                up => up.Module.Code.ToLower(),
                up => up.PermissionType
            );
            return new ApiResponse<Dictionary<string, int>>(true, "Module permissions retrieved successfully", dict);
        }

        public async Task<ApiResponse<UserPermissionDto>> AssignPermissionAsync(CreateUserPermissionDto dto)
        {
            var entity = _mapper.Map<UserPermission>(dto);
            _context.UserPermissions.Add(entity);
            await _context.SaveChangesAsync();
            return new ApiResponse<UserPermissionDto>(true, "Permission assigned", _mapper.Map<UserPermissionDto>(entity));
        }
    }
}
