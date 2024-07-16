public void ConfigureServices(IServiceCollection services)
{
    services.AddRazorPages();
    services.AddServerSideBlazor();
    services.AddHttpClient<CrmApiService>(client =>
    {
        client.BaseAddress = new Uri("https://api.***REMOVED***/");
    }).ConfigureHttpClient((sp, client) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        var clientId = config["ClientId"];
        var clientSecret = config["ClientSecret"];
        return new CrmApiService(client, clientId, clientSecret);
    });
}
