using Microsoft.EntityFrameworkCore;
using Surefire.Domain.Accounting.Models;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Policies.Models;
using Surefire.Domain.Shared.Models;
using Microsoft.AspNetCore.Identity;
namespace Surefire.Data;

public partial class ApplicationDbContext
{
    partial void ConfigureColumnProperties(ModelBuilder modelBuilder)
    {
        if (Database.IsSqlite())
        {
            modelBuilder.Entity<IdentityRole>()
            .Property(r => r.ConcurrencyStamp)

            .HasColumnType("TEXT");
            // Use double instead of decimal with precision for SQLite compatibility
            modelBuilder.Entity<GeneralLiabilityCoverage>()
                .Property(g => g.Premium)
                .HasConversion<double>();

            modelBuilder.Entity<Loss>()
                .Property(l => l.AmountPaid)
                .HasConversion<double>();
            modelBuilder.Entity<Loss>()
                .Property(l => l.AmountReserved)
                .HasConversion<double>();

            modelBuilder.Entity<RatingBasis>()
                .Property(r => r.BaseRate)
                .HasConversion<double>();
            modelBuilder.Entity<RatingBasis>()
                .Property(r => r.NetRate)
                .HasConversion<double>();
            modelBuilder.Entity<RatingBasis>()
                .Property(r => r.Payroll)
                .HasConversion<double>();
            modelBuilder.Entity<RatingBasis>()
                .Property(r => r.Premium)
                .HasConversion<double>();

            modelBuilder.Entity<Location>()
                .Property(l => l.GrossSales)
                .HasConversion<double>();

            modelBuilder.Entity<Claim>()
                .Property(c => c.AmountPaid)
                .HasConversion<double>();

            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedGrossSales0)
                .HasConversion<double>();
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedGrossSales1)
                .HasConversion<double>();
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedGrossSales2)
                .HasConversion<double>();
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedGrossSales3)
                .HasConversion<double>();
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedGrossSales4)
                .HasConversion<double>();
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedAnnualPayroll0)
                .HasConversion<double>();
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedAnnualPayroll1)
                .HasConversion<double>();
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedAnnualPayroll2)
                .HasConversion<double>();
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedAnnualPayroll3)
                .HasConversion<double>();
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedAnnualPayroll4)
                .HasConversion<double>();
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedSubcontractingExpenses)
                .HasConversion<double>();
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.AnnualGrossSalesRevenueReceipts)
                .HasConversion<double>();
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.AnnualPayrollHazardExposure)
                .HasConversion<double>();
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.BusinessPersonalPropertyBPP)
                .HasConversion<double>();
        }
        else
        {
            modelBuilder.Entity<GeneralLiabilityCoverage>()
                .Property(g => g.Premium)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Loss>()
                .Property(l => l.AmountPaid)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Loss>()
                .Property(l => l.AmountReserved)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<RatingBasis>()
                .Property(r => r.BaseRate)
                .HasColumnType("decimal(7,4)");
            modelBuilder.Entity<RatingBasis>()
                .Property(r => r.NetRate)
                .HasColumnType("decimal(7,4)");
            modelBuilder.Entity<RatingBasis>()
                .Property(r => r.Payroll)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<RatingBasis>()
                .Property(r => r.Premium)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Location>()
                .Property(l => l.GrossSales)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Claim>()
                .Property(c => c.AmountPaid)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedGrossSales0)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedGrossSales1)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedGrossSales2)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedGrossSales3)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedGrossSales4)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedAnnualPayroll0)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedAnnualPayroll1)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedAnnualPayroll2)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedAnnualPayroll3)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedAnnualPayroll4)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.EstimatedSubcontractingExpenses)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.AnnualGrossSalesRevenueReceipts)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.AnnualPayrollHazardExposure)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.BusinessPersonalPropertyBPP)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BusinessDetails>()
                .Property(b => b.BusinessPersonalPropertyBPP)
                .HasColumnType("decimal(18,2)");

            // Accounting
            modelBuilder.Entity<Settlement>()
                .Property(s => s.Premium)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.FinanceAmount)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.DownPaymentAmount)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.FinanceChargePercent)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.PayAmountNeededToBind)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.PayAmountNet)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.PayFees)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.MinEarnedPercentage)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.BrokerFee)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.PayAmount)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.DownPaymentPercentage)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.CommissionPercentage)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.CommissionAmount)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.AccountingPaidToCarrierAmount)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Settlement>()
                .Property(s => s.FullGrandTotalPayment)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SettlementItem>()
                .Property(si => si.Amount)
                .HasColumnType("decimal(18,2)");
        }
    }
}
