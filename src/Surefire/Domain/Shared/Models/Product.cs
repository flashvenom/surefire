namespace Surefire.Domain.Shared.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string LineName { get; set; }
        public string LineNickname { get; set; }
        public string LineCode { get; set; }
        public string? Description { get; set; }

        // Navigation property for carrier-product relationships
        public ICollection<Surefire.Domain.Carriers.Models.CarrierProduct> CarrierProducts { get; set; } = new List<Surefire.Domain.Carriers.Models.CarrierProduct>(); // Carriers that offer this product
    }
}