using AutoMapper;
using netapi_template.DTOs;
using netapi_template.Models;
using netapi_template.Repositories.Interfaces;
using netapi_template.Services.Interfaces;

namespace netapi_template.Services;

public class CatalogService : ICatalogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CatalogService> _logger;

    public CatalogService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CatalogService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<CatalogDto>> GetCatalogByIdAsync(int id)
    {
        try
        {
            var catalog = await _unitOfWork.Catalogs.GetByIdAsync(id);
            if (catalog == null)
            {
                return new ApiResponse<CatalogDto>(false, "Producto no encontrado");
            }

            var catalogDto = _mapper.Map<CatalogDto>(catalog);
            return new ApiResponse<CatalogDto>(true, "Producto encontrado", catalogDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto con ID {CatalogId}", id);
            return new ApiResponse<CatalogDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<IEnumerable<CatalogDto>>> GetAllCatalogsAsync()
    {
        try
        {
            var catalogs = await _unitOfWork.Catalogs.GetAllAsync();
            var catalogDtos = _mapper.Map<IEnumerable<CatalogDto>>(catalogs);
            return new ApiResponse<IEnumerable<CatalogDto>>(true, "Productos obtenidos exitosamente", catalogDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los productos");
            return new ApiResponse<IEnumerable<CatalogDto>>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<IEnumerable<CatalogDto>>> GetByCategoryAsync(string category)
    {
        try
        {
            var catalogs = await _unitOfWork.Catalogs.GetByCategoryAsync(category);
            var catalogDtos = _mapper.Map<IEnumerable<CatalogDto>>(catalogs);
            return new ApiResponse<IEnumerable<CatalogDto>>(true, "Productos por categoría obtenidos exitosamente", catalogDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos de la categoría {Category}", category);
            return new ApiResponse<IEnumerable<CatalogDto>>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<CatalogDto>> CreateCatalogAsync(CreateCatalogDto createCatalogDto)
    {
        try
        {
            var catalog = _mapper.Map<Catalog>(createCatalogDto);
            await _unitOfWork.Catalogs.AddAsync(catalog);
            await _unitOfWork.SaveChangesAsync();

            var catalogDto = _mapper.Map<CatalogDto>(catalog);
            return new ApiResponse<CatalogDto>(true, "Producto creado exitosamente", catalogDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto");
            return new ApiResponse<CatalogDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<CatalogDto>> UpdateCatalogAsync(int id, UpdateCatalogDto updateCatalogDto)
    {
        try
        {
            var catalog = await _unitOfWork.Catalogs.GetByIdAsync(id);
            if (catalog == null)
            {
                return new ApiResponse<CatalogDto>(false, "Producto no encontrado");
            }

            _mapper.Map(updateCatalogDto, catalog);
            catalog.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Catalogs.Update(catalog);
            await _unitOfWork.SaveChangesAsync();

            var catalogDto = _mapper.Map<CatalogDto>(catalog);
            return new ApiResponse<CatalogDto>(true, "Producto actualizado exitosamente", catalogDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto con ID {CatalogId}", id);
            return new ApiResponse<CatalogDto>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<bool>> DeleteCatalogAsync(int id)
    {
        try
        {
            var catalog = await _unitOfWork.Catalogs.GetByIdAsync(id);
            if (catalog == null)
            {
                return new ApiResponse<bool>(false, "Producto no encontrado");
            }

            _unitOfWork.Catalogs.Remove(catalog);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Producto eliminado exitosamente", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto con ID {CatalogId}", id);
            return new ApiResponse<bool>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<IEnumerable<CatalogDto>>> GetInStockItemsAsync()
    {
        try
        {
            var catalogs = await _unitOfWork.Catalogs.GetInStockItemsAsync();
            var catalogDtos = _mapper.Map<IEnumerable<CatalogDto>>(catalogs);
            return new ApiResponse<IEnumerable<CatalogDto>>(true, "Productos en stock obtenidos exitosamente", catalogDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos en stock");
            return new ApiResponse<IEnumerable<CatalogDto>>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<IEnumerable<CatalogDto>>> GetCatalogsByTypeAsync(string type)
    {
        try
        {
            var catalogs = await _unitOfWork.Catalogs.GetByCategoryAsync(type);
            var catalogDtos = _mapper.Map<IEnumerable<CatalogDto>>(catalogs);
            return new ApiResponse<IEnumerable<CatalogDto>>(true, "Productos por tipo obtenidos exitosamente", catalogDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos del tipo {Type}", type);
            return new ApiResponse<IEnumerable<CatalogDto>>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<IEnumerable<CatalogDto>>> GetActiveCatalogsAsync()
    {
        try
        {
            var catalogs = await _unitOfWork.Catalogs.GetInStockItemsAsync(); // For now, using InStock as active
            var catalogDtos = _mapper.Map<IEnumerable<CatalogDto>>(catalogs);
            return new ApiResponse<IEnumerable<CatalogDto>>(true, "Productos activos obtenidos exitosamente", catalogDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos activos");
            return new ApiResponse<IEnumerable<CatalogDto>>(false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<CatalogDto>> ToggleCatalogStatusAsync(int id, bool isActive)
    {
        try
        {
            var catalog = await _unitOfWork.Catalogs.GetByIdAsync(id);
            if (catalog == null)
            {
                return new ApiResponse<CatalogDto>(false, "Producto no encontrado");
            }

            catalog.InStock = isActive;
            catalog.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Catalogs.Update(catalog);
            await _unitOfWork.SaveChangesAsync();

            var catalogDto = _mapper.Map<CatalogDto>(catalog);
            return new ApiResponse<CatalogDto>(true, "Estado del producto actualizado exitosamente", catalogDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del producto con ID {CatalogId}", id);
            return new ApiResponse<CatalogDto>(false, "Error interno del servidor");
        }
    }
}
