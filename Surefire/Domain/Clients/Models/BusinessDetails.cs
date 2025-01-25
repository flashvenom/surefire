using Surefire.Data;

namespace Surefire.Domain.Clients.Models
{
    public class BusinessDetails
    {
        //Database Details
        public int BusinessDetailsId { get; set; }
        public Client Client { get; set; }
        public int ClientId { get; set; }
        public string RecordType { get; set; } = "orphan";
        public DateTime DateModified { get; set; } = DateTime.UtcNow;

        //Business Basics
        public string? FEIN { get; set; }
        public LegalEntityType? LegalEntityType { get; set; } // Enum type for legal entity
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public string? BusinessIndustry { get; set; }
        public string? BusinessSpecialty { get; set; }
        public BusinessType? BusinessType { get; set; }

        //Financials
        public decimal? AnnualGrossSalesRevenueReceipts { get; set; } //General estimate
        public decimal? BusinessPersonalPropertyBPP { get; set; }
        public decimal? AnnualPayrollHazardExposure { get; set; } //General estimate
        public decimal? EstimatedGrossSales0 { get; set; } //Estimated next 12 months
        public decimal? EstimatedGrossSales1 { get; set; } //Current year
        public decimal? EstimatedGrossSales2 { get; set; } //Prior year
        public decimal? EstimatedGrossSales3 { get; set; } //2 years ago
        public decimal? EstimatedGrossSales4 { get; set; } //3 years ago

        //History
        public DateTime? DateStarted { get; set; }
        public int? YearsExperience { get; set; }
        public string? InsuranceHistory { get; set; } //No prior, 1-59 days lapse, 60+ days lapse
        public string? LapseHistory { get; set; } //No prior, 1-59 days lapse, 60+ days lapse
        public int? NumClaims { get; set; }

        //Contractors
        public LicenseType? LicenseType { get; set; }
        public string? LicenseNumber { get; set; }
        public decimal? EstimatedSubcontractingExpenses { get; set; }

        //PrimaryBuilding
        public int? BuildingLocationYearBuilt { get; set; }
        public int? BuildingLocationSquareFootage { get; set; }
        public int? BuildingLocationNumberOfStories { get; set; }
        public bool? BuildingLocationSprinklered { get; set; }
        public bool? BuildingLocationMonitoredSecurity { get; set; }

        //Employees and Subs
        public int? NumPartTimeEmployees { get; set; }
        public int? NumFullTimeEmployees { get; set; }
        public decimal? EstimatedAnnualPayroll0 { get; set; }
        public decimal? EstimatedAnnualPayroll1 { get; set; }
        public decimal? EstimatedAnnualPayroll2 { get; set; }
        public decimal? EstimatedAnnualPayroll3 { get; set; }
        public decimal? EstimatedAnnualPayroll4 { get; set; }
    }

    public enum LegalEntityType
    {
        Individual = 1,
        C_Corporation,
        S_Corporation,
        LLC,
        Nonprofit,
        Partnership,
        Trust,
        Joint_Venture,
        Other
    }

    public enum BusinessType
    {
        Construction = 1,
        Education,
        Financial,
        FoodAndBeverage,
        Hospitality,
        Legal,
        Manufacturing,
        Medical,
        Nonprofit,
        Office,
        RealEstate,
        Retail,
        Service,
        Technology,
        Wholesale,
        Other
    }

    public enum LicenseType
    {
        A_GeneralEngineering = 1,
        B_GeneralBuilding,
        C2_InsulationAndAcoustical,
        C4_BoilerHotWaterHeatingAndSteamFitting,
        C5_FramingAndRoughCarpentry,
        C6_CabinetMillworkAndFinishCarpentry,
        C7_LowVoltageSystems,
        C8_Concrete,
        C9_Drywall,
        C10_Electrical,
        C11_Elevator,
        C12_EarthworkAndPaving,
        C13_Fencing,
        C15_FlooringAndFloorCovering,
        C16_FireProtection,
        C17_Glazing,
        C20_HVAC,
        C21_BuildingMovingDemolition,
        C22_AsbestosAbatement,
        C23_OrnamentalMetal,
        C27_Landscaping,
        C28_LockAndSecurityEquipment,
        C29_Masonry,
        C31_ConstructingZoneTrafficControl,
        C32_ParkingAndHighwayImprovement,
        C33_PaintingAndDecorating,
        C34_Pipeline,
        C35_Plastering,
        C36_Plumbing,
        C38_Refrigeration,
        C39_Roofing,
        C42_SanitationSystem,
        C43_SheetMetal,
        C45_Sign,
        C46_Solar,
        C47_GeneralManufacturedHousing,
        C50_ReinforcingSteel,
        C51_StructuralSteel,
        C53_SwimmingPool,
        C54_CeramicAndMosaicTile,
        C55_WaterConditioning,
        C57_WellDrilling,
        C60_Dredging,
        C61_LimitedSpecialty,
        D03_Awnings,
        D06_ConcreteRelatedServices,
        D09_DripIrrigationSystems,
        D12_SyntheticProducts,
        D16_HardwareLocksAndSafes,
        D21_MachineryAndPumps,
        D24_MetalProducts,
        D28_DoorsGatesAndActivatingDevices,
        D29_Paperhanging,
        D30_PileDrivingAndPressureFoundations,
        D31_PolesAndPosts,
        D34_PrefabricatedEquipment,
        D35_PoolAndSpaMaintenance,
        D38_SandAndWaterBlasting,
        D39_Scaffolding,
        D40_ServiceStationEquipmentAndMaintenance,
        D41_SidingAndDecking,
        D49_TreeService,
        D50_SuspendedCeilings,
        D52_WindowCoverings,
        D53_WoodTankConstruction,
        D56_Trenching,
        D59_HydroseedSpraying,
        D62_AirAndWaterBalancing,
        D63_ConstructingOrganizingAndEstablishingOfManufacturedHomes,
        D64_NonElectricalSignInstallation,
        D65_PetroleumRefineryService,
        D66_RiggingAndCrane,
        D71_AgriculturalIrrigationSystems,
        D72_MedicalGasSystems,
        D73_CleanRoomConstruction,
        D74_CoolingTowerConstruction,
        D75_ChemicalBlendingSystems,
        D76_BlastIng,
        D77_PrecastConcrete
    }
}