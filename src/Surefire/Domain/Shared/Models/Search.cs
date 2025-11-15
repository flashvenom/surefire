using Newtonsoft.Json;

namespace Surefire.Domain.Shared.Models
{
    public class FireSearchResultViewModel
    {
        public string DataType { get; set; } // e.g., "Client", "Carrier", "Contact", "Policy", "Address"
        public int Id { get; set; } // ID of the entity
        public string? Primary { get; set; } // Primary information, such as Name or Policy Number
        public string? Parent { get; set; } // Parent information, e.g., Client or Carrier Name
        public string? AddressType { get; set; } // Parent information, e.g., Client or Carrier Name
    }
    public class TransactionsResponse
    {
        public List<RecentTransactions> Transactions { get; set; }
        public int TotalRecords { get; set; }
    }
    public class RecentTransactions
    {
        [JsonProperty("id")]
        public int TransactionId { get; set; }

        [JsonProperty("payer")]
        public string? Payer { get; set; }

        [JsonProperty("emailAddress")]
        public string? Email { get; set; }

        [JsonProperty("transactionType")]
        public string? Type { get; set; }

        [JsonProperty("comments")]
        public string? Comments { get; set; }

        [JsonProperty("amount")]
        public decimal? Amount { get; set; }

        [JsonProperty("fee")]
        public decimal? Fee { get; set; }

        [JsonProperty("events")]
        public List<Event> Events { get; set; }

        // Optional: Adding properties for SaleDate and SettleDate
        public DateTime? SaleDate => Events?.FirstOrDefault(e => e.EventType == "Sale")?.EventDate;
        public DateTime? SettleDate => Events?.FirstOrDefault(e => e.EventType == "Settle")?.EventDate;

        public class Event
        {
            [JsonProperty("eventDate")]
            public DateTime? EventDate { get; set; }

            [JsonProperty("eventType")]
            public string? EventType { get; set; }
        }
    }
    public class CallLogItem
    {
        public string id { get; set; }
        public Party from { get; set; }
        public Party to { get; set; }
        public string direction { get; set; } // "Inbound" or "Outbound"
        public string startTime { get; set; }
        public string duration { get; set; }
        public string extension { get; set; }

    }

    public class Party
    {
        public string phoneNumber { get; set; }
    }

    public class EnumData<T>
    {
        public T Value { get; set; }
        public string Text { get; set; }
    }
}
