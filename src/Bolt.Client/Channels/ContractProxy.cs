using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.Client.Channels
{
    public abstract class ContractProxy<TContractDescriptor> : IContractDescriptorProvider<TContractDescriptor>, IChannel
        where TContractDescriptor : ContractDescriptor
    {
        protected ContractProxy(ContractProxy<TContractDescriptor> proxy)
        {
            if (proxy == null)
            {
                throw new ArgumentNullException("proxy");
            }

            Descriptor = proxy.Descriptor;
            Channel = proxy.Channel;
        }

        protected ContractProxy(TContractDescriptor contractDescriptor, IChannel channel)
        {
            if (contractDescriptor == null)
            {
                throw new ArgumentNullException("contractDescriptor");
            }

            if (channel == null)
            {
                throw new ArgumentNullException("channel");
            }

            Descriptor = contractDescriptor;
            Channel = channel;
        }

        public TContractDescriptor Descriptor { get; private set; }

        public IChannel Channel { get; private set; }

        public virtual ContractProxy<TContractDescriptor> Clone(IChannel channel = null)
        {
            ContractProxy<TContractDescriptor> proxy = (ContractProxy<TContractDescriptor>)Activator.CreateInstance(GetType(), this);
            if (channel != null)
            {
                proxy.Channel = channel;
            }

            return proxy;
        }

        #region IChannel Implementation

        public virtual CancellationToken GetCancellationToken(ActionDescriptor descriptor)
        {
            return Channel.GetCancellationToken(descriptor);
        }

        public void Dispose()
        {
            Channel.Dispose();
        }

        void IChannel.Open()
        {
            Channel.Open();
        }

        Task IChannel.OpenAsync()
        {
            return Channel.OpenAsync();
        }

        bool IChannel.IsOpened
        {
            get { return Channel.IsOpened; }
        }

        void IChannel.Close()
        {
            Channel.Close();
        }

        Task IChannel.CloseAsync()
        {
            return Channel.CloseAsync();
        }

        bool IChannel.IsClosed
        {
            get { return Channel.IsClosed; }
        }

        Task IChannel.SendAsync<TRequestParameters>(TRequestParameters parameters, ActionDescriptor descriptor, CancellationToken cancellation)
        {
            return Channel.SendAsync(parameters, descriptor, cancellation);
        }

        Task<TResult> IChannel.SendAsync<TResult, TRequestParameters>(TRequestParameters parameters, ActionDescriptor descriptor, CancellationToken cancellation)
        {
            return Channel.SendAsync<TResult, TRequestParameters>(parameters, descriptor, cancellation);
        }

        void IChannel.Send<TRequestParameters>(TRequestParameters parameters, ActionDescriptor descriptor, CancellationToken cancellation)
        {
            Channel.Send(parameters, descriptor, cancellation);
        }

        TResult IChannel.Send<TResult, TRequestParameters>(TRequestParameters parameters, ActionDescriptor descriptor, CancellationToken cancellation)
        {
            return Channel.Send<TResult, TRequestParameters>(parameters, descriptor, cancellation);
        }

        #endregion
    }
}