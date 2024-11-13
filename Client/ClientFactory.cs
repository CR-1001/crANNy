/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Threading.Tasks;
    using CRAI.Server;

    public class ClientFactory
    {
        public IService Create(String uri, TimeSpan timeSpanTimeout)
        {
            return Create(new Uri(uri), timeSpanTimeout);
        }

        public IService Create(Uri uri, TimeSpan timeSpanTimeout)
        {
            var binding = GetBinding(uri);

            binding.CloseTimeout = timeSpanTimeout;
            binding.SendTimeout = timeSpanTimeout;
            binding.ReceiveTimeout = timeSpanTimeout;

            var client = new Client(uri, binding);

            client.Open();

            return client;
        }

        private Binding GetBinding(Uri uri, int maxMessageSize = Int32.MaxValue)
        {
            if(uri.Scheme == "net.tcp")
            {
                return new NetTcpBinding(SecurityMode.Message) 
                { 
                    MaxReceivedMessageSize = maxMessageSize,
                    MaxBufferSize = maxMessageSize,
                    MaxBufferPoolSize = maxMessageSize,
                };
            }
            else if(uri.Scheme == "https")
            {
                return new BasicHttpsBinding() 
                { 
                    MaxReceivedMessageSize = maxMessageSize,
                    MaxBufferSize = maxMessageSize,
                    MaxBufferPoolSize = maxMessageSize,
                };
            }
            else if(uri.Scheme == "http")
            {
                return new BasicHttpBinding() 
                { 
                    MaxReceivedMessageSize = maxMessageSize,
                    MaxBufferSize = maxMessageSize,
                    MaxBufferPoolSize = maxMessageSize,
                };
            }
            else
            {
                throw new ArgumentException("uri");
            }
        }
    }
}
