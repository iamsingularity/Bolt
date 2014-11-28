using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt
{
    public class DefaultWebRequestHandler : IWebRequestHandler
    {
        [DebuggerStepThrough]
        public virtual async Task<HttpWebResponse> GetResponseAsync(HttpWebRequest request, TimeSpan timeout, CancellationToken cancellation)
        {
            if (cancellation == CancellationToken.None)
            {
                return (HttpWebResponse)await GetResponseAsyncCore(request, timeout);
            }

            try
            {
                using (cancellation.Register(
                    () =>
                    {
                        try
                        {
                            request.Abort();
                        }
                        catch (ObjectDisposedException)
                        {
                        }
                    },
                    false))
                {
                    WebResponse response = await GetResponseAsyncCore(request, timeout);
                    cancellation.ThrowIfCancellationRequested();
                    return (HttpWebResponse)response;
                }
            }
            catch (WebException e)
            {
                cancellation.ThrowIfCancellationRequested();
                e.EnsureNotCancelled();

                if (e.Status == WebExceptionStatus.RequestCanceled)
                {
                    throw new OperationCanceledException(cancellation);
                }

                throw;
            }
        }

        [DebuggerStepThrough]
        public virtual HttpWebResponse GetResponse(HttpWebRequest request, TimeSpan timeout, CancellationToken cancellation)
        {
            if (cancellation == CancellationToken.None)
            {
                return (HttpWebResponse)GetResponseCore(request, timeout);
            }

            try
            {
                using (cancellation.Register(
                    () =>
                    {
                        try
                        {
                            request.Abort();
                        }
                        catch (ObjectDisposedException)
                        {
                        }
                    },
                    false))
                {
                    WebResponse response = GetResponseCore(request, timeout);
                    cancellation.ThrowIfCancellationRequested();
                    return (HttpWebResponse)response;
                }
            }
            catch (WebException e)
            {
                cancellation.ThrowIfCancellationRequested();
                e.EnsureNotCancelled();

                if (e.Status == WebExceptionStatus.RequestCanceled)
                {
                    throw new OperationCanceledException(cancellation);
                }

                throw;
            }
        }

        [DebuggerStepThrough]
        public virtual Stream GetRequestStream(HttpWebRequest response)
        {
            return TaskExtensions.Execute(response.GetRequestStreamAsync);
        }

        [DebuggerStepThrough]
        protected virtual WebResponse GetResponseCore(HttpWebRequest webRequest, TimeSpan timeout)
        {
            return TaskExtensions.Execute(() => GetResponseAsyncCore(webRequest, timeout));
        }

        [DebuggerStepThrough]
        protected virtual Task<WebResponse> GetResponseAsyncCore(HttpWebRequest webRequest, TimeSpan timeout)
        {
            return webRequest.GetResponseAsync();
        }
    }
}