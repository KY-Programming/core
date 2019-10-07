using System.ServiceModel;
using System.ServiceModel.Channels;

namespace KY.Core
{
    public class ClientFacade<TChannel, TCallback> : ChannelFacade<TChannel>
    {
        private readonly DuplexChannelFactory<TChannel> factory;

        protected TCallback Callback { get; private set; }

        public ClientFacade(string host, TCallback callback, Binding binding = null)
        {
            this.Callback = callback;
            binding = binding ?? BindingProvider.Get();
            this.factory = new DuplexChannelFactory<TChannel>(this.Callback, binding, host);
        }

        public ClientFacade(EndpointAddress endpoint, TCallback callback, Binding binding = null)
        {
            this.Callback = callback;
            binding = binding ?? BindingProvider.Get();
            this.factory = new DuplexChannelFactory<TChannel>(this.Callback, binding, endpoint);
        }

        protected override TChannel CreateChannel()
        {
            if(this.factory.State != CommunicationState.Opened && this.factory.State != CommunicationState.Opening)
            {
                this.factory.Open();
            }
            TChannel channel = this.factory.CreateChannel();
            return channel;
        }
    }
}