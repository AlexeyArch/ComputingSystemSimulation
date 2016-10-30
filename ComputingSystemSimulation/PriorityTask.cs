using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    public class PriorityTask : BaseTask
    {
        public double waitTime { get; private set; }

        public PriorityTask(int id,
                            int requiredCores,
                            int requiredMemory,
                            double addTime,
                            double workTime,
                            double freeMemoryTime,
                            double waitTime) 
        :base(id, requiredCores, requiredMemory, addTime, workTime, freeMemoryTime)
        {
            this.waitTime = waitTime;
        }
    }
}
