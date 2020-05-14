using EventConsumer.Services;
using Funq;
using ServiceStack;
using ServiceStack.Messaging.Redis;
using ServiceStack.Redis;
using ServiceStack.Text;
using ServiceStack.Web;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;
using tWorks.Core.CoreCommons;

namespace EventConsumer
{
    public class ServerAppHost : AppHostHttpListenerBase
    {
        public event EventHandler<string> OnMessageReceived;

        public ServerAppHost(string name) : base(name, typeof(HelloService).Assembly) { }

        public override void Configure(Container container)
        {
            //base.Routes
            //    .Add<Hello>("/hello")
            //    .Add<Hello>("/hello/{Name}");

            container.Register(new ConsumerInfo() { Name = ServiceName });

            var redisFactory = new PooledRedisClientManager("localhost:6379");
            container.Register<IRedisClientsManager>(redisFactory);
            var mqHost = new RedisMqServer(redisFactory, retryCount: 2);

            GlobalMessageRequestFilters.Add(MyGlobalRequestFilter);

            mqHost.RequestFilter = RequestFilter;
            mqHost.ResponseFilter = ResponseFilter;

            //Listens for 'Hello' messages sent with: mqClient.Publish(new Hello { ... })
            mqHost.RegisterHandler<VehicleContainer>(base.ExecuteMessage);
            mqHost.RegisterHandler<tWorks.Alfa.AlfaCommons.Actors.Requests.GetLegitimationsForCustomer>(base.ExecuteMessage);
            mqHost.Start(); //Starts listening for messages
        }

        private object ResponseFilter(object arg)
        {
            return new VehicleContainerResponse(arg as CoreRequest);
        }

        private void MyGlobalRequestFilter(IRequest req, IResponse response, object obj)
        {
            System.Diagnostics.Debug.WriteLine("MyGlobalRequestFilter");
            System.Diagnostics.Debug.WriteLine(req);
            System.Diagnostics.Debug.WriteLine(response);
            System.Diagnostics.Debug.WriteLine(obj);
            System.Diagnostics.Debug.WriteLine("/MyGlobalRequestFilter");
        }

        private ServiceStack.Messaging.IMessage RequestFilter(ServiceStack.Messaging.IMessage message)
        {
            System.Diagnostics.Debug.WriteLine($"{nameof(RequestFilter)}\t{message.Body.GetType().Name}");
            if (message.Body is IProtocolContainer protocolContainer)
            {
                System.Diagnostics.Debug.WriteLine($"\tReplaced Body with {protocolContainer.TheRequest.GetType().Name}");
                message.Body = protocolContainer.TheRequest;
            }
            OnMessageReceived?.Invoke(this, $"{message.Body.GetType().Name}> {message.Body.Dump()}");
            return message;
        }
    }
}
