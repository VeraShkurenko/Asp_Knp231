using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AspKnP231.Data.Entities
{
    public record ShopProduct
    {
        public Guid Id { get; set; }

        public Guid ShopSectionId { get; set; }

        public String Title { get; set; } = null!;

        public String? Description { get; set; } = null!;

        public String? Slug { get; set; } = null!;

        public String? ImageUrl { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public DateTime? DeletedAt { get; set; }


        [JsonIgnore]
        public ShopSection Section { get; set; } = null!;
    }
}
