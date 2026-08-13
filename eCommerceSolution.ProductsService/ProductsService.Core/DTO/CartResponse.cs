namespace ProductsService.Core.DTO
{
    public class CartResponse
    {
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public List<CartItemResponse> CartItems { get; set; } = new();
        public decimal CartTotal { get; set; }
        public int TotalItems { get; set; }
    }
}
