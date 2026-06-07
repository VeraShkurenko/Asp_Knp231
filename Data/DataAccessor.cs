using AspKnP231.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspKnP231.Data
{
    public class DataAccessor(DataContext dataContext)
    {
        private readonly DataContext _dataContext = dataContext;
        public IEnumerable<ShopSection> AllShopSections()
        {
            return _dataContext
                .ShopSections
                .AsNoTracking()
                .Where(s => s.DeletedAt == null)
                .AsEnumerable();
        }
        public ShopSection? GetShopSectionBySlug(String slug)
        {
            return _dataContext
                .ShopSections
                .Include(s=>s.Products)
                .AsNoTracking()
                .FirstOrDefault(s => s.Slug == slug && s.DeletedAt==null);
        }
        public ShopProduct? GetShopProductBySlug(String slug)
        {
            return _dataContext
                .ShopProducts
                .AsNoTracking()
                .FirstOrDefault(s => s.Slug == slug && s.DeletedAt==null);
        }
    }
}
