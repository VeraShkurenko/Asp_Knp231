using AspKnP231.Data.Entities;

namespace AspKnP231.Models.Shop
{
    public class ShopProductViewModal
    {
        public ShopProduct? ShopProduct { get; set; }

        public ShopProduct[] PromoProduct { get; set; } = [];

        public ShopSection[] ShopSections { get; set; } = [];
    }
}
