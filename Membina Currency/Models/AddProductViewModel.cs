using Membina_Currency.Models.Domain;

namespace Membina_Currency.Models
{
    public class AddProductViewModel
    {
        public string Category { get; set; }
        public string BrandName { get; set; }
        public string ProductSeriesName { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImageURL { get; set; }
        public double SgdPrice { get; set; }

    }
}
