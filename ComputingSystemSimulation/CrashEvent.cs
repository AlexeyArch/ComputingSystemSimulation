using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    public class CrashEvent : Event
    {
        public int coreId { get; private set; }

        public CrashEvent(double beginTimestamp)
        :base(EventTypes.CrashCore, beginTimestamp, 0) { }
    }
}
