using ServiceStack;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventConsumer.Services
{
    public class HelloService : Service
    {
        public object Any(Hello req)
        {
            
            return new HelloResponse { Result = $"This is a reply from Consumer '{TryResolve<ConsumerInfo>().Name}'. You wrote: " + req.Name };
        }
    }
}
