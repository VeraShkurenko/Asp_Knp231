using System.ComponentModel.DataAnnotations.Schema;

namespace AspKnP231.Data.Entities
{
    public record CartItem
    {
        public Guid Id { get; set; }

        public Guid CartId { get; set; }

        public Guid ProductId { get; set; }

        public Guid? DiscountId { get; set; }

        public DateTime? DeleteDt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public ShopProduct Product { get; set; } = null!;
    }
}