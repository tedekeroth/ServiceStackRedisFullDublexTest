using System;
using System.Collections.Generic;
using System.Text;
using tWorks.Core.CoreCommons;

namespace Shared
{
    public interface IProtocolContainer
    {
        CoreRequest TheRequest { get; set; }
    }

    public class VehicleContainer : IProtocolContainer
    {
        public CoreRequest TheRequest { get; set; }

        public VehicleContainer(CoreRequest request)
        {
            TheRequest = request;
        }
    }

    public class VehicleContainerResponse : IProtocolContainer
    {
        public CoreRequest TheRequest { get; set; }

        public VehicleContainerResponse(CoreRequest request)
        {
            TheRequest = request;
        }
    }
}
