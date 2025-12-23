Reference list for insurance product types used across Policies, Renewals, Leads, and reporting. Source of truth: `src/Quickfire.Blazor/Data/SeedInitialData.cs` (`SeedProducts`).

| Id | Name | Line Code | Nickname | Notes |
| --:| --- | --- | --- | --- |
| 1 | Cyber Liability | CYBR | Cyber | Data breach, ransomware, media liability |
| 2 | Workers Compensation | WCO | Work Comp | Includes employer liability |
| 3 | General Liability | GLI | GL | Third-party BI/PD/advertising |
| 4 | Commercial Auto | AUT | Auto | Company-owned/used vehicles |
| 5 | Professional Liability | E&O | E&O | Errors and omissions |
| 6 | Business Owner's Package | BOP | BOP | Bundled property + GL |
| 7 | Commercial Umbrella | UMB | Umbrella | Excess over GL/Auto/EL |
| 8 | Employer Practices Liability | EPLI | EPLI | Employment practices |
| 9 | Group Medical | MED | Group Med | Employee health benefits |
| 10 | Other | N/A | Other | Catch-all |
| 11 | Accidental Death | ACC | AD&D | AD&D add-ons |
| 12 | Bond | BND | Bond | Surety/fidelity |
| 13 | Commercial Package | PKG | Package | Multi-line custom bundle |
| 14 | Property | PROP | Property | Buildings, contents, BI |
| 15 | Dental | DEN | Dental | Group dental |
| 16 | Inland Marine | INL | Inland | Movable property |
| 17 | Equipment Floater | FLT | Tools | Contractor equipment |
| 18 | Earthquake | EQUK | EQ | Cat coverage |
| 19 | Directors and Officers | DOLI | D&O | Executive protection |
| 20 | Life | LIFE | Life | Term/permanent life |
| 21 | Personal Homeowners | HOME | Home | Residential coverage |

## Updating the list
1. Update `SeedProducts` in `Data/SeedInitialData.cs` (Id, Code, Description).
2. Run the seed flow or migration to add new rows (IDs are ints; keep them stable).
3. Update dropdowns/templates referencing product codes:
   - `Domain/Policies/Pages/Create.razor`
   - `Domain/Policies/Pages/Edit.razor`
   - `Domain/Renewals/Pages/Edit.razor`
   - `Domain/Leads/Components/Create.razor`

Use this table when mapping coverage-specific workflows and reports.
