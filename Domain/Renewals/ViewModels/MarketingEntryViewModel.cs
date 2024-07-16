using System;
using System.Collections.Generic;

namespace Mantis.Domain.Renewals.ViewModels
{
    public class MarketingEntryViewModel
    {
        public int Id { get; set; }
        public string InsuredName { get; set; }
        public string PolicyType { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string WholesalerName { get; set; }
        public string CarrierName { get; set; }
        public string CsrAssigned { get; set; }
        public List<TaskViewModel> Tasks { get; set; } // List of tasks associated with the marketing entry
    }
}
