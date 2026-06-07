namespace AspKnp231.Models.Api.Cart
{
    public class CartItemFormModel
    {
        public string ProductId { get; set; } = null!;

        public int Cnt { get; set; }

        public double Price { get; set; }
    }
}