using System.ComponentModel.DataAnnotations;

namespace netapi_template.DTOs;

public record CreateCatalogDto(
    [Required][StringLength(200)] string Title,
    [StringLength(1000)] string? Description,
    [Required][StringLength(100)] string Category,
    [StringLength(255)] string? Image,
    double Rating,
    decimal Price,
    bool InStock
);

public record UpdateCatalogDto(
    [StringLength(200)] string? Title,
    [StringLength(1000)] string? Description,
    [StringLength(100)] string? Category,
    [StringLength(255)] string? Image,
    double? Rating,
    decimal? Price,
    bool? InStock
);

public record CatalogDto(
    int Id,
    string Title,
    string Description,
    string Category,
    string? Image,
    double Rating,
    decimal Price,
    bool InStock,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
