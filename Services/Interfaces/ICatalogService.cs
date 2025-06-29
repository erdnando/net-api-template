using netapi_template.DTOs;

namespace netapi_template.Services.Interfaces;

public interface ICatalogService
{
    Task<ApiResponse<CatalogDto>> GetCatalogByIdAsync(int id);
    Task<ApiResponse<IEnumerable<CatalogDto>>> GetAllCatalogsAsync();
    Task<ApiResponse<IEnumerable<CatalogDto>>> GetCatalogsByTypeAsync(string type);
    Task<ApiResponse<IEnumerable<CatalogDto>>> GetByCategoryAsync(string category);
    Task<ApiResponse<IEnumerable<CatalogDto>>> GetActiveCatalogsAsync();
    Task<ApiResponse<CatalogDto>> CreateCatalogAsync(CreateCatalogDto createCatalogDto);
    Task<ApiResponse<CatalogDto>> UpdateCatalogAsync(int id, UpdateCatalogDto updateCatalogDto);
    Task<ApiResponse<bool>> DeleteCatalogAsync(int id);
    Task<ApiResponse<CatalogDto>> ToggleCatalogStatusAsync(int id, bool isActive);
    Task<ApiResponse<IEnumerable<CatalogDto>>> GetInStockItemsAsync();
}
