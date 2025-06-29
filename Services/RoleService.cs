using AutoMapper;
using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.DTOs;
using netapi_template.Models;
using netapi_template.Services.Interfaces;

namespace netapi_template.Services;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleService> _logger;

    public RoleService(ApplicationDbContext context, IMapper mapper, ILogger<RoleService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<RoleDto>>> GetAllRolesAsync(PaginationQuery? query = null)
    {
        try
        {
            query ??= new PaginationQuery();
            
            var rolesQuery = _context.Roles.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(query.Search))
            {
                rolesQuery = rolesQuery.Where(r => 
                    r.Name.Contains(query.Search) ||
                    (r.Description != null && r.Description.Contains(query.Search)));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                rolesQuery = query.SortBy.ToLower() switch
                {
                    "name" => query.SortDescending ? rolesQuery.OrderByDescending(r => r.Name) : rolesQuery.OrderBy(r => r.Name),
                    "createdat" => query.SortDescending ? rolesQuery.OrderByDescending(r => r.CreatedAt) : rolesQuery.OrderBy(r => r.CreatedAt),
                    _ => rolesQuery.OrderBy(r => r.Id)
                };
            }
            else
            {
                rolesQuery = rolesQuery.OrderBy(r => r.Id);
            }

            var totalRecords = await rolesQuery.CountAsync();
            var roles = await rolesQuery
                .Skip(query.Skip)
                .Take(query.Take)
                .ToListAsync();

            var roleDtos = _mapper.Map<List<RoleDto>>(roles);
            var pagedResult = PagedResult<RoleDto>.Create(roleDtos, query.Page, query.PageSize, totalRecords);
            
            return new ApiResponse<PagedResult<RoleDto>>(true, "Roles retrieved successfully", pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles");
            return new ApiResponse<PagedResult<RoleDto>>(false, "Error retrieving roles");
        }
    }

    public async Task<ApiResponse<RoleDto>> GetRoleByIdAsync(int id)
    {
        try
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return new ApiResponse<RoleDto>(false, "Role not found");
            }

            var roleDto = _mapper.Map<RoleDto>(role);
            return new ApiResponse<RoleDto>(true, "Role found", roleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role with ID {RoleId}", id);
            return new ApiResponse<RoleDto>(false, "Error retrieving role");
        }
    }

    public async Task<ApiResponse<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto)
    {
        try
        {
            // Check if role name already exists
            if (await _context.Roles.AnyAsync(r => r.Name == createRoleDto.Name))
            {
                return new ApiResponse<RoleDto>(false, "Role name already exists");
            }

            var role = _mapper.Map<Role>(createRoleDto);
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var roleDto = _mapper.Map<RoleDto>(role);
            return new ApiResponse<RoleDto>(true, "Role created successfully", roleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return new ApiResponse<RoleDto>(false, "Error creating role");
        }
    }

    public async Task<ApiResponse<RoleDto>> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto)
    {
        try
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return new ApiResponse<RoleDto>(false, "Role not found");
            }

            // Check if it's a system role
            if (role.IsSystemRole)
            {
                return new ApiResponse<RoleDto>(false, "Cannot modify system roles");
            }

            // Check if role name already exists (excluding current role)
            if (!string.IsNullOrEmpty(updateRoleDto.Name) && 
                await _context.Roles.AnyAsync(r => r.Name == updateRoleDto.Name && r.Id != id))
            {
                return new ApiResponse<RoleDto>(false, "Role name already exists");
            }

            _mapper.Map(updateRoleDto, role);
            role.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var roleDto = _mapper.Map<RoleDto>(role);
            return new ApiResponse<RoleDto>(true, "Role updated successfully", roleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role with ID {RoleId}", id);
            return new ApiResponse<RoleDto>(false, "Error updating role");
        }
    }

    public async Task<ApiResponse<bool>> DeleteRoleAsync(int id)
    {
        try
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return new ApiResponse<bool>(false, "Role not found");
            }

            // Check if it's a system role
            if (role.IsSystemRole)
            {
                return new ApiResponse<bool>(false, "Cannot delete system roles");
            }

            // Check if there are users assigned to this role
            var usersWithRole = await _context.Users.AnyAsync(u => u.RoleId == id);
            if (usersWithRole)
            {
                return new ApiResponse<bool>(false, "Cannot delete role that has assigned users");
            }

            role.IsDeleted = true;
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Role deleted successfully", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role with ID {RoleId}", id);
            return new ApiResponse<bool>(false, "Error deleting role");
        }
    }
}
