namespace Orange.Models.DTO
{
    public class CartDetailsDTO
    {
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public CartHeaderDTO? CartHeader { get; set; }
        public ProductDTO? Product { get; set; }
    }
}
