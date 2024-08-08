using Mantis.Domain.Policies.Models;
using Mantis.Domain.Shared;
using System.Net;

namespace Mantis.Domain.Shared
{
    public class Product
    {
        public int ProductId { get; set; }
        public string LineName { get; set; }
        public string LineNickname { get; set; }
        public string LineCode { get; set; }
        public string Description { get; set; }
        public string ProductModel { get; set; }
    }
}
