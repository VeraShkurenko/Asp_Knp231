using System.Text.Json.Serialization;

namespace AspKnP231.Models.Shop.Admin
{
    public class ShopProductFormModel
    {
        public Guid SectionId { get; set; }

        public String Title { get; set; } = null!;

        public String? Description { get; set; } = null!;

        public String? Slug { get; set; } = null!;

        [JsonIgnore]
        public IFormFile? ImageFile { get; set; }
        public String? ImageUrl { get; set; } = null!;

        public double Price { get; set; }

        public int Stock { get; set; }

        public String Button { get; set; } = null!;
    }
}
