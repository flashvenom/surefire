using System.Reflection;
using Surefire.Domain.Logs;
using System.Linq;
public static class PluginLoader
{
    public static void LoadPlugins(IServiceCollection services, string pluginsPath, IServiceProvider serviceProvider, ILoggingService _log)
    {
        if (!Directory.Exists(pluginsPath))
        {
            Console.WriteLine("Plugins folder not found.");
            _log.LogAsync(LogLevel.Error, $"Plugins folder not found: {pluginsPath}", "PluginLoader");
            return;
        }

        var pluginFiles = Directory.GetFiles(pluginsPath, "*.dll", SearchOption.AllDirectories);
        foreach (var file in pluginFiles)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                Console.WriteLine($"Loading plugins from {file}...");

                // Look for PluginStartup class
                var startupType = assembly.GetTypes().FirstOrDefault(t => t.Name == "PluginStartup");
                if (startupType != null)
                {
                    var registerMethod = startupType.GetMethod("RegisterPlugin");
                    if (registerMethod != null)
                    {
                        // Call RegisterPlugin
                        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                        registerMethod.Invoke(null, new object[] { services, configuration });
                        Console.WriteLine($"RegisterPlugin called for {startupType.FullName}");
                    }
                }

                // Register plugins implementing IPlugin
                var pluginTypes = assembly.GetTypes()
                    .Where(type => typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

                foreach (var pluginType in pluginTypes)
                {
                    services.AddScoped(typeof(IPlugin), pluginType);
                    Console.WriteLine($"Registered plugin: {pluginType.FullName}");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading plugin from {file}: {ex.Message}");
                _log.LogAsync(LogLevel.Information, $"Error loading plugin from:  {file}: {ex.Message}", "PluginLoader");
            }
        }
    }
}
