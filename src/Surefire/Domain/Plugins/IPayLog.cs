namespace Surefire.Domain.Plugins
{
    public interface IPayLogPlugin : IPlugin
    {
        Task<PluginMethodResponse> GetRecentPayments(CancellationToken cancellationToken);
    }
}