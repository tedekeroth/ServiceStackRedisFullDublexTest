using ServiceStack;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using tWorks.Alfa.AlfaCommons.Actors.Requests;

namespace EventConsumer.Services
{
    public class GetLegitimationsForCustomerService : Service
    {
        public object Any(GetLegitimationsForCustomer req)
        {
            System.Diagnostics.Debug.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: Reply '{TryResolve<ConsumerInfo>().Name}'. You send {nameof(GetLegitimationsForCustomer)}: customerid={req._Customer}");
            req.SetResultSuccess(true);
            return req;
        }
    }
}
