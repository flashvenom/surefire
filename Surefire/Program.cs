using Surefire.Data;
using Surefire.Domain.Logs;
using Surefire.Domain.Ember;
using Surefire.Domain.OpenAI;
using Surefire.Domain.Plugins;
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
using DotNetEnv;
using Syncfusion.Blazor;
using Surefire.Components;
using Surefire.Components.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components.Components.Tooltip;


// INITIAL VARIABLES -- -- -- -   -     -
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddMemoryCache();
Env.Load();
bool detailedErrorsEnabled = builder.Configuration.GetValue<bool>("DetailedErrors:Enabled");


// SYNCFUSION -- -- -- -   -     -
builder.Services.AddSyncfusionBlazor();
builder.Services.AddFluentUIComponents();
builder.Services.AddDataGridEntityFrameworkAdapter();
string syncFusionKey = Environment.GetEnvironmentVariable("SYNCFUSION");
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncFusionKey);


// IDEN AND AUTH -- -- -- -   -     -
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddAuthentication(options => { 
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
string openAiApiKey = Environment.GetEnvironmentVariable("OPENAI") ?? "MISSING";
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

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

//app.UseHttpsRedirection(); //Turn off for WenView2 Desktop runtime
app.UseDeveloperExceptionPage();
app.UseMigrationsEndPoint();
app.MapStaticAssets();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<EmberHub>("/emberHub");
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapAdditionalIdentityEndpoints();

// Check if we need to configure and seed a local database
if(databaseType == "SQLite")
{
    using (var scope = app.Services.CreateScope())
    {
        var scopedServiceProvider = scope.ServiceProvider;

        var dbContext = scopedServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            dbContext.Database.Migrate(); // Applies any pending migrations
            SeedInitialData.SeedData(scopedServiceProvider);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while creating or migrating the database: {ex}");
            throw;
        }
    }
}

app.Run();