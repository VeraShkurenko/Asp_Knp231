namespace AspKnP231.Models.Api
{
    public class ShopProductModel
    {
        public String Id { get; set; } = null!;

        public String Title { get; set; } = null!;

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public String ImageUrl { get; set; } = null!;

        public double? Rating { get; set; }

        public String? Slug { get; set; }

        public int? Stock { get; set; }
    }
}
