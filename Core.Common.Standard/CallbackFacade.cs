using System.ServiceModel;

namespace KY.Core
{
    public class CallbackFacade<TChannel> : ChannelFacade<TChannel>
    {
        protected override TChannel CreateChannel()
        {
            return OperationContext.Current == null ? default(TChannel) : OperationContext.Current.GetCallbackChannel<TChannel>();
        }
    }
}