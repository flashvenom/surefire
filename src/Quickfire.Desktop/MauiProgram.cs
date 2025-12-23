using System.IO;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quickfire.Desktop.Logging;
using Quickfire.Desktop.Services;

namespace Quickfire.Desktop
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.Configuration.AddJsonFile("appsettings.maui.json", optional: true, reloadOnChange: false);
            var logDirectory = Path.Combine(DesktopStorage.GetAppDataRoot(), "logs");
            var logPath = Path.Combine(logDirectory, "quickfire.log");

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Logging.AddFileLogger(logPath);

            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.Configure<QuickfireHostOptions>(builder.Configuration.GetSection("QuickfireHost"));
            builder.Services.AddSingleton<IDesktopSetupService, DesktopSetupService>();
            builder.Services.AddSingleton<IQuickfireHost, QuickfireHostService>();
            builder.Services.AddSingleton<IDesktopEmberBridge, DesktopEmberBridge>();
            builder.Services.AddSingleton<IQuickfireTrayService, QuickfireTrayService>();
            builder.Services.AddSingleton<MainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
