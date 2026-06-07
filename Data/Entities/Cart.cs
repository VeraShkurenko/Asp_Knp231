using System.ComponentModel.DataAnnotations.Schema;

namespace AspKnP231.Data.Entities
{
    public record Cart
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? DiscountId { get; set; }

        public DateTime CreateDt { get; set; }

        public DateTime? DeleteDt { get; set; }

        public DateTime? OrderDt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = [];

    }
}
/*
[Cart]          [CartItems]
Id               Id
UserId           CartId
CreateDt         ProductId 
DeleteDt         Quantity
OrderDt          Price
Price            DiscountId
DiscountId       DeleteDt
 */