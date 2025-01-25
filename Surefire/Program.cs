using Surefire.Data;
using Surefire.Domain.Logs;
using Surefire.Domain.Ember;
using Surefire.Domain.OpenAI;
using Surefire.Domain.Plugins;
using Surefire.Domain.Shared.Models;
using Surefire.Domain.Forms.Services;
using Surefire.Domain.Users.Services;
using Surefire.Domain.Shared.Services;
using Surefire.Domain.Clients.Services;
using Surefire.Domain.Contacts.Services;
using Surefire.Domain.Carriers.Services;
using Surefire.Domain.Policies.Services;
using Surefire.Domain.Renewals.Services;
using Surefire.Domain.Accounting.Services;
using Surefire.Domain.Attachments.Services;
using Surefire.Domain.Attachments.Models;
using DotNetEnv;
using Syncfusion.Blazor;
using Surefire.Components;
using Surefire.Components.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components.Components.Tooltip;
using NuGet.Configuration;


// INITIAL VARIABLES -- -- -- -   -     -
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
//builder.Services.AddControllers();
builder.Services.AddMemoryCache();
Env.Load();
bool detailedErrorsEnabled = builder.Configuration.GetValue<bool>("DetailedErrors:Enabled");


// SYNCFUSION -- -- -- -   -     -
builder.Services.AddSyncfusionBlazor();
builder.Services.AddFluentUIComponents();
builder.Services.AddDataGridEntityFrameworkAdapter();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("[SYNCFUSIONKEY]");


// IDEN AND AUTH -- -- -- -   -     -
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
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>().AddSignInManager().AddDefaultTokenProviders();


// DATABASE  -- -- -- -   -     -
string connectionString = Environment.GetEnvironmentVariable("DEFAULTCONNECTION");
string databaseType = "SQLServer";
if (string.IsNullOrEmpty(connectionString)) {
    //connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."); 
    connectionString = $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Surefire.db")}";
    builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString)
    );
    databaseType = "SQLite";
}
else
{
    builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString, sqlOptions =>
            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
}



// OPENAI -- -- -- -   -     -
builder.Services.AddHttpClient<OpenAiService>();
string openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_KEY");
builder.Configuration["OpenAi:ApiKey"] = openAiApiKey;


// DEPENDENCIES -- -- -- -   -     -
builder.Services.AddScoped<AttachmentService>();
builder.Services.AddScoped<CarrierService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<EmberService>();
builder.Services.AddScoped<FormService>();
builder.Services.AddScoped<HomeService>();
builder.Services.AddScoped<PolicyService>();
builder.Services.AddScoped<RenewalService>();
builder.Services.AddScoped<AccountingService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<SharedService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<StateService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddScoped<ITooltipService, TooltipService>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true).AddEnvironmentVariables();

// PLUGINS -- -- -- -   -     -
builder.Services.AddScoped<PluginManager>();
#if DEBUG
var pluginsPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
#else
    var pluginsPath = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
#endif
var serviceProvider = builder.Services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILoggingService>();
PluginLoader.LoadPlugins(builder.Services, pluginsPath, serviceProvider, logger);

// ---------------------------------------------------//
// App Configuration Protocols ---------------------- //
// ---------------------------------------------------//
var app = builder.Build();
app.UseDeveloperExceptionPage();
app.UseMigrationsEndPoint();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

//Turn off for WenView2 Desktop runtime
//app.UseHttpsRedirection();

app.MapStaticAssets();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<EmberHub>("/emberHub");
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
//app.MapControllers();
app.MapAdditionalIdentityEndpoints();

// Seed data on application startup
if(databaseType == "SQLite")
{
    using (var scope = app.Services.CreateScope())
    {
        var scopedServiceProvider = scope.ServiceProvider;

        var dbContext = scopedServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            //dbContext.Database.Migrate(); // Applies any pending migrations
            //SeedData(scopedServiceProvider);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while creating or migrating the database: {ex}");
            throw;
        }
    }
}

app.Run();


void SeedData(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
    // Check if the admin user exists
    if (!userManager.Users.Any())
    {
        var adminUser = new ApplicationUser
        {
            UserName = "demo@surefire.com",
            FirstName = "John",
            LastName = "Smith",
            Email = "demo@surefire.com",
            PhoneNumber = "3103103103",
            PictureUrl = "default.png",
            EmailConfirmed = true
        };

        var result = userManager.CreateAsync(adminUser, "Password123!").Result;

        var folders = new[]
        {
            new Folder
            {
                Name = "Policy",
                Description = "For declarations pages and copies of policies"
            },
            new Folder
            {
                Name = "Endorsement",
                Description = "For carrier endorsements and policy changes"
            },
            new Folder
            {
                Name = "Quote",
                Description = "For quotes and documents from carriers regarding renewals and new business"
            },
            new Folder
            {
                Name = "Accounting",
                Description = "Invoices and such"
            },
            new Folder
            {
                Name = "Application",
                Description = "Apps and supps and supps and apps"
            }
        };

        context.Folders.AddRange(folders);

        var settings = new Surefire.Domain.Shared.Models.Settings();
        settings.FileStore = FileStoreType.Local;
        settings.DisablePlugins = false;
        context.Settings.Add(settings);

        context.SaveChanges();

        if (!result.Succeeded)
        {
            throw new Exception("Failed to create the admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}