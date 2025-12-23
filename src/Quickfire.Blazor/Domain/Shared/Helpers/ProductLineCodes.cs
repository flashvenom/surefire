using System;
using Quickfire.Blazor.Domain.Shared.Models;

namespace Quickfire.Blazor.Domain.Shared.Helpers
{
    public static class ProductLineCodes
    {
        public const string WorkersComp = "WCO";
        public const string GeneralLiability = "GLI";
        public const string CommercialAuto = "AUT";
        public const string BusinessOwners = "BOP";
        public const string Umbrella = "UMB";
        public const string Property = "PROP";
        public const string ProfessionalLiability = "E&O";
        public const string EmploymentPractices = "EPLI";
        public const string GroupMedical = "MED";
        public const string Other = "N/A";

        public static bool MatchesLineCode(Product? product, string lineCode)
        {
            return MatchesLineCode(product?.LineCode, lineCode);
        }

        public static bool MatchesLineCode(string? lineCodeValue, string lineCode)
        {
            return !string.IsNullOrWhiteSpace(lineCodeValue)
                && lineCodeValue.Trim().Equals(lineCode, StringComparison.OrdinalIgnoreCase);
        }
    }
}
