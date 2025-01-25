namespace Surefire.Domain.Shared.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string LineName { get; set; }
        public string LineNickname { get; set; }
        public string LineCode { get; set; }
        public string? Description { get; set; }
    }
}
