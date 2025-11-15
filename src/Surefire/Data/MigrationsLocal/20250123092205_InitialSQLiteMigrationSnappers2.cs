using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surefire.Data.MigrationsLocal
{
    /// <inheritdoc />
    public partial class InitialSQLiteMigrationSnappers2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddressLine1 = table.Column<string>(type: "TEXT", nullable: true),
                    AddressLine2 = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.ApplicationId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PictureUrl = table.Column<string>(type: "TEXT", nullable: true),
                    LastLookupClient = table.Column<string>(type: "TEXT", nullable: true),
                    LastLookupPerson = table.Column<string>(type: "TEXT", nullable: true),
                    LastRenewalId = table.Column<int>(type: "INTEGER", nullable: true),
                    LastRenewalMonth = table.Column<int>(type: "INTEGER", nullable: true),
                    LastRenewalYear = table.Column<int>(type: "INTEGER", nullable: true),
                    LastRenewalPerson = table.Column<string>(type: "TEXT", nullable: true),
                    LastRenewalScreen = table.Column<string>(type: "TEXT", nullable: true),
                    DesktopUsername = table.Column<string>(type: "TEXT", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentGroups",
                columns: table => new
                {
                    AttachmentGroupId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentGroups", x => x.AttachmentGroupId);
                });

            migrationBuilder.CreateTable(
                name: "FireSearchResultViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataType = table.Column<string>(type: "TEXT", nullable: false),
                    Primary = table.Column<string>(type: "TEXT", nullable: true),
                    Parent = table.Column<string>(type: "TEXT", nullable: true),
                    AddressType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FireSearchResultViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    FolderId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.FolderId);
                });

            migrationBuilder.CreateTable(
                name: "OpenAIPrompt",
                columns: table => new
                {
                    OpenAIPromptId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    prompt = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    note = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenAIPrompt", x => x.OpenAIPromptId);
                });

            migrationBuilder.CreateTable(
                name: "OpenAIResponse",
                columns: table => new
                {
                    OpenAIResponseId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    promptId = table.Column<string>(type: "TEXT", nullable: true),
                    response = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenAIResponse", x => x.OpenAIResponseId);
                });

            migrationBuilder.CreateTable(
                name: "Plugins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HashId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    ShortDescription = table.Column<string>(type: "TEXT", nullable: false),
                    PluginWebsite = table.Column<string>(type: "TEXT", nullable: false),
                    DeveloperName = table.Column<string>(type: "TEXT", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: true),
                    ClientSecret = table.Column<string>(type: "TEXT", nullable: true),
                    Jwt = table.Column<string>(type: "TEXT", nullable: true),
                    TokenUri = table.Column<string>(type: "TEXT", nullable: true),
                    BaseUri = table.Column<string>(type: "TEXT", nullable: true),
                    RedirectUri = table.Column<string>(type: "TEXT", nullable: true),
                    Scope = table.Column<string>(type: "TEXT", nullable: true),
                    GrantType = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plugins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LineName = table.Column<string>(type: "TEXT", nullable: false),
                    LineNickname = table.Column<string>(type: "TEXT", nullable: false),
                    LineCode = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "TaskMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Important = table.Column<bool>(type: "INTEGER", nullable: false),
                    ParentTaskId = table.Column<int>(type: "INTEGER", nullable: true),
                    TaskName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DaysBeforeExpiration = table.Column<int>(type: "INTEGER", nullable: true),
                    ForType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskMasters_TaskMasters_ParentTaskId",
                        column: x => x.ParentTaskId,
                        principalTable: "TaskMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carriers",
                columns: table => new
                {
                    CarrierId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LookupCode = table.Column<string>(type: "TEXT", nullable: false),
                    CarrierName = table.Column<string>(type: "TEXT", nullable: false),
                    CarrierNickname = table.Column<string>(type: "TEXT", nullable: true),
                    StreetAddress = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    Zip = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Website = table.Column<string>(type: "TEXT", nullable: true),
                    QuotingWebsite = table.Column<string>(type: "TEXT", nullable: true),
                    ServicingWebsite = table.Column<string>(type: "TEXT", nullable: true),
                    NewSubmissionEmail = table.Column<string>(type: "TEXT", nullable: true),
                    ServicingEmail = table.Column<string>(type: "TEXT", nullable: true),
                    LossRunsEmail = table.Column<string>(type: "TEXT", nullable: true),
                    IssuingCarrier = table.Column<bool>(type: "INTEGER", nullable: false),
                    Wholesaler = table.Column<bool>(type: "INTEGER", nullable: false),
                    QuickLink = table.Column<bool>(type: "INTEGER", nullable: false),
                    AppetiteJson = table.Column<string>(type: "TEXT", nullable: true),
                    QuotelinesJson = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    LogoFilename = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AddressId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carriers", x => x.CarrierId);
                    table.ForeignKey(
                        name: "FK_Carriers_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK_Carriers_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DailyTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TaskName = table.Column<string>(type: "TEXT", nullable: false),
                    Completed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Highlighted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AssignedToId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyTasks_AspNetUsers_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FormPdf",
                columns: table => new
                {
                    FormPdfId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Filepath = table.Column<string>(type: "TEXT", nullable: true),
                    JSONFields = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedById = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormPdf", x => x.FormPdfId);
                    table.ForeignKey(
                        name: "FK_FormPdf_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormPdf_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LogLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", nullable: true),
                    EntityId = table.Column<string>(type: "TEXT", nullable: true),
                    Exception = table.Column<string>(type: "TEXT", nullable: true),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_Logs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Credentials",
                columns: table => new
                {
                    CredentialId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    Website = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    CarrierId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credentials", x => x.CredentialId);
                    table.ForeignKey(
                        name: "FK_Credentials_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Credentials_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "CarrierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OriginalFileName = table.Column<string>(type: "TEXT", nullable: true),
                    HashedFileName = table.Column<string>(type: "TEXT", nullable: false),
                    LocalPath = table.Column<string>(type: "TEXT", nullable: false),
                    FileFormat = table.Column<string>(type: "TEXT", nullable: true),
                    FileSize = table.Column<double>(type: "REAL", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateLastOpened = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Comments = table.Column<string>(type: "TEXT", nullable: true),
                    IsClientAccessible = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true),
                    RenewalId = table.Column<int>(type: "INTEGER", nullable: true),
                    SubmissionId = table.Column<int>(type: "INTEGER", nullable: true),
                    CarrierId = table.Column<int>(type: "INTEGER", nullable: true),
                    AttachmentGroupId = table.Column<int>(type: "INTEGER", nullable: true),
                    FolderId = table.Column<int>(type: "INTEGER", nullable: true),
                    UploadedById = table.Column<string>(type: "TEXT", nullable: true),
                    IsPolicyCopy = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEndorsement = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsBinder = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsQuote = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsProposal = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClaimId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.AttachmentId);
                    table.ForeignKey(
                        name: "FK_Attachments_AspNetUsers_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attachments_AttachmentGroups_AttachmentGroupId",
                        column: x => x.AttachmentGroupId,
                        principalTable: "AttachmentGroups",
                        principalColumn: "AttachmentGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachments_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "CarrierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attachments_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutoCoverages",
                columns: table => new
                {
                    AutoCoverageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CombinedLimit = table.Column<int>(type: "INTEGER", nullable: true),
                    BodilyInjuryPerPerson = table.Column<int>(type: "INTEGER", nullable: true),
                    BodilyInjuryPerAccident = table.Column<int>(type: "INTEGER", nullable: true),
                    PropertyDamage = table.Column<int>(type: "INTEGER", nullable: true),
                    AdditionalCoverageName = table.Column<string>(type: "TEXT", nullable: true),
                    AdditionalCoverageLimit = table.Column<int>(type: "INTEGER", nullable: true),
                    ForAny = table.Column<bool>(type: "INTEGER", nullable: true),
                    ForOwned = table.Column<bool>(type: "INTEGER", nullable: true),
                    ForHired = table.Column<bool>(type: "INTEGER", nullable: true),
                    ForScheduled = table.Column<bool>(type: "INTEGER", nullable: true),
                    ForNonOwned = table.Column<bool>(type: "INTEGER", nullable: true),
                    AdditionalInsured = table.Column<bool>(type: "INTEGER", nullable: true),
                    AdditionalInsuredAttachmentAttachmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    WaiverOfSub = table.Column<bool>(type: "INTEGER", nullable: true),
                    WaiverOfSubAttachmentAttachmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    AdditionalAttachments = table.Column<bool>(type: "INTEGER", nullable: true),
                    AdditionalAttachmentsAttachmentAttachmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedById = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoCoverages", x => x.AutoCoverageId);
                    table.ForeignKey(
                        name: "FK_AutoCoverages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutoCoverages_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutoCoverages_Attachments_AdditionalAttachmentsAttachmentAttachmentId",
                        column: x => x.AdditionalAttachmentsAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                    table.ForeignKey(
                        name: "FK_AutoCoverages_Attachments_AdditionalInsuredAttachmentAttachmentId",
                        column: x => x.AdditionalInsuredAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                    table.ForeignKey(
                        name: "FK_AutoCoverages_Attachments_WaiverOfSubAttachmentAttachmentId",
                        column: x => x.WaiverOfSubAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                });

            migrationBuilder.CreateTable(
                name: "BusinessDetails",
                columns: table => new
                {
                    BusinessDetailsId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    RecordType = table.Column<string>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FEIN = table.Column<string>(type: "TEXT", nullable: true),
                    LegalEntityType = table.Column<int>(type: "INTEGER", nullable: true),
                    ShortDescription = table.Column<string>(type: "TEXT", nullable: true),
                    LongDescription = table.Column<string>(type: "TEXT", nullable: true),
                    BusinessIndustry = table.Column<string>(type: "TEXT", nullable: true),
                    BusinessSpecialty = table.Column<string>(type: "TEXT", nullable: true),
                    BusinessType = table.Column<int>(type: "INTEGER", nullable: true),
                    AnnualGrossSalesRevenueReceipts = table.Column<double>(type: "REAL", nullable: true),
                    BusinessPersonalPropertyBPP = table.Column<double>(type: "REAL", nullable: true),
                    AnnualPayrollHazardExposure = table.Column<double>(type: "REAL", nullable: true),
                    EstimatedGrossSales0 = table.Column<double>(type: "REAL", nullable: true),
                    EstimatedGrossSales1 = table.Column<double>(type: "REAL", nullable: true),
                    EstimatedGrossSales2 = table.Column<double>(type: "REAL", nullable: true),
                    EstimatedGrossSales3 = table.Column<double>(type: "REAL", nullable: true),
                    EstimatedGrossSales4 = table.Column<double>(type: "REAL", nullable: true),
                    DateStarted = table.Column<DateTime>(type: "TEXT", nullable: true),
                    YearsExperience = table.Column<int>(type: "INTEGER", nullable: true),
                    InsuranceHistory = table.Column<string>(type: "TEXT", nullable: true),
                    LapseHistory = table.Column<string>(type: "TEXT", nullable: true),
                    NumClaims = table.Column<int>(type: "INTEGER", nullable: true),
                    LicenseType = table.Column<int>(type: "INTEGER", nullable: true),
                    LicenseNumber = table.Column<string>(type: "TEXT", nullable: true),
                    EstimatedSubcontractingExpenses = table.Column<double>(type: "REAL", nullable: true),
                    BuildingLocationYearBuilt = table.Column<int>(type: "INTEGER", nullable: true),
                    BuildingLocationSquareFootage = table.Column<int>(type: "INTEGER", nullable: true),
                    BuildingLocationNumberOfStories = table.Column<int>(type: "INTEGER", nullable: true),
                    BuildingLocationSprinklered = table.Column<bool>(type: "INTEGER", nullable: true),
                    BuildingLocationMonitoredSecurity = table.Column<bool>(type: "INTEGER", nullable: true),
                    NumPartTimeEmployees = table.Column<int>(type: "INTEGER", nullable: true),
                    NumFullTimeEmployees = table.Column<int>(type: "INTEGER", nullable: true),
                    EstimatedAnnualPayroll0 = table.Column<double>(type: "REAL", nullable: true),
                    EstimatedAnnualPayroll1 = table.Column<double>(type: "REAL", nullable: true),
                    EstimatedAnnualPayroll2 = table.Column<double>(type: "REAL", nullable: true),
                    EstimatedAnnualPayroll3 = table.Column<double>(type: "REAL", nullable: true),
                    EstimatedAnnualPayroll4 = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessDetails", x => x.BusinessDetailsId);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    CertificateId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JSONData = table.Column<string>(type: "TEXT", nullable: true),
                    JSONDataTemp = table.Column<string>(type: "TEXT", nullable: true),
                    HolderName = table.Column<string>(type: "TEXT", nullable: true),
                    ProjectName = table.Column<string>(type: "TEXT", nullable: true),
                    AttachGLAI = table.Column<bool>(type: "INTEGER", nullable: true),
                    AttachGLAIfilename = table.Column<string>(type: "TEXT", nullable: true),
                    AttachGLWOS = table.Column<bool>(type: "INTEGER", nullable: true),
                    AttachGLWOSfilename = table.Column<string>(type: "TEXT", nullable: true),
                    AttachWCWOS = table.Column<bool>(type: "INTEGER", nullable: true),
                    AttachWCWOSfilename = table.Column<string>(type: "TEXT", nullable: true),
                    AttachPNC = table.Column<bool>(type: "INTEGER", nullable: true),
                    BlockAttachments = table.Column<bool>(type: "INTEGER", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedById = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.CertificateId);
                    table.ForeignKey(
                        name: "FK_Certificates_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificates_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    ClaimId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimNumber = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfLoss = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AmountPaid = table.Column<double>(type: "REAL", nullable: false),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.ClaimId);
                });

            migrationBuilder.CreateTable(
                name: "ClientNotes",
                columns: table => new
                {
                    ClientNoteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientNotes", x => x.ClientNoteId);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    eClientId = table.Column<string>(type: "TEXT", nullable: true),
                    LookupCode = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Website = table.Column<string>(type: "TEXT", nullable: true),
                    Comments = table.Column<string>(type: "TEXT", nullable: true),
                    LogoFilename = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateOpened = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AddressId = table.Column<int>(type: "INTEGER", nullable: false),
                    PrimaryContactId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProducerId = table.Column<string>(type: "TEXT", nullable: true),
                    CSRId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Clients_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clients_AspNetUsers_CSRId",
                        column: x => x.CSRId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_AspNetUsers_ProducerId",
                        column: x => x.ProducerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    MiddleName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    EmailAlternate = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Fax = table.Column<string>(type: "TEXT", nullable: true),
                    Mobile = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    HeadshotFilename = table.Column<string>(type: "TEXT", nullable: true),
                    IsStarred = table.Column<bool>(type: "INTEGER", nullable: false),
                    Underwriter = table.Column<bool>(type: "INTEGER", nullable: false),
                    Service = table.Column<bool>(type: "INTEGER", nullable: false),
                    Billing = table.Column<bool>(type: "INTEGER", nullable: false),
                    Representative = table.Column<bool>(type: "INTEGER", nullable: false),
                    Owner = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsInactive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AddressId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    CarrierId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_Contacts_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK_Contacts_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "CarrierId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Contacts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    LeadId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: true),
                    ContactName = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Website = table.Column<string>(type: "TEXT", nullable: true),
                    Operations = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    Zip = table.Column<string>(type: "TEXT", nullable: true),
                    Stage = table.Column<int>(type: "INTEGER", nullable: true),
                    Source = table.Column<int>(type: "INTEGER", nullable: true),
                    BindDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastOpened = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.LeadId);
                    table.ForeignKey(
                        name: "FK_Leads_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leads_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leads_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuildingName = table.Column<string>(type: "TEXT", nullable: false),
                    YearBuilt = table.Column<string>(type: "TEXT", nullable: false),
                    SquareFootage = table.Column<string>(type: "TEXT", nullable: false),
                    Owner = table.Column<bool>(type: "INTEGER", nullable: true),
                    Tenant = table.Column<bool>(type: "INTEGER", nullable: true),
                    NumPartTimeEmployees = table.Column<int>(type: "INTEGER", nullable: true),
                    NumFullTimeEmployees = table.Column<int>(type: "INTEGER", nullable: true),
                    GrossSales = table.Column<double>(type: "REAL", nullable: true),
                    OccupiedSquareFootage = table.Column<int>(type: "INTEGER", nullable: true),
                    BuildingTotalSquareFootage = table.Column<int>(type: "INTEGER", nullable: true),
                    NumStories = table.Column<int>(type: "INTEGER", nullable: true),
                    AddressId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_Locations_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Locations_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId");
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PolicyNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ePolicyId = table.Column<string>(type: "TEXT", nullable: true),
                    ePolicyLineId = table.Column<string>(type: "TEXT", nullable: true),
                    eType = table.Column<string>(type: "TEXT", nullable: true),
                    eTypeCode = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Premium = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ApplicationId = table.Column<int>(type: "INTEGER", nullable: true),
                    CarrierId = table.Column<int>(type: "INTEGER", nullable: true),
                    WholesalerId = table.Column<int>(type: "INTEGER", nullable: true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProducerId = table.Column<string>(type: "TEXT", nullable: true),
                    CSRId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.PolicyId);
                    table.ForeignKey(
                        name: "FK_Policies_Application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "ApplicationId");
                    table.ForeignKey(
                        name: "FK_Policies_AspNetUsers_CSRId",
                        column: x => x.CSRId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Policies_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Policies_AspNetUsers_ProducerId",
                        column: x => x.ProducerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Policies_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "CarrierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Policies_Carriers_WholesalerId",
                        column: x => x.WholesalerId,
                        principalTable: "Carriers",
                        principalColumn: "CarrierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Policies_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Policies_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormDocs",
                columns: table => new
                {
                    FormDocId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    JSONData = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FormPdfId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedById = table.Column<string>(type: "TEXT", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    LeadId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormDocs", x => x.FormDocId);
                    table.ForeignKey(
                        name: "FK_FormDocs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormDocs_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormDocs_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormDocs_FormPdf_FormPdfId",
                        column: x => x.FormPdfId,
                        principalTable: "FormPdf",
                        principalColumn: "FormPdfId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormDocs_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "LeadId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadNotes",
                columns: table => new
                {
                    LeadNoteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: false),
                    LeadId = table.Column<int>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadNotes", x => x.LeadNoteId);
                    table.ForeignKey(
                        name: "FK_LeadNotes_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "LeadId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    DriverId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LicenseNumber = table.Column<string>(type: "TEXT", nullable: false),
                    LicenseExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsPrimaryDriver = table.Column<bool>(type: "INTEGER", nullable: false),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.DriverId);
                    table.ForeignKey(
                        name: "FK_Drivers_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId");
                });

            migrationBuilder.CreateTable(
                name: "GeneralLiabilityCoverages",
                columns: table => new
                {
                    GeneralLiabilityCoverageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EachOccurrence = table.Column<int>(type: "INTEGER", nullable: true),
                    DamageToPremises = table.Column<int>(type: "INTEGER", nullable: true),
                    MedicalExpenses = table.Column<int>(type: "INTEGER", nullable: true),
                    PersonalInjury = table.Column<int>(type: "INTEGER", nullable: true),
                    GeneralAggregate = table.Column<int>(type: "INTEGER", nullable: true),
                    ProductsAggregate = table.Column<int>(type: "INTEGER", nullable: true),
                    AdditionalCoverageName = table.Column<string>(type: "TEXT", nullable: true),
                    AdditionalCoverageLimit = table.Column<int>(type: "INTEGER", nullable: true),
                    Premium = table.Column<double>(type: "REAL", nullable: true),
                    ClaimsMade = table.Column<bool>(type: "INTEGER", nullable: true),
                    Occurence = table.Column<bool>(type: "INTEGER", nullable: true),
                    AggregateAppliesPer = table.Column<int>(type: "INTEGER", nullable: true),
                    AdditionalInsured = table.Column<bool>(type: "INTEGER", nullable: true),
                    AdditionalInsuredFormNumber = table.Column<string>(type: "TEXT", nullable: true),
                    AdditionalInsuredAttachmentAttachmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    PrimaryWording = table.Column<bool>(type: "INTEGER", nullable: true),
                    WaiverOfSub = table.Column<bool>(type: "INTEGER", nullable: true),
                    WaiverOfSubAttachmentAttachmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    AdditionalAttachments = table.Column<bool>(type: "INTEGER", nullable: true),
                    AdditionalAttachmentsAttachmentAttachmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedById = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralLiabilityCoverages", x => x.GeneralLiabilityCoverageId);
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_Attachments_AdditionalAttachmentsAttachmentAttachmentId",
                        column: x => x.AdditionalAttachmentsAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_Attachments_AdditionalInsuredAttachmentAttachmentId",
                        column: x => x.AdditionalInsuredAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_Attachments_WaiverOfSubAttachmentAttachmentId",
                        column: x => x.WaiverOfSubAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Losses",
                columns: table => new
                {
                    LossId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateOccurred = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ShortDescription = table.Column<string>(type: "TEXT", nullable: true),
                    LongDescription = table.Column<string>(type: "TEXT", nullable: true),
                    DateClaimSubmitted = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AmountPaid = table.Column<double>(type: "REAL", nullable: true),
                    AmountReserved = table.Column<double>(type: "REAL", nullable: true),
                    Subgrogated = table.Column<bool>(type: "INTEGER", nullable: true),
                    Open = table.Column<bool>(type: "INTEGER", nullable: true),
                    UserModifiedId = table.Column<string>(type: "TEXT", nullable: true),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Losses", x => x.LossId);
                    table.ForeignKey(
                        name: "FK_Losses_AspNetUsers_UserModifiedId",
                        column: x => x.UserModifiedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Losses_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertyCoverage",
                columns: table => new
                {
                    PropertyCoverageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BusinessPersonalProperty = table.Column<int>(type: "INTEGER", nullable: true),
                    Equipment = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedById = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyCoverage", x => x.PropertyCoverageId);
                    table.ForeignKey(
                        name: "FK_PropertyCoverage_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyCoverage_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyCoverage_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyCoverage_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RatingBases",
                columns: table => new
                {
                    RatingBasisId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClassCode = table.Column<string>(type: "TEXT", nullable: true),
                    ClassDescription = table.Column<string>(type: "TEXT", nullable: true),
                    BaseRate = table.Column<double>(type: "REAL", nullable: true),
                    NetRate = table.Column<double>(type: "REAL", nullable: true),
                    Premium = table.Column<double>(type: "REAL", nullable: true),
                    Payroll = table.Column<double>(type: "REAL", nullable: true),
                    Basis = table.Column<string>(type: "TEXT", nullable: true),
                    Exposure = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserModifiedId = table.Column<string>(type: "TEXT", nullable: true),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: true),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingBases", x => x.RatingBasisId);
                    table.ForeignKey(
                        name: "FK_RatingBases_AspNetUsers_UserModifiedId",
                        column: x => x.UserModifiedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingBases_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingBases_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingBases_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Renewals",
                columns: table => new
                {
                    RenewalId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RenewalDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    ExpiringPolicyNumber = table.Column<string>(type: "TEXT", nullable: true),
                    ExpiringPremium = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CarrierId = table.Column<int>(type: "INTEGER", nullable: true),
                    WholesalerId = table.Column<int>(type: "INTEGER", nullable: true),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedToId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Renewals", x => x.RenewalId);
                    table.ForeignKey(
                        name: "FK_Renewals_AspNetUsers_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Renewals_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "CarrierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Renewals_Carriers_WholesalerId",
                        column: x => x.WholesalerId,
                        principalTable: "Carriers",
                        principalColumn: "CarrierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Renewals_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Renewals_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Renewals_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UmbrellaCoverage",
                columns: table => new
                {
                    UmbrellaCoverageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsUmbrella = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsExcess = table.Column<bool>(type: "INTEGER", nullable: true),
                    ClaimsMade = table.Column<bool>(type: "INTEGER", nullable: true),
                    Occurrence = table.Column<bool>(type: "INTEGER", nullable: true),
                    HasRetention = table.Column<bool>(type: "INTEGER", nullable: true),
                    HasDeductible = table.Column<bool>(type: "INTEGER", nullable: true),
                    EachOccurrence = table.Column<int>(type: "INTEGER", nullable: true),
                    GeneralAggregate = table.Column<int>(type: "INTEGER", nullable: true),
                    DeductibleRetentionAmount = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedById = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmbrellaCoverage", x => x.UmbrellaCoverageId);
                    table.ForeignKey(
                        name: "FK_UmbrellaCoverage_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UmbrellaCoverage_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UmbrellaCoverage_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UmbrellaCoverage_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    VehicleId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<string>(type: "TEXT", nullable: false),
                    Make = table.Column<string>(type: "TEXT", nullable: false),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    VIN = table.Column<string>(type: "TEXT", nullable: false),
                    LicensePlate = table.Column<string>(type: "TEXT", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CountryOfRegistration = table.Column<string>(type: "TEXT", nullable: false),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.VehicleId);
                    table.ForeignKey(
                        name: "FK_Vehicles_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId");
                });

            migrationBuilder.CreateTable(
                name: "WorkCompCoverages",
                columns: table => new
                {
                    WorkCompCoverageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EachAccident = table.Column<int>(type: "INTEGER", nullable: true),
                    DiseaseEachEmployee = table.Column<int>(type: "INTEGER", nullable: true),
                    DiseasePolicyLimit = table.Column<int>(type: "INTEGER", nullable: true),
                    OwnersOfficersExcluded = table.Column<bool>(type: "INTEGER", nullable: true),
                    PerStatute = table.Column<bool>(type: "INTEGER", nullable: true),
                    PerOther = table.Column<bool>(type: "INTEGER", nullable: true),
                    WaiverOfSub = table.Column<bool>(type: "INTEGER", nullable: true),
                    WaiverOfSubAttachmentAttachmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedById = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkCompCoverages", x => x.WorkCompCoverageId);
                    table.ForeignKey(
                        name: "FK_WorkCompCoverages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkCompCoverages_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkCompCoverages_Attachments_WaiverOfSubAttachmentAttachmentId",
                        column: x => x.WaiverOfSubAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                    table.ForeignKey(
                        name: "FK_WorkCompCoverages_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkCompCoverages_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormDocRevisions",
                columns: table => new
                {
                    FormDocRevisionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RevisionName = table.Column<string>(type: "TEXT", nullable: true),
                    JSONData = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedById = table.Column<string>(type: "TEXT", nullable: true),
                    FormDocId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormDocRevisions", x => x.FormDocRevisionId);
                    table.ForeignKey(
                        name: "FK_FormDocRevisions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormDocRevisions_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormDocRevisions_FormDocs_FormDocId",
                        column: x => x.FormDocId,
                        principalTable: "FormDocs",
                        principalColumn: "FormDocId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Settlements",
                columns: table => new
                {
                    SettlementId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BillType = table.Column<string>(type: "TEXT", nullable: true),
                    PayType = table.Column<string>(type: "TEXT", nullable: true),
                    Premium = table.Column<decimal>(type: "TEXT", nullable: true),
                    BrokerFee = table.Column<decimal>(type: "TEXT", nullable: true),
                    MinEarnedPercentage = table.Column<decimal>(type: "TEXT", nullable: true),
                    IsFinanced = table.Column<bool>(type: "INTEGER", nullable: false),
                    FinanceMonths = table.Column<int>(type: "INTEGER", nullable: true),
                    FinanceAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    FinanceChargePercent = table.Column<decimal>(type: "TEXT", nullable: true),
                    FinanceAccountNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PayDone = table.Column<bool>(type: "INTEGER", nullable: false),
                    PayAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    PayAmountNet = table.Column<decimal>(type: "TEXT", nullable: true),
                    PayFees = table.Column<decimal>(type: "TEXT", nullable: true),
                    PayDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PayNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PayDepositDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsFullPayment = table.Column<bool>(type: "INTEGER", nullable: false),
                    DownPaymentPercentage = table.Column<decimal>(type: "TEXT", nullable: true),
                    DownPaymentAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    CommissionPercentage = table.Column<decimal>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CommissionAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    FullGrandTotalPayment = table.Column<decimal>(type: "TEXT", nullable: true),
                    PayAmountNeededToBind = table.Column<decimal>(type: "TEXT", nullable: true),
                    AccountingIssueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AccountingPaidToCarrierAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    AccountingPaidOnDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AccountingPolicyNumber = table.Column<string>(type: "TEXT", nullable: true),
                    AccountingStatementNumber = table.Column<string>(type: "TEXT", nullable: true),
                    AccountingStatementDueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AccountingCarrier = table.Column<string>(type: "TEXT", nullable: true),
                    AccountingBillingCompany = table.Column<string>(type: "TEXT", nullable: true),
                    RenewalId = table.Column<int>(type: "INTEGER", nullable: true),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settlements", x => x.SettlementId);
                    table.ForeignKey(
                        name: "FK_Settlements_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Settlements_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Settlements_Renewals_RenewalId",
                        column: x => x.RenewalId,
                        principalTable: "Renewals",
                        principalColumn: "RenewalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    SubmissionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubmissionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    SubmissionStatus = table.Column<string>(type: "TEXT", nullable: true),
                    StatusInt = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Premium = table.Column<int>(type: "INTEGER", nullable: true),
                    PrimaryCarrierContactId = table.Column<int>(type: "INTEGER", nullable: true),
                    PrimaryWholesalerContactId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    CarrierId = table.Column<int>(type: "INTEGER", nullable: true),
                    WholesalerId = table.Column<int>(type: "INTEGER", nullable: true),
                    RenewalId = table.Column<int>(type: "INTEGER", nullable: true),
                    LeadId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.SubmissionId);
                    table.ForeignKey(
                        name: "FK_Submissions_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "CarrierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Submissions_Carriers_WholesalerId",
                        column: x => x.WholesalerId,
                        principalTable: "Carriers",
                        principalColumn: "CarrierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Submissions_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "LeadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Submissions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Submissions_Renewals_RenewalId",
                        column: x => x.RenewalId,
                        principalTable: "Renewals",
                        principalColumn: "RenewalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrackTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskName = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Completed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Hidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    Highlighted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RenewalId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GoalDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AssignedToId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackTasks_AspNetUsers_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackTasks_Renewals_RenewalId",
                        column: x => x.RenewalId,
                        principalTable: "Renewals",
                        principalColumn: "RenewalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SettlementItems",
                columns: table => new
                {
                    SettlementItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemCode = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: true),
                    SettlementId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettlementItems", x => x.SettlementItemId);
                    table.ForeignKey(
                        name: "FK_SettlementItems_Settlements_SettlementId",
                        column: x => x.SettlementId,
                        principalTable: "Settlements",
                        principalColumn: "SettlementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionNotes",
                columns: table => new
                {
                    SubmissionNoteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: false),
                    SubmissionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionNotes", x => x.SubmissionNoteId);
                    table.ForeignKey(
                        name: "FK_SubmissionNotes_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "SubmissionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_AttachmentGroupId",
                table: "Attachments",
                column: "AttachmentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_CarrierId",
                table: "Attachments",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ClaimId",
                table: "Attachments",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ClientId",
                table: "Attachments",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_FolderId",
                table: "Attachments",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_PolicyId",
                table: "Attachments",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_RenewalId",
                table: "Attachments",
                column: "RenewalId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_SubmissionId",
                table: "Attachments",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_UploadedById",
                table: "Attachments",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_AdditionalAttachmentsAttachmentAttachmentId",
                table: "AutoCoverages",
                column: "AdditionalAttachmentsAttachmentAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_AdditionalInsuredAttachmentAttachmentId",
                table: "AutoCoverages",
                column: "AdditionalInsuredAttachmentAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_ClientId",
                table: "AutoCoverages",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_CreatedById",
                table: "AutoCoverages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_ModifiedById",
                table: "AutoCoverages",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_PolicyId",
                table: "AutoCoverages",
                column: "PolicyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_WaiverOfSubAttachmentAttachmentId",
                table: "AutoCoverages",
                column: "WaiverOfSubAttachmentAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDetails_ClientId",
                table: "BusinessDetails",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Carriers_AddressId",
                table: "Carriers",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Carriers_CreatedById",
                table: "Carriers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_ClientId",
                table: "Certificates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_CreatedById",
                table: "Certificates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_ModifiedById",
                table: "Certificates",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_PolicyId",
                table: "Claims",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientNotes_ClientId",
                table: "ClientNotes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AddressId",
                table: "Clients",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CreatedById",
                table: "Clients",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CSRId",
                table: "Clients",
                column: "CSRId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_PrimaryContactId",
                table: "Clients",
                column: "PrimaryContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ProducerId",
                table: "Clients",
                column: "ProducerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_AddressId",
                table: "Contacts",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_CarrierId",
                table: "Contacts",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ClientId",
                table: "Contacts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Credentials_CarrierId",
                table: "Credentials",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Credentials_CreatedById",
                table: "Credentials",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTasks_AssignedToId",
                table: "DailyTasks",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_PolicyId",
                table: "Drivers",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocRevisions_CreatedById",
                table: "FormDocRevisions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocRevisions_FormDocId",
                table: "FormDocRevisions",
                column: "FormDocId");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocRevisions_ModifiedById",
                table: "FormDocRevisions",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocs_ClientId",
                table: "FormDocs",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocs_CreatedById",
                table: "FormDocs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocs_FormPdfId",
                table: "FormDocs",
                column: "FormPdfId");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocs_LeadId",
                table: "FormDocs",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocs_ModifiedById",
                table: "FormDocs",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormPdf_CreatedById",
                table: "FormPdf",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormPdf_ModifiedById",
                table: "FormPdf",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_AdditionalAttachmentsAttachmentAttachmentId",
                table: "GeneralLiabilityCoverages",
                column: "AdditionalAttachmentsAttachmentAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_AdditionalInsuredAttachmentAttachmentId",
                table: "GeneralLiabilityCoverages",
                column: "AdditionalInsuredAttachmentAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_ClientId",
                table: "GeneralLiabilityCoverages",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_CreatedById",
                table: "GeneralLiabilityCoverages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_ModifiedById",
                table: "GeneralLiabilityCoverages",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_PolicyId",
                table: "GeneralLiabilityCoverages",
                column: "PolicyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_WaiverOfSubAttachmentAttachmentId",
                table: "GeneralLiabilityCoverages",
                column: "WaiverOfSubAttachmentAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadNotes_LeadId",
                table: "LeadNotes",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_ClientId",
                table: "Leads",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_CreatedById",
                table: "Leads",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_ProductId",
                table: "Leads",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_AddressId",
                table: "Locations",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ClientId",
                table: "Locations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Losses_PolicyId",
                table: "Losses",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Losses_UserModifiedId",
                table: "Losses",
                column: "UserModifiedId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_ApplicationId",
                table: "Policies",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_CarrierId",
                table: "Policies",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_ClientId",
                table: "Policies",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_CreatedById",
                table: "Policies",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_CSRId",
                table: "Policies",
                column: "CSRId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_ProducerId",
                table: "Policies",
                column: "ProducerId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_ProductId",
                table: "Policies",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_WholesalerId",
                table: "Policies",
                column: "WholesalerId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCoverage_ClientId",
                table: "PropertyCoverage",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCoverage_CreatedById",
                table: "PropertyCoverage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCoverage_ModifiedById",
                table: "PropertyCoverage",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCoverage_PolicyId",
                table: "PropertyCoverage",
                column: "PolicyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RatingBases_LocationId",
                table: "RatingBases",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingBases_PolicyId",
                table: "RatingBases",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingBases_ProductId",
                table: "RatingBases",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingBases_UserModifiedId",
                table: "RatingBases",
                column: "UserModifiedId");

            migrationBuilder.CreateIndex(
                name: "IX_Renewals_AssignedToId",
                table: "Renewals",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Renewals_CarrierId",
                table: "Renewals",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Renewals_ClientId",
                table: "Renewals",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Renewals_PolicyId",
                table: "Renewals",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Renewals_ProductId",
                table: "Renewals",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Renewals_WholesalerId",
                table: "Renewals",
                column: "WholesalerId");

            migrationBuilder.CreateIndex(
                name: "IX_SettlementItems_SettlementId",
                table: "SettlementItems",
                column: "SettlementId");

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_CreatedById",
                table: "Settlements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_PolicyId",
                table: "Settlements",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_RenewalId",
                table: "Settlements",
                column: "RenewalId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionNotes_SubmissionId",
                table: "SubmissionNotes",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_CarrierId",
                table: "Submissions",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_LeadId",
                table: "Submissions",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ProductId",
                table: "Submissions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_RenewalId",
                table: "Submissions",
                column: "RenewalId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_WholesalerId",
                table: "Submissions",
                column: "WholesalerId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskMasters_ParentTaskId",
                table: "TaskMasters",
                column: "ParentTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackTasks_AssignedToId",
                table: "TrackTasks",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackTasks_RenewalId",
                table: "TrackTasks",
                column: "RenewalId");

            migrationBuilder.CreateIndex(
                name: "IX_UmbrellaCoverage_ClientId",
                table: "UmbrellaCoverage",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_UmbrellaCoverage_CreatedById",
                table: "UmbrellaCoverage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UmbrellaCoverage_ModifiedById",
                table: "UmbrellaCoverage",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_UmbrellaCoverage_PolicyId",
                table: "UmbrellaCoverage",
                column: "PolicyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_PolicyId",
                table: "Vehicles",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCompCoverages_ClientId",
                table: "WorkCompCoverages",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCompCoverages_CreatedById",
                table: "WorkCompCoverages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCompCoverages_ModifiedById",
                table: "WorkCompCoverages",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCompCoverages_PolicyId",
                table: "WorkCompCoverages",
                column: "PolicyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkCompCoverages_WaiverOfSubAttachmentAttachmentId",
                table: "WorkCompCoverages",
                column: "WaiverOfSubAttachmentAttachmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Claims_ClaimId",
                table: "Attachments",
                column: "ClaimId",
                principalTable: "Claims",
                principalColumn: "ClaimId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Clients_ClientId",
                table: "Attachments",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Policies_PolicyId",
                table: "Attachments",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "PolicyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Renewals_RenewalId",
                table: "Attachments",
                column: "RenewalId",
                principalTable: "Renewals",
                principalColumn: "RenewalId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Submissions_SubmissionId",
                table: "Attachments",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "SubmissionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AutoCoverages_Clients_ClientId",
                table: "AutoCoverages",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AutoCoverages_Policies_PolicyId",
                table: "AutoCoverages",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "PolicyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessDetails_Clients_ClientId",
                table: "BusinessDetails",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Clients_ClientId",
                table: "Certificates",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Policies_PolicyId",
                table: "Claims",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "PolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientNotes_Clients_ClientId",
                table: "ClientNotes",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Contacts_PrimaryContactId",
                table: "Clients",
                column: "PrimaryContactId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carriers_AspNetUsers_CreatedById",
                table: "Carriers");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AspNetUsers_CSRId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AspNetUsers_CreatedById",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AspNetUsers_ProducerId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Carriers_CarrierId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Clients_ClientId",
                table: "Contacts");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AutoCoverages");

            migrationBuilder.DropTable(
                name: "BusinessDetails");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "ClientNotes");

            migrationBuilder.DropTable(
                name: "Credentials");

            migrationBuilder.DropTable(
                name: "DailyTasks");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "FireSearchResultViewModel");

            migrationBuilder.DropTable(
                name: "FormDocRevisions");

            migrationBuilder.DropTable(
                name: "GeneralLiabilityCoverages");

            migrationBuilder.DropTable(
                name: "LeadNotes");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Losses");

            migrationBuilder.DropTable(
                name: "OpenAIPrompt");

            migrationBuilder.DropTable(
                name: "OpenAIResponse");

            migrationBuilder.DropTable(
                name: "Plugins");

            migrationBuilder.DropTable(
                name: "PropertyCoverage");

            migrationBuilder.DropTable(
                name: "RatingBases");

            migrationBuilder.DropTable(
                name: "SettlementItems");

            migrationBuilder.DropTable(
                name: "SubmissionNotes");

            migrationBuilder.DropTable(
                name: "TaskMasters");

            migrationBuilder.DropTable(
                name: "TrackTasks");

            migrationBuilder.DropTable(
                name: "UmbrellaCoverage");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "WorkCompCoverages");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "FormDocs");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Settlements");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "FormPdf");

            migrationBuilder.DropTable(
                name: "AttachmentGroups");

            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropTable(
                name: "Renewals");

            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Carriers");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
