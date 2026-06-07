namespace AspKnp231.Models.Api.Cart
{
    public class CartFormModel
    {
        public double Price { get; set; }

        public CartItemFormModel[] CartItems { get; set; } = null!;
    }
}