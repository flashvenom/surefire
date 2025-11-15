using System.Collections.Generic;

namespace Surefire.Domain.Plugins
{
    public class PluginManager
    {
        private readonly IEnumerable<IPlugin> _plugins;

        public PluginManager(IEnumerable<IPlugin> plugins)
        {
            _plugins = plugins;
        }

        public IEnumerable<IPlugin> GetAllPlugins() => _plugins;

        public T? GetPluginByType<T>() where T : class, IPlugin
        {
            return _plugins.OfType<T>().FirstOrDefault();
        }

        public IPlugin? GetPluginByName(string name)
        {
            return _plugins.FirstOrDefault(plugin => plugin.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
