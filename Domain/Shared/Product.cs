using Folient.Domain.Policies.Models;
using Folient.Domain.Shared;
using System.Net;

namespace Folient.Domain.Shared
{
    public class Product
    {
        
        public int ProductId { get; set; }
        //Line of insurance
        public string LineName { get; set; }
        public string Description { get; set; }
        public string ProductModel { get; set; }

    }
}
