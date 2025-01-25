using Surefire.Domain.Plugins;
public interface IPlugin : IPluginBase
{
    Task<PluginMethodResponse> ExecuteAsync(string methodName, object[] parameters, CancellationToken cancellationToken);
}