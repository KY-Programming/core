using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace KY.Core
{
    public static class BindingProvider
    {
        private static Binding binding;

        public static Binding Get()
        {
            if (binding == null)
            {
                NetTcpBinding tcpBinding = new NetTcpBinding();
                tcpBinding.Security.Mode = SecurityMode.None;
                tcpBinding.MaxReceivedMessageSize = int.MaxValue;
                tcpBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                tcpBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
                tcpBinding.MaxConnections = 1000;
                tcpBinding.ReceiveTimeout = new TimeSpan(24, 0, 0);
                binding = tcpBinding;
            }
            return binding;
        }
    }
}