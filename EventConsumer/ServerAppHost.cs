using EventConsumer.Services;
using Funq;
using ServiceStack;
using ServiceStack.Messaging.Redis;
using ServiceStack.Redis;
using ServiceStack.Text;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventConsumer
{
    public class ServerAppHost : AppHostHttpListenerBase
    {
        public event EventHandler<string> OnMessageReceived;

        public ServerAppHost(string name) : base(name, typeof(HelloService).Assembly) { }

        public override void Configure(Container container)
        {
            base.Routes
                .Add<Hello>("/hello")
                .Add<Hello>("/hello/{Name}");

            container.Register(new ConsumerInfo() { Name = ServiceName });

            var redisFactory = new PooledRedisClientManager("localhost:6379");
            container.Register<IRedisClientsManager>(redisFactory);
            var mqHost = new RedisMqServer(redisFactory, retryCount: 2);


            mqHost.RequestFilter = RequestFilter;

            //Listens for 'Hello' messages sent with: mqClient.Publish(new Hello { ... })
            mqHost.RegisterHandler<Hello>(base.ExecuteMessage);
            mqHost.Start(); //Starts listening for messages
        }

        private ServiceStack.Messaging.IMessage RequestFilter(ServiceStack.Messaging.IMessage message)
        {
            OnMessageReceived?.Invoke(this, $"{message.Body.GetType().Name}> {message.Body.Dump()}");
            return message;
        }
    }
}
