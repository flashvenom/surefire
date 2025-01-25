namespace Surefire.Domain.Plugins
{
    public interface IDataSyncPlugin : IPlugin
    {
        Task<PluginMethodResponse> DataSync(int clientId, bool forceUpdate, CancellationToken cancellationToken);
        Task<PluginMethodResponse> GetContacts(string eClientId);
    }
}