using RingCentral;

namespace Surefire.Domain.Plugins
{
    public interface ICallLogPlugin : IPlugin
    {
        Task<List<CallLogRecord>> GetCallLogsAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken);
    }
}