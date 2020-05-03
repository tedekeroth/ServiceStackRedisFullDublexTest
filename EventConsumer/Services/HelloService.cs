using ServiceStack;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EventConsumer.Services
{
    public class HelloService : Service
    {
        public object Any(Hello req)
        {
            System.Diagnostics.Debug.WriteLine($"Thread { Thread.CurrentThread.ManagedThreadId}: Waiting 2 sec (text={req.Name})");
            Thread.Sleep(2000);
            return new HelloResponse { Result = $"Thread {Thread.CurrentThread.ManagedThreadId}: Reply '{TryResolve<ConsumerInfo>().Name}'. You wrote: " + req.Name };
        }
    }
}
