# Certificate Editor Field Mapping - a25-2016-03.pdf

## Overview
This document tracks the field name mappings between the code and the PDF form for the ACORD 25 (2016/03) certificate.

## Recent Changes (November 2024)
After PDF edits, several field names were updated. This caused `NullReferenceException` errors when the code tried to access non-existent fields.

## Field Name Corrections

### Insurer Name Fields
**OLD (Incorrect):**
- `Insurer_FullName_B[0]`
- `Insurer_FullName_C[0]`
- `Insurer_FullName_D[0]`
- `Insurer_FullName_E[0]`

**NEW (Correct):**
- `Insurer_FullName_001`
- `Insurer_FullName_003`
- `Insurer_FullName_005`
- `Insurer_FullName_007`

### Additional Insured Indicator Fields
**OLD (Incorrect):**
- `GeneralLiability_AdditionalInsuredIndicator`

**NEW (Correct):**
- `CertificateOfInsurance_GeneralLiability_AdditionalInsuredCode`

### Waiver of Subrogation Fields
**OLD (Incorrect):**
- `GeneralLiability_WaiverOfSubrogationIndicator`
- `WorkersCompensationAndEmployersLiability_WaiverOfSubrogationIndicator`

**NEW (Correct):**
- `Policy_GeneralLiability_SubrogationWaivedCode`
- `Policy_WorkersCompensation_SubrogationWaivedCode`

### Other Policy Coverage Fields
**OLD (Incorrect):**
- `OtherPolicy_CoverageCode_B[0]`
- `OtherPolicy_CoverageLimitAmount_B[0]`

**NEW (Correct):**
- `OtherPolicy_CoverageCode_102`
- `OtherPolicy_CoverageLimitAmount_103`

## Complete Field List (from JSON)

### Form Header
- `Form_CompletionDate`
- `CertificateOfInsurance_CertificateNumberIdentifier`
- `CertificateOfInsurance_RevisionNumberIdentifier`

### Producer Information
- `Producer_FullName`
- `Producer_MailingAddress_LineOne`
- `Producer_MailingAddress_LineTwo`
- `Producer_MailingAddress_CityName`
- `Producer_MailingAddress_StateOrProvinceCode`
- `Producer_MailingAddress_PostalCode`
- `Producer_ContactPerson_FullName`
- `Producer_ContactPerson_PhoneNumber`
- `Producer_FaxNumber`
- `Producer_ContactPerson_EmailAddress`

### Named Insured
- `NamedInsured_FullName`
- `NamedInsured_MailingAddress_LineOne`
- `NamedInsured_MailingAddress_LineTwo`
- `NamedInsured_MailingAddress_CityName`
- `NamedInsured_MailingAddress_StateOrProvinceCode`
- `NamedInsured_MailingAddress_PostalCode`

### Insurers (A-E)
- `Insurer_FullName` (Insurer A)
- `Insurer_NAICCode`
- `Insurer_FullName_001` (Insurer B)
- `Insurer_NAICCode_002`
- `Insurer_FullName_003` (Insurer C)
- `Insurer_NAICCode_004`
- `Insurer_FullName_005` (Insurer D)
- `Insurer_NAICCode_006`
- `Insurer_FullName_007` (Insurer E)
- `Insurer_NAICCode_008`
- `Insurer_FullName_009` (Insurer F)
- `Insurer_NAICCode_010`

### General Liability
- `GeneralLiability_InsurerLetterCode`
- `GeneralLiability_CoverageIndicator`
- `GeneralLiability_ClaimsMadeIndicator`
- `GeneralLiability_OccurrenceIndicator`
- `GeneralLiability_OtherCoverageIndicator`
- `GeneralLiability_OtherCoverageDescription`
- `GeneralLiability_GeneralAggregate_LimitAppliesPerPolicyIndicator`
- `GeneralLiability_GeneralAggregate_LimitAppliesPerProjectIndicator`
- `GeneralLiability_GeneralAggregate_LimitAppliesPerLocationIndicator`
- `GeneralLiability_GeneralAggregate_LimitAppliesToOtherIndicator`
- `GeneralLiability_GeneralAggregate_LimitAppliesToCode`
- `CertificateOfInsurance_GeneralLiability_AdditionalInsuredCode`
- `Policy_GeneralLiability_SubrogationWaivedCode`
- `Policy_GeneralLiability_PolicyNumberIdentifier`
- `Policy_GeneralLiability_EffectiveDate`
- `Policy_GeneralLiability_ExpirationDate`
- `GeneralLiability_EachOccurrence_LimitAmount`
- `GeneralLiability_FireDamageRentedPremises_EachOccurrenceLimitAmount`
- `GeneralLiability_MedicalExpense_EachPersonLimitAmount`
- `GeneralLiability_PersonalAndAdvertisingInjury_LimitAmount`
- `GeneralLiability_GeneralAggregate_LimitAmount`
- `GeneralLiability_ProductsAndCompletedOperations_AggregateLimitAmount`
- `GeneralLiability_OtherCoverageLimitDescription`
- `GeneralLiability_OtherCoverageLimitAmount`

### Automobile Liability
- `Vehicle_InsurerLetterCode`
- `Vehicle_AnyAutoIndicator`
- `Vehicle_AllOwnedAutosIndicator`
- `Vehicle_HiredAutosIndicator`
- `Vehicle_ScheduledAutosIndicator`
- `Vehicle_NonOwnedAutosIndicator`
- `Vehicle_OtherCoveredAutoIndicator`
- `Vehicle_OtherCoveredAutoDescription`
- `CertificateOfInsurance_AutomobileLiability_AdditionalInsuredCode`
- `Policy_AutomobileLiability_SubrogationWaivedCode`
- `Policy_AutomobileLiability_PolicyNumberIdentifier`
- `Policy_AutomobileLiability_EffectiveDate`
- `Policy_AutomobileLiability_ExpirationDate`
- `Vehicle_CombinedSingleLimit_EachAccidentAmount`
- `Vehicle_BodilyInjury_PerPersonLimitAmount`
- `Vehicle_BodilyInjury_PerAccidentLimitAmount`
- `Vehicle_PropertyDamage_PerAccidentLimitAmount`
- `Vehicle_OtherCoverage_CoverageDescription`
- `Vehicle_OtherCoverage_LimitAmount`

### Excess/Umbrella Liability
- `ExcessUmbrella_InsurerLetterCode`
- `Policy_PolicyType_UmbrellaIndicator`
- `Policy_PolicyType_ExcessIndicator`
- `ExcessUmbrella_OccurrenceIndicator`
- `ExcessUmbrella_ClaimsMadeIndicator`
- `ExcessUmbrella_DeductibleIndicator`
- `ExcessUmbrella_RetentionIndicator`
- `ExcessUmbrella_Umbrella_DeductibleOrRetentionAmount`
- `CertificateOfInsurance_ExcessLiability_AdditionalInsuredCode`
- `Policy_ExcessLiability_SubrogationWaivedCode`
- `Policy_ExcessLiability_PolicyNumberIdentifier`
- `Policy_ExcessLiability_EffectiveDate`
- `Policy_ExcessLiability_ExpirationDate`
- `ExcessUmbrella_Umbrella_EachOccurrenceAmount`
- `ExcessUmbrella_Umbrella_AggregateAmount`
- `ExcessUmbrella_OtherCoverageDescription`
- `ExcessUmbrella_OtherCoverageLimitAmount`

### Workers Compensation
- `WorkersCompensationEmployersLiability_InsurerLetterCode`
- `WorkersCompensationEmployersLiability_AnyPersonsExcludedIndicator`
- `Policy_WorkersCompensation_SubrogationWaivedCode`
- `Policy_WorkersCompensationAndEmployersLiability_PolicyNumberIdentifier`
- `Policy_WorkersCompensationAndEmployersLiability_EffectiveDate`
- `Policy_WorkersCompensationAndEmployersLiability_ExpirationDate`
- `WorkersCompensationEmployersLiability_WorkersCompensationStatutoryLimitIndicator`
- `WorkersCompensationEmployersLiability_OtherCoverageIndicator`
- `WorkersCompensationEmployersLiability_OtherCoverageDescription`
- `WorkersCompensationEmployersLiability_EmployersLiability_EachAccidentLimitAmount`
- `WorkersCompensationEmployersLiability_EmployersLiability_DiseaseEachEmployeeLimitAmount`
- `WorkersCompensationEmployersLiability_EmployersLiability_DiseasePolicyLimitAmount`

### Other Policy
- `OtherPolicy_InsurerLetterCode`
- `OtherPolicy_OtherPolicyDescription`
- `CertificateOfInsurance_OtherPolicy_AdditionalInsuredCode`
- `OtherPolicy_SubrogationWaivedCode`
- `OtherPolicy_PolicyNumberIdentifier`
- `OtherPolicy_PolicyEffectiveDate`
- `OtherPolicy_PolicyExpirationDate`
- `OtherPolicy_CoverageCode`
- `OtherPolicy_CoverageLimitAmount`
- `OtherPolicy_CoverageCode_102`
- `OtherPolicy_CoverageLimitAmount_103`
- `OtherPolicy_CoverageCode_104`
- `OtherPolicy_CoverageLimitAmount_105`

### Certificate Holder
- `CertificateHolder_FullName`
- `CertificateHolder_MailingAddress_LineOne`
- `CertificateHolder_MailingAddress_LineTwo`
- `CertificateHolder_MailingAddress_CityName`
- `CertificateHolder_MailingAddress_StateOrProvinceCode`
- `CertificateHolder_MailingAddress_PostalCode`

### Description of Operations / Remarks
- `CertificateOfLiabilityInsurance_ACORDForm_RemarkText`

### Signature
- `Producer_AuthorizedRepresentative_Signature`

## Error Prevention

### Null-Safety Checks
The code now includes comprehensive null-safety checks for:
- Policy objects (glpolicy, autopolicy, umbrellapolicy, wcpolicy, propertypolicy)
- Coverage objects (GeneralLiabilityCoverage, AutoCoverage, UmbrellaCoverage, WorkCompCoverage, PropertyCoverage)
- Carrier objects
- Client and Address objects

### Validation Method
A `ValidateJsonFields()` method has been added that:
1. Loads the PDF form's JSON template
2. Compares expected fields against actual fields
3. Logs warnings for any missing fields
4. Returns a list of missing field names

### Error Handling
- Try-catch blocks wrap the main `HandleLoadEverything()` method
- Detailed logging for null reference errors
- Field-level validation with `SetJsonField()` helper method

## Troubleshooting

### If you get NullReferenceException errors:
1. Check the application logs for warnings about missing fields
2. Compare the field names in the code against this document
3. Use the `ValidateJsonFields()` method to identify mismatches
4. Verify that the PDF form hasn't been edited with different field names

### If data isn't appearing in the PDF:
1. Check that the JSON field name matches exactly (case-sensitive)
2. Verify the policy has the required coverage object (e.g., WorkCompCoverage)
3. Check logs for warnings about null coverage objects
4. Ensure the policy's ProductId matches the expected value

## Policy Product IDs
- General Liability: ProductId = 3
- Workers Compensation: ProductId = 2
- Auto: ProductId = 4
- Umbrella: ProductId = 7
- Property: ProductId = 14





