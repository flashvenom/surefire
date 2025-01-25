using Surefire.Data;
using Surefire.Domain.Policies.Models;
using Surefire.Domain.Renewals.Models;
using Surefire.Domain.Shared.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Surefire.Domain.Accounting.Models
{
    public class Settlement
    {
        public int SettlementId { get; set; } // Primary key

        // Enums for BillType and PayType
        public BillType? BillType { get; set; } // Direct or Agency
        public PayType? PayType { get; set; } // Check, Online, Cash, Other

        public decimal? Premium { get; set; }
        public decimal? BrokerFee { get; set; }
        public decimal? MinEarnedPercentage { get; set; }
        public bool IsFinanced { get; set; }
        public int? FinanceMonths { get; set; }
        public decimal? FinanceAmount { get; set; }
        public decimal? FinanceChargePercent { get; set; }
        public string? FinanceAccountNumber { get; set; }

        public bool PayDone { get; set; }
        public decimal? PayAmount { get; set; }
        public decimal? PayAmountNet { get; set; }
        public decimal? PayFees { get; set; }
        public DateTime? PayDate { get; set; }
        public string? PayNumber { get; set; } // Check or confirmation number
        public DateTime? PayDepositDate { get; set; }
        public bool IsFullPayment { get; set; }
        public decimal? DownPaymentPercentage { get; set; }
        public decimal? DownPaymentAmount { get; set; }
        public decimal? CommissionPercentage { get; set; }
        public string? Notes { get; set; } // Nullable

        //Calculated Fields for the most part
        public decimal? CommissionAmount { get; set; } //New
        public decimal? FullGrandTotalPayment { get; set; } //New
        public decimal? PayAmountNeededToBind { get; set; } //New

        public DateTime? AccountingIssueDate { get; set; }
        public decimal? AccountingPaidToCarrierAmount { get; set; }
        public DateTime? AccountingPaidOnDate { get; set; }
        public string? AccountingPolicyNumber { get; set; }
        public string? AccountingStatementNumber { get; set; }
        public DateTime? AccountingStatementDueDate { get; set; }
        public string? AccountingCarrier { get; set; }
        public string? AccountingBillingCompany { get; set; }

        // Foreign keys
        public int? RenewalId { get; set; }
        public Renewal? Renewal { get; set; }
        public int? PolicyId { get; set; }
        public Policy? Policy { get; set; }

        // Audit properties
        public DateTime DateCreated { get; set; } = DateTime.UtcNow; // Default to today
        public DateTime? DateModified { get; set; }
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }

        // Navigation property for Settlement Items
        public ICollection<SettlementItem> SettlementItems { get; set; } = new List<SettlementItem>();
    }

    public class SettlementItem
    {
        public int SettlementItemId { get; set; } // Primary key

        // Settlement item properties
        public SettlmentItemCode? ItemCode { get; set; } // CFE, TFE, WBF, RBF, etc.
        public string? Description { get; set; } // Processing Fee, Policy Fee, etc.
        public decimal? Amount { get; set; }

        // Foreign key to Settlement
        public int SettlementId { get; set; }
        public Settlement Settlement { get; set; }

        [NotMapped]
        public EnumData<SettlmentItemCode>? ItemCodeData
        {
            get => ItemCode.HasValue
                    ? new EnumData<SettlmentItemCode> { Value = ItemCode.Value, Text = ItemCode.Value.ToString() }
                    : null;
            set => ItemCode = value?.Value;
        }

        [NotMapped]
        public string? ItemCodeString
        {
            get => ItemCode?.ToString();
            set => ItemCode = Enum.TryParse<SettlmentItemCode>(value, out var result) ? result : null;
        }
    }

    // Enums
    public enum BillType
    {
        Direct,
        Agency
    }

    public enum PayType
    {
        Check,
        Online,
        Cash,
        Other
    }

    public enum SettlmentItemCode
    {
        CFE,
        TFE,
        WBF,
        RBF,
        NOC
    }




    public class InvoiceDataDto
    {
        public string PolicyNumber { get; set; }
        public string Carrier { get; set; }
        public string BillingCompany { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceFrom { get; set; }
        public string DueDate { get; set; }
        public string BasePremium { get; set; }
        public List<FeeDto> Fees { get; set; }
        public string CommissionRate { get; set; }
        public string CommissionNet { get; set; }
        public string BasePremiumNetAmount { get; set; }
        public string NetDue { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string Comments { get; set; }
    }

    public class FeeDto
    {
        public string Description { get; set; }
        public string Amount { get; set; }
    }
}
