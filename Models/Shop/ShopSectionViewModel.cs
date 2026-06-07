using AspKnP231.Data.Entities;

namespace AspKnP231.Models.Shop
{
    public class ShopSectionViewModel
    {
        public ShopSection? ShopSection { get; set; }
        public ShopSection[] ShopSections { get; set; } = null!;
    }
}
