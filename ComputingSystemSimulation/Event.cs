using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    public class Event
    {
        public enum EventTypes
        {
            AddTask, BeginComputeTask, EndComputeTask, FreeMemory
        }

        public EventTypes type { get; private set; }
        public double beginTimestamp { get; private set; }
        public double duration { get; private set; }

        public Event(EventTypes type, double beginTimestamp, double duration)
        {
            this.type = type;
            this.beginTimestamp = beginTimestamp;
            this.duration = duration;
        }
    }
}
