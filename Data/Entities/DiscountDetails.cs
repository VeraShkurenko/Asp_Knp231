using System.ComponentModel.DataAnnotations.Schema;

namespace AspKnP231.Data.Entities
{
    public class DiscountDetail
    {
        public Guid Id { get; set; }

        public Guid DiscountId { get; set; }

        public Guid ProductId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        public Discount Discount { get; set; } = null!;
        public ShopProduct Product { get; set; } = null!;
    }
}