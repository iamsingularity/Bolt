using System.Threading;
using System.Threading.Tasks;

namespace Bolt.Client
{
    public interface IChannel : ICancellationTokenProvider, ICloseable
    {
        void Open();

        Task OpenAsync();

        bool IsOpened { get; }

        Task SendAsync<TRequestParameters>(TRequestParameters parameters, ActionDescriptor descriptor, CancellationToken cancellation);

        Task<TResult> SendAsync<TResult, TRequestParameters>(TRequestParameters parameters, ActionDescriptor descriptor, CancellationToken cancellation);

        void Send<TRequestParameters>(TRequestParameters parameters, ActionDescriptor descriptor, CancellationToken cancellation);

        TResult Send<TResult, TRequestParameters>(TRequestParameters parameters, ActionDescriptor descriptor, CancellationToken cancellation);
    }
}