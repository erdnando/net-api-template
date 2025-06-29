using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using netapi_template.DTOs;
using netapi_template.Services.Interfaces;

namespace netapi_template.Controllers;

/// <summary>
/// Controlador para gestión de catálogos
/// </summary>
[Produces("application/json")]
public class CatalogController : BaseController
{
    private readonly ICatalogService _catalogService;

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    /// <summary>
    /// Obtiene todos los catálogos
    /// </summary>
    /// <returns>Lista de catálogos</returns>
    /// <response code="200">Catálogos obtenidos exitosamente</response>
    /// <response code="400">Error en la petición</response>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CatalogDto>>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetAllCatalogs()
    {
        var response = await _catalogService.GetAllCatalogsAsync();
        return HandleResponse(response);
    }

    /// <summary>
    /// Obtiene un catálogo por su ID
    /// </summary>
    /// <param name="id">ID del catálogo</param>
    /// <returns>Catálogo encontrado</returns>
    /// <response code="200">Catálogo encontrado</response>
    /// <response code="400">Catálogo no encontrado</response>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CatalogDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetCatalogById([FromRoute] int id)
    {
        var response = await _catalogService.GetCatalogByIdAsync(id);
        return HandleResponse(response);
    }

    /// <summary>
    /// Obtiene catálogos por categoría
    /// </summary>
    /// <param name="category">Categoría de catálogo</param>
    /// <returns>Lista de catálogos de la categoría especificada</returns>
    /// <response code="200">Catálogos por categoría obtenidos exitosamente</response>
    /// <response code="400">Error en la petición</response>
    [Authorize]
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CatalogDto>>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetCatalogsByCategory([FromRoute] string category)
    {
        var response = await _catalogService.GetByCategoryAsync(category);
        return HandleResponse(response);
    }

    /// <summary>
    /// Obtiene catálogos por tipo
    /// </summary>
    /// <param name="type">Tipo de catálogo</param>
    /// <returns>Lista de catálogos del tipo especificado</returns>
    /// <response code="200">Catálogos por tipo obtenidos exitosamente</response>
    /// <response code="400">Error en la petición</response>
    [Authorize]
    [HttpGet("type/{type}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CatalogDto>>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetCatalogsByType([FromRoute] string type)
    {
        var response = await _catalogService.GetCatalogsByTypeAsync(type);
        return HandleResponse(response);
    }

    /// <summary>
    /// Obtiene catálogos activos/en stock
    /// </summary>
    /// <returns>Lista de catálogos activos</returns>
    /// <response code="200">Catálogos activos obtenidos exitosamente</response>
    /// <response code="400">Error en la petición</response>
    [Authorize]
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CatalogDto>>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetActiveCatalogs()
    {
        var response = await _catalogService.GetActiveCatalogsAsync();
        return HandleResponse(response);
    }

    /// <summary>
    /// Obtiene catálogos en stock
    /// </summary>
    /// <returns>Lista de catálogos en stock</returns>
    /// <response code="200">Catálogos en stock obtenidos exitosamente</response>
    /// <response code="400">Error en la petición</response>
    [Authorize]
    [HttpGet("in-stock")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CatalogDto>>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetInStockCatalogs()
    {
        var response = await _catalogService.GetInStockItemsAsync();
        return HandleResponse(response);
    }

    /// <summary>
    /// Crea un nuevo catálogo
    /// </summary>
    /// <param name="createCatalogDto">Datos del catálogo a crear</param>
    /// <returns>Catálogo creado</returns>
    /// <response code="200">Catálogo creado exitosamente</response>
    /// <response code="400">Error en los datos proporcionados</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CatalogDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateCatalog([FromBody] CreateCatalogDto createCatalogDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _catalogService.CreateCatalogAsync(createCatalogDto);
        return HandleResponse(response);
    }

    /// <summary>
    /// Actualiza un catálogo existente
    /// </summary>
    /// <param name="id">ID del catálogo a actualizar</param>
    /// <param name="updateCatalogDto">Datos a actualizar</param>
    /// <returns>Catálogo actualizado</returns>
    /// <response code="200">Catálogo actualizado exitosamente</response>
    /// <response code="400">Error en los datos o catálogo no encontrado</response>
    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CatalogDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateCatalog([FromRoute] int id, [FromBody] UpdateCatalogDto updateCatalogDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _catalogService.UpdateCatalogAsync(id, updateCatalogDto);
        return HandleResponse(response);
    }

    /// <summary>
    /// Elimina un catálogo
    /// </summary>
    /// <param name="id">ID del catálogo a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="200">Catálogo eliminado exitosamente</response>
    /// <response code="400">Catálogo no encontrado</response>
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteCatalog([FromRoute] int id)
    {
        var response = await _catalogService.DeleteCatalogAsync(id);
        return HandleResponse(response);
    }

    /// <summary>
    /// Activa/desactiva un catálogo (cambia estado de stock)
    /// </summary>
    /// <param name="id">ID del catálogo</param>
    /// <param name="isActive">Estado activo/inactivo</param>
    /// <returns>Catálogo actualizado</returns>
    /// <response code="200">Estado del catálogo actualizado exitosamente</response>
    /// <response code="400">Catálogo no encontrado</response>
    [Authorize]
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<CatalogDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ToggleCatalogStatus([FromRoute] int id, [FromBody] bool isActive)
    {
        var response = await _catalogService.ToggleCatalogStatusAsync(id, isActive);
        return HandleResponse(response);
    }
}
