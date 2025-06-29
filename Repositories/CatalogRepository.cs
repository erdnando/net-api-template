using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.Models;
using netapi_template.Repositories.Interfaces;

namespace netapi_template.Repositories;

public class CatalogRepository : Repository<Catalog>, ICatalogRepository
{
    public CatalogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Catalog>> GetByCategoryAsync(string category)
    {
        return await _dbSet
            .Where(c => c.Category == category)
            .OrderBy(c => c.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Catalog>> GetInStockItemsAsync()
    {
        return await _dbSet
            .Where(c => c.InStock)
            .OrderBy(c => c.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Catalog>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _dbSet
            .Where(c => c.Price >= minPrice && c.Price <= maxPrice)
            .OrderBy(c => c.Price)
            .ToListAsync();
    }

    public async Task<IEnumerable<Catalog>> GetByRatingAsync(double minRating)
    {
        return await _dbSet
            .Where(c => c.Rating >= minRating)
            .OrderByDescending(c => c.Rating)
            .ToListAsync();
    }
}
