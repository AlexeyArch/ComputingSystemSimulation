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

        public override string LogEvent()
        {
            string log = type.ToString() + ": \n\t id = " + taskId +
                            " \n\t begin_time = " + beginTimestamp.ToString("0.0000") +
                            " \n\t duration_time = " + duration.ToString("0.0000");
            return log;
        }
    }
}
