using System.Text.Json.Serialization;

namespace AspKnP231.Models.Shop.Admin
{
    public class ShopSectionFormModel
    {
        public String Title { get; set; } = null!;

        public String Description { get; set; } = null!;

        public String Slug { get; set; } = null!;

        [JsonIgnore]
        public IFormFile? ImageFile { get; set; }
        public String? ImageUrl { get; set; } = null!;

        public String Button { get; set; } = null!;
    }
}
