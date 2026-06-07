using AspKnP231.Data.Entities;

namespace AspKnP231.Models.Shop.Admin
{
    public class AdminDiscountViewModel
    {
        public List<Discount> Discounts { get; set; } = [];
        public List<DiscountDetail> DiscountDetails { get; set; } = [];

        public List<ShopProduct> Products { get; set; } = [];
    }
}