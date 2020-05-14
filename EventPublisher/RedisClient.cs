using ServiceStack.Messaging.Redis;
using ServiceStack.Redis;
using ServiceStack.Text;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;
using tWorks.Core.CoreCommons;

namespace EventPublisher
{
    class RedisClient
    {
        PooledRedisClientManager redisFactory;
        RedisMqServer mqServer;
        public string Name { get; }

        public event EventHandler<string> OnServiceHandlerTriggered;

        public RedisClient(string name)
        {
            Name = name;
            redisFactory = new PooledRedisClientManager("localhost:6379");
            mqServer = new RedisMqServer(redisFactory, retryCount: 2);

            mqServer.RequestFilter = RequestFilter;

            //Client - MQ Service Impl:
            ////Listens for 'HelloResponse' returned by the 'Hello' Service
            mqServer.RegisterHandler<VehicleContainerResponse>(m =>
            {
                return m;
            });

            mqServer.RegisterHandler<tWorks.Alfa.AlfaCommons.Actors.Requests.GetLegitimationsForCustomer>(m =>
            {
                object o = m.GetBody().Result;
                OnServiceHandlerTriggered?.Invoke(this, $"{name}> Received {nameof(tWorks.Alfa.AlfaCommons.Actors.Requests.GetLegitimationsForCustomer)}Response?: {m.Body.Dump()}");
                return null;
            });

            //or to call an existing service with:
            //mqServer.RegisterHandler<HelloResponse>(m =>   
            //    this.ServiceController.ExecuteMessage(m));

            mqServer.Start(); //Starts listening for messages
        }

        private ServiceStack.Messaging.IMessage RequestFilter(ServiceStack.Messaging.IMessage message)
        {
            if (message.Body is IProtocolContainer protocolContainer)
            {
                message.Body = protocolContainer.TheRequest;
            }
            return message;
        }

        public void AddMessage<T>(T coreRequest) where T : CoreRequest
        {
            using (var mqClient = mqServer.CreateMessageQueueClient())
            {
                mqClient.Publish(new VehicleContainer(coreRequest));
            }
        }

    }
}
