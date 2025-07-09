# Guía para Desarrollo de Nuevos Módulos

## Introducción

Este documento proporciona lineamientos, reglas y mejores prácticas para desarrollar e integrar nuevos módulos en la aplicación base. La arquitectura está diseñada para soportar escalabilidad, mantenibilidad y seguridad, siguiendo un patrón de capas bien definido.

## Tabla de Contenidos

1. [Estructura General de la Aplicación](#estructura-general-de-la-aplicación)
2. [Proceso de Creación de un Nuevo Módulo](#proceso-de-creación-de-un-nuevo-módulo)
3. [Modelo de Datos](#modelo-de-datos)
4. [Repositorios](#repositorios)
5. [Servicios](#servicios)
6. [DTOs (Data Transfer Objects)](#dtos-data-transfer-objects)
7. [Controladores](#controladores)
8. [Mapeo con AutoMapper](#mapeo-con-automapper)
9. [Gestión de Permisos y Seguridad](#gestión-de-permisos-y-seguridad)
10. [Validación con FluentValidation](#validación-con-fluentvalidation)
11. [Manejo de Errores](#manejo-de-errores)
12. [Registrar el Módulo en el Sistema](#registrar-el-módulo-en-el-sistema)
13. [Lista de Verificación](#lista-de-verificación)

---

## Estructura General de la Aplicación

La aplicación sigue una arquitectura en capas:

1. **Capa de Presentación**: Controladores API
2. **Capa de Servicios**: Lógica de negocio
3. **Capa de Acceso a Datos**: Repositorios y contexto de base de datos
4. **Capa de Modelos**: Entidades y DTOs

Esta separación permite flexibilidad, mantenibilidad y pruebas unitarias efectivas.

---

## Proceso de Creación de un Nuevo Módulo

A continuación se describen los pasos para crear un nuevo módulo:

### 1. Definir el propósito y alcance del módulo

Antes de comenzar, documenta:
- Funcionalidad principal
- Entidades/modelos necesarios
- Operaciones CRUD y específicas del dominio
- Relación con otros módulos existentes

### 2. Implementar en este orden:

1. Modelo de datos
2. Repositorio e interfaz
3. DTOs
4. Servicios e interfaces
5. Mapeo con AutoMapper
6. Validaciones
7. Controlador
8. Registro en el sistema (DbContext, DI, permisos)

---

## Modelo de Datos

Los modelos representan las entidades de dominio que se persistirán en la base de datos:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using netapi_template.Models;

namespace netapi_template.Models
{
    [Table("NombrePluralDelModelo")]
    public class NuevoModelo : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Descripcion { get; set; }
        
        // Propiedades específicas del dominio
        
        // Relaciones con otros modelos (si aplica)
        public int? ModuloRelacionadoId { get; set; }
        public virtual ModuloRelacionado? ModuloRelacionado { get; set; }
    }
}
```

**Mejores Prácticas para Modelos:**
- Utilizar atributos de validación de datos
- Definir correctamente las relaciones entre entidades
- Incluir tipo de columna para fechas y decimales
- Heredar de `BaseEntity` cuando corresponda para campos comunes

---

## Repositorios

Los repositorios encapsulan el acceso a datos:

### Interfaz del Repositorio

```csharp
using netapi_template.Models;
using netapi_template.Repositories.Interfaces;

namespace netapi_template.Repositories.Interfaces
{
    public interface INuevoModeloRepository : IRepository<NuevoModelo>
    {
        // Métodos específicos para este repositorio
        Task<IEnumerable<NuevoModelo>> GetActivosAsync();
        Task<NuevoModelo?> GetCompleteByIdAsync(int id);
    }
}
```

### Implementación del Repositorio

```csharp
using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.Models;
using netapi_template.Repositories.Interfaces;

namespace netapi_template.Repositories
{
    public class NuevoModeloRepository : Repository<NuevoModelo>, INuevoModeloRepository
    {
        private readonly ApplicationDbContext _context;
        
        public NuevoModeloRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        
        // Implementación de métodos específicos
        public async Task<IEnumerable<NuevoModelo>> GetActivosAsync()
        {
            return await _context.NuevoModelo.Where(x => x.IsActive).ToListAsync();
        }
        
        public async Task<NuevoModelo?> GetCompleteByIdAsync(int id)
        {
            return await _context.NuevoModelo
                .Include(x => x.ModuloRelacionado)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
```

---

## Servicios

Los servicios contienen la lógica de negocio:

### Interfaz del Servicio

```csharp
using netapi_template.DTOs;

namespace netapi_template.Services.Interfaces
{
    public interface INuevoModeloService
    {
        Task<ApiResponse<NuevoModeloDto>> GetByIdAsync(int id);
        Task<ApiResponse<IEnumerable<NuevoModeloDto>>> GetAllAsync();
        Task<ApiResponse<NuevoModeloDto>> CreateAsync(CreateNuevoModeloDto dto);
        Task<ApiResponse<NuevoModeloDto>> UpdateAsync(int id, UpdateNuevoModeloDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        
        // Operaciones específicas del dominio
        Task<ApiResponse<IEnumerable<NuevoModeloDto>>> GetActivosAsync();
    }
}
```

### Implementación del Servicio

```csharp
using AutoMapper;
using netapi_template.DTOs;
using netapi_template.Models;
using netapi_template.Repositories.Interfaces;
using netapi_template.Services.Interfaces;

namespace netapi_template.Services
{
    public class NuevoModeloService : INuevoModeloService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<NuevoModeloService> _logger;
        
        public NuevoModeloService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<NuevoModeloService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<ApiResponse<NuevoModeloDto>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.NuevoModelo.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ApiResponse<NuevoModeloDto>(false, "Registro no encontrado");
                }
                
                var dto = _mapper.Map<NuevoModeloDto>(entity);
                return new ApiResponse<NuevoModeloDto>(true, "Registro encontrado", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro con ID {Id}", id);
                return new ApiResponse<NuevoModeloDto>(false, "Error interno del servidor");
            }
        }
        
        // Implementaciones de los demás métodos...
        
        public async Task<ApiResponse<NuevoModeloDto>> CreateAsync(CreateNuevoModeloDto dto)
        {
            try
            {
                var entity = _mapper.Map<NuevoModelo>(dto);
                var result = await _unitOfWork.NuevoModelo.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                
                var createdDto = _mapper.Map<NuevoModeloDto>(result);
                return new ApiResponse<NuevoModeloDto>(true, "Registro creado exitosamente", createdDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo registro");
                return new ApiResponse<NuevoModeloDto>(false, "Error al crear el registro");
            }
        }
    }
}
```

---

## DTOs (Data Transfer Objects)

Los DTOs definen la estructura de los datos que se envían y reciben en la API:

```csharp
using System.ComponentModel.DataAnnotations;

namespace netapi_template.DTOs
{
    // DTO para respuestas (GET)
    public record NuevoModeloDto(
        int Id,
        string Nombre,
        string Descripcion,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        bool IsActive,
        string? ModuloRelacionadoNombre
    );
    
    // DTO para creación (POST)
    public record CreateNuevoModeloDto(
        [Required][StringLength(100)] string Nombre,
        [StringLength(500)] string? Descripcion,
        int? ModuloRelacionadoId
    );
    
    // DTO para actualización (PUT/PATCH)
    public record UpdateNuevoModeloDto(
        [StringLength(100)] string? Nombre,
        [StringLength(500)] string? Descripcion,
        int? ModuloRelacionadoId,
        bool? IsActive
    );
}
```

**Mejores Prácticas para DTOs:**
- Usar records para inmutabilidad
- Incluir validaciones mediante atributos
- Separar DTOs por operación (Create, Update, Response)
- No exponer más información de la necesaria

---

## Controladores

Los controladores definen los endpoints de la API:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using netapi_template.DTOs;
using netapi_template.Services.Interfaces;
using netapi_template.Attributes;

namespace netapi_template.Controllers
{
    [Produces("application/json")]
    public class NuevoModeloController : BaseController
    {
        private readonly INuevoModeloService _service;
        
        public NuevoModeloController(INuevoModeloService service)
        {
            _service = service;
        }
        
        /// <summary>
        /// Obtiene todos los registros
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<NuevoModeloDto>>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetAll()
        {
            var response = await _service.GetAllAsync();
            return HandleResponse(response);
        }
        
        /// <summary>
        /// Obtiene un registro por su ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<NuevoModeloDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            return HandleResponse(response);
        }
        
        /// <summary>
        /// Crea un nuevo registro
        /// </summary>
        [HttpPost]
        [Authorize]
        [DisableValidation]  // Si se usa FluentValidation en vez de atributos
        [ProducesResponseType(typeof(ApiResponse<NuevoModeloDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Create([FromBody] CreateNuevoModeloDto dto)
        {
            var response = await _service.CreateAsync(dto);
            return HandleResponse(response);
        }
        
        // Implementar otros métodos (Put, Delete, etc)
    }
}
```

**Mejores Prácticas para Controladores:**
- Heredar de `BaseController` para aprovechar el manejo de respuestas estandarizadas
- Documentar endpoints con comentarios XML
- Especificar códigos de respuesta con `ProducesResponseType`
- Aplicar atributos de autorización y validación adecuados
- Mantener los controladores delgados (sin lógica de negocio)

---

## Mapeo con AutoMapper

Configure los mapeos en `Configuration/MappingProfile.cs`:

```csharp
using AutoMapper;
using netapi_template.DTOs;
using netapi_template.Models;

namespace netapi_template.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeos existentes...
            
            // Mapeos para el nuevo módulo
            CreateMap<NuevoModelo, NuevoModeloDto>()
                .ForMember(dest => dest.ModuloRelacionadoNombre, 
                    opt => opt.MapFrom(src => src.ModuloRelacionado != null ? src.ModuloRelacionado.Name : null));
                
            CreateMap<CreateNuevoModeloDto, NuevoModelo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModuloRelacionado, opt => opt.Ignore());
                
            CreateMap<UpdateNuevoModeloDto, NuevoModelo>()
                .ForMember(dest => dest.Nombre, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Nombre)))
                .ForMember(dest => dest.Descripcion, opt => opt.Condition(src => src.Descripcion != null))
                .ForMember(dest => dest.ModuloRelacionadoId, opt => opt.Condition(src => src.ModuloRelacionadoId.HasValue))
                .ForMember(dest => dest.IsActive, opt => opt.Condition(src => src.IsActive.HasValue));
        }
    }
}
```

---

## Gestión de Permisos y Seguridad

Cada módulo debe integrarse con el sistema de permisos:

### 1. Registrar el módulo en la tabla de Módulos

En una migración o en `DbInitializer`:

```csharp
// En una migración o inicializador
var nuevoModulo = new Module
{
    Name = "Nuevo Módulo",
    Code = "NUEVO_MODULO",
    Description = "Descripción del nuevo módulo",
    IsActive = true
};

modelBuilder.Entity<Module>().HasData(nuevoModulo);
```

### 2. Aplicar filtros de permisos en controladores

```csharp
[HttpGet]
[Authorize]
[RequirePermission(ModuleCode = "NUEVO_MODULO", PermissionType = PermissionType.Read)]
public async Task<IActionResult> GetAll()
{
    var response = await _service.GetAllAsync();
    return HandleResponse(response);
}

[HttpPost]
[Authorize]
[RequirePermission(ModuleCode = "NUEVO_MODULO", PermissionType = PermissionType.Write)]
public async Task<IActionResult> Create([FromBody] CreateNuevoModeloDto dto)
{
    var response = await _service.CreateAsync(dto);
    return HandleResponse(response);
}
```

---

## Validación con FluentValidation

Para validaciones complejas, utilizar FluentValidation:

```csharp
using FluentValidation;
using netapi_template.DTOs;

namespace netapi_template.Validators
{
    public class CreateNuevoModeloValidator : AbstractValidator<CreateNuevoModeloDto>
    {
        public CreateNuevoModeloValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");
                
            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");
                
            // Reglas personalizadas
            RuleFor(x => x.ModuloRelacionadoId)
                .Must(BeAValidRelatedId).When(x => x.HasValue)
                .WithMessage("El módulo relacionado no existe");
        }
        
        private bool BeAValidRelatedId(int? id)
        {
            // Validación personalizada
            return id == null || id > 0;
        }
    }
}
```

Y registrar el validador en `Program.cs`:

```csharp
builder.Services.AddScoped<IValidator<CreateNuevoModeloDto>, CreateNuevoModeloValidator>();
```

---

## Manejo de Errores

El sistema ya tiene un manejo global de excepciones. Sin embargo:

1. Utilice bloques try-catch en servicios para control granular de errores
2. Registre errores con el logger inyectado
3. Devuelva mensajes de error amigables al usuario
4. Utilice códigos HTTP adecuados para cada tipo de error

```csharp
try
{
    // Código que puede fallar
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error detallado para registro");
    return new ApiResponse<T>(false, "Mensaje amigable para el usuario");
}
```

---

## Registrar el Módulo en el Sistema

### 1. Registrar en ApplicationDbContext

```csharp
// En Data/ApplicationDbContext.cs
public DbSet<NuevoModelo> NuevoModelo { get; set; }
```

### 2. Configurar el modelo en OnModelCreating

```csharp
// En Data/ApplicationDbContext.cs - OnModelCreating
modelBuilder.Entity<NuevoModelo>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.CreatedAt).HasColumnType("datetime");
    entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
    
    // Configuración de relaciones
    entity.HasOne(e => e.ModuloRelacionado)
        .WithMany()
        .HasForeignKey(e => e.ModuloRelacionadoId)
        .OnDelete(DeleteBehavior.Restrict);
});
```

### 3. Registrar servicios y repositorios en Program.cs

```csharp
// Registrar repositorio
builder.Services.AddScoped<INuevoModeloRepository, NuevoModeloRepository>();

// Actualizar UnitOfWork
// (Asegúrate de actualizar la interfaz IUnitOfWork y su implementación)

// Registrar servicio
builder.Services.AddScoped<INuevoModeloService, NuevoModeloService>();
```

### 4. Crear migración

```bash
dotnet ef migrations add AddNuevoModulo
dotnet ef database update
```

---

## Lista de Verificación

Antes de considerar completo un módulo, verifica:

- [ ] Modelo creado y configurado en DbContext
- [ ] Repositorio e interfaz implementados
- [ ] DTOs creados para todas las operaciones
- [ ] Mapeos configurados en AutoMapper
- [ ] Servicio implementado con manejo de errores
- [ ] Validaciones aplicadas (atributos o FluentValidation)
- [ ] Controlador con endpoints documentados
- [ ] Permisos configurados en controladores
- [ ] Registros en DI Container (Program.cs)
- [ ] Migración creada y aplicada
- [ ] Pruebas unitarias implementadas (recomendado)
- [ ] Documentación en Swagger correcta

---

## Recomendaciones Finales

1. **Consistencia**: Seguir los patrones ya establecidos en la aplicación
2. **Documentación**: Comentar código no obvio y mantener la documentación Swagger actualizada
3. **Pruebas**: Implementar pruebas unitarias para servicios y repositorios
4. **Seguridad**: Validar siempre entradas y aplicar permisos adecuados
5. **Registro**: Usar el logger para eventos importantes y errores
6. **Transacciones**: Usar UnitOfWork para operaciones que afectan múltiples entidades

Con esta guía, podrás implementar nuevos módulos que se integren perfectamente con la arquitectura existente, manteniendo la coherencia y calidad del código.
