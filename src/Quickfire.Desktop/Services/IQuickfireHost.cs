using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quickfire.Desktop.Services
{
    public interface IQuickfireHost : IAsyncDisposable
    {
        Task<Uri> EnsureStartedAsync(CancellationToken cancellationToken = default);
    }
}
