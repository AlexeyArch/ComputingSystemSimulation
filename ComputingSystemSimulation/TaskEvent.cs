using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    public class TaskEvent : Event
    {
        public int taskId { get; private set; }
        

        public TaskEvent(EventTypes type, int taskId, double beginTimestamp, double duration)
        :base(type, beginTimestamp, duration)
        {
            this.taskId = taskId;
        }
    }
}
