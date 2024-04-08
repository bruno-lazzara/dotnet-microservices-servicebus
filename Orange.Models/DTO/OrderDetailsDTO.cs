using System.Globalization;

namespace Orange.Models.DTO
{
    public class OrderDetailsDTO
    {
        public int OrderDetailsId { get; set; }
        public int OrderHeaderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public ProductDTO? Product { get; set; }

        public string ProductPriceCurrency => ProductPrice.ToString("c", CultureInfo.CreateSpecificCulture("en-US"));
        public double TotalValue => Count * ProductPrice;
        public string TotalValueCurrency => TotalValue.ToString("c", CultureInfo.CreateSpecificCulture("en-US"));
    }
}
