namespace Mantis.Domain.Shared.Models
{
    public class FireSearchResultViewModel
    {
        public string DataType { get; set; } // e.g., "Client", "Carrier", "Contact", "Policy", "Address"
        public int Id { get; set; } // ID of the entity
        public string Primary { get; set; } // Primary information, such as Name or Policy Number
        public string Parent { get; set; } // Parent information, e.g., Client or Carrier Name
    }
}
