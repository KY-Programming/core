using System;
using System.ServiceModel;

namespace KY.Core
{
    public static class CommunicationObjectExtension
    {
        public static void CloseSafe(this ICommunicationObject proxy)
        {
            if (proxy == null || proxy.State == CommunicationState.Closing || proxy.State == CommunicationState.Closed)
                return;

            try
            {
                if (proxy.State != CommunicationState.Faulted)
                {
                    proxy.Close();
                }
                else
                {
                    proxy.Abort();
                }
            }
            catch (CommunicationException)
            {
                proxy.Abort();
            }
            catch (TimeoutException)
            {
                proxy.Abort();
            }
            catch (Exception)
            {
                proxy.Abort();
                throw;
            }
        }
    }
}