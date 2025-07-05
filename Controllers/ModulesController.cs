using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.DTOs;
using netapi_template.Models;

namespace netapi_template.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModulesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ModulesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /api/modules
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModuleResponseDto>>> GetModules()
    {
        var modules = await _context.Modules
            .Where(m => !m.IsDeleted && m.IsActive)
            .OrderBy(m => m.Order)
            .Select(m => new ModuleResponseDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Path = m.Path,
                Icon = m.Icon,
                Order = m.Order,
                Code = m.Code
            })
            .ToListAsync();
        return Ok(modules);
    }

    // POST: /api/modules
    [HttpPost]
    public async Task<ActionResult<ModuleResponseDto>> CreateModule([FromBody] CreateModuleDto dto)
    {
        var module = new Module
        {
            Name = dto.Name,
            Description = dto.Description,
            Path = dto.Path,
            Icon = dto.Icon,
            Order = dto.Order,
            Code = dto.Code,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        _context.Modules.Add(module);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException != null && ex.InnerException.Message.Contains("Duplicate entry") && ex.InnerException.Message.Contains("IX_Modules_Code"))
            {
                return BadRequest(new { success = false, message = "Ya existe un módulo con ese código.", data = (object?)null });
            }
            throw;
        }
        var result = new ModuleResponseDto
        {
            Id = module.Id,
            Name = module.Name,
            Description = module.Description,
            Path = module.Path,
            Icon = module.Icon,
            Order = module.Order,
            Code = module.Code
        };
        // Respuesta estándar con success/data para facilitar el parseo en scripts
        return Ok(new { success = true, message = "Módulo creado", data = result });
    }

    // PUT: /api/modules/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateModule(int id, [FromBody] UpdateModuleDto dto)
    {
        var module = await _context.Modules.FindAsync(id);
        if (module == null || module.IsDeleted)
            return NotFound();
        module.Name = dto.Name;
        module.Description = dto.Description;
        module.Path = dto.Path;
        module.Icon = dto.Icon;
        module.Order = dto.Order;
        module.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: /api/modules/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteModule(int id)
    {
        var module = await _context.Modules.FindAsync(id);
        if (module == null || module.IsDeleted)
            return NotFound();
        module.IsDeleted = true;
        module.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
