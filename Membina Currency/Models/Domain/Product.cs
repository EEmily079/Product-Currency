namespace Membina_Currency.Models.Domain
{
    public class Product
    {
        public Guid Id { get; set; }
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


