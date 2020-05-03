using ServiceStack.Messaging.Redis;
using ServiceStack.Redis;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

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

            //Client - MQ Service Impl:
            //Listens for 'HelloResponse' returned by the 'Hello' Service
            mqServer.RegisterHandler<HelloResponse>(m => {
                OnServiceHandlerTriggered?.Invoke(this, $"{name}> Received {nameof(HelloResponse)}: {m.GetBody().Result}");
                return null;
            });

            //or to call an existing service with:
            //mqServer.RegisterHandler<HelloResponse>(m =>   
            //    this.ServiceController.ExecuteMessage(m));

            mqServer.Start(); //Starts listening for messages
        }

        public void AddHello(string name)
        {
            using (var mqClient = mqServer.CreateMessageQueueClient())
            {
                mqClient.Publish(new Hello { Name = name });
            }
        }

    }
}
