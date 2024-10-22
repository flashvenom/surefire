using Mantis.Components;
using Mantis.Components.Account;
using Mantis.Data;
using Mantis.Domain.Carriers.Services;
using Mantis.Domain.Policies.Services;
using Mantis.Domain.Clients.Services;
using Mantis.Domain.Renewals.Services;
using Mantis.Domain.Forms.Services;
using Mantis.Domain.Contacts.Services;
using Mantis.Domain.Shared.Services;
using Mantis.Domain.Shared.Helpers;
using Mantis.Domain.Users.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Syncfusion.Blazor;
using Microsoft.FluentUI.AspNetCore.Components.Components.Tooltip;
using Microsoft.Extensions.Options;
using DotNetEnv;

//Initial Variables
var builder = WebApplication.CreateBuilder(args);
Env.Load();
builder.Services.AddHttpClient();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddControllers();
bool detailedErrorsEnabled = builder.Configuration.GetValue<bool>("DetailedErrors:Enabled");

//SyncFusion Requirements
builder.Services.AddSyncfusionBlazor();
builder.Services.AddMemoryCache();
builder.Services.AddFluentUIComponents();
builder.Services.AddDataGridEntityFrameworkAdapter();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NCaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXdceXVXRGNeWEJ2WEQ=");

//Identity and Authorization
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddIdentityCookies();
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();


//Database Context
string connectionString = Environment.GetEnvironmentVariable("DEFAULTCONNECTION");
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddQuickGridEntityFrameworkAdapter();


//Surefire Services
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<CarrierService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RenewalService>();
builder.Services.AddScoped<PolicyService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<SharedService>();
builder.Services.AddScoped<FormService>();
builder.Services.AddScoped<AttachmentService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<ITooltipService, TooltipService>();
builder.Services.AddScoped<NavigationService>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddHttpContextAccessor();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                      .AddEnvironmentVariables();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<DataHelper>();

//Surefire API Services
builder.Services.AddHttpClient("CertificateApi", client =>
{
    client.BaseAddress = new Uri("https://flashvenomdesign-001-site6.atempurl.com/"); // Base address of your Blazor app
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


//RingCentral API Service ------------------------------//
//builder.Services.AddSignalR();
//builder.Services.AddScoped<CallAlertService>();
//builder.Services.Configure<RingCentralOptions>(builder.Configuration.GetSection("RingCentralApi"));
//builder.Services.AddHttpClient<RingCentralService>();
//builder.Services.AddScoped<HubConnection>(sp =>
//{
//    var navigationManager = sp.GetRequiredService<NavigationManager>();
//    return new HubConnectionBuilder()
//        .WithUrl(navigationManager.ToAbsoluteUri("/callAlertHub"))
//        .WithAutomaticReconnect()
//        .Build();
//});

//CrmApiService
builder.Services.Configure<CrmApiOptions>(options =>
{
    options.ClientId = Environment.GetEnvironmentVariable("CRM_API_CLIENT_ID");
    options.ClientSecret = Environment.GetEnvironmentVariable("CRM_API_CLIENT_SECRET");
    options.BaseAddress = Environment.GetEnvironmentVariable("CRM_API_BASE_ADDRESS");
    options.TokenAddress = Environment.GetEnvironmentVariable("CRM_API_TOKEN_ADDRESS");
    options.ClientAddress = Environment.GetEnvironmentVariable("CRM_API_CLIENT_ADDRESS");
    options.PoliciesAddress = Environment.GetEnvironmentVariable("CRM_API_POLICIES_ADDRESS");
    options.PolicyClientsAddress = Environment.GetEnvironmentVariable("CRM_API_POLICY_CLIENTS_ADDRESS");
});
builder.Services.AddHttpClient<CrmApiService>((sp, client) =>
{
    var crmApiOptions = sp.GetRequiredService<IOptions<CrmApiOptions>>().Value;
    client.BaseAddress = new Uri(crmApiOptions.BaseAddress ?? "https://defaulturl.com/");
}).ConfigureHttpClient((sp, client) =>
{
    var crmApiOptions = sp.GetRequiredService<IOptions<CrmApiOptions>>().Value;
    client.DefaultRequestHeaders.Add("ClientId", crmApiOptions.ClientId);
    client.DefaultRequestHeaders.Add("ClientSecret", crmApiOptions.ClientSecret);
});


//Configure the Application
var app = builder.Build();
if (detailedErrorsEnabled)
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseMigrationsEndPoint();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapControllers();
app.MapAdditionalIdentityEndpoints();
app.Run();