using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    public class BaseTask
    {
        public int id { get; protected set; }
        public int requiredCores { get; protected set; }
        public int requiredMemory { get; protected set; }
        public double addTime { get; set; }
        public double workTime { get; protected set; }
        public double freeMemoryTime { get; protected set; }

        public BaseTask(int id,
                        int requiredCores,
                        int requiredMemory,
                        double addTime,
                        double workTime,
                        double freeMemoryTime)
        {
            this.id = id;
            this.requiredCores = requiredCores;
            this.requiredMemory = requiredMemory;
            this.addTime = addTime;
            this.workTime = workTime;
            this.freeMemoryTime = freeMemoryTime;
        }
    }
}
