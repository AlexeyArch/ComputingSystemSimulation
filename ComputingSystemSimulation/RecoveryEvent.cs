using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    public class RecoveryEvent : Event
    {
        public int coreId { get; private set; }

        public RecoveryEvent(int coreId, double beginTimestamp)
        :base(EventTypes.RecoveryCore, beginTimestamp, 0)
        {
            this.coreId = coreId;
        }
    }
}
