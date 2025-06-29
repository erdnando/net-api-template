using netapi_template.Models;

namespace netapi_template.Repositories.Interfaces;

public interface ICatalogRepository : IRepository<Catalog>
{
    Task<IEnumerable<Catalog>> GetByCategoryAsync(string category);
    Task<IEnumerable<Catalog>> GetInStockItemsAsync();
    Task<IEnumerable<Catalog>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<Catalog>> GetByRatingAsync(double minRating);
}
