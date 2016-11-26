using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    class PriorityEventGenerator
    {
        public static Dictionary<int, PriorityTask> GenerateTasks(CompSystemParams compSystemParams, double lambda, double timeLimit)
        {
            Random rand = new Random();
            double time = 0.0;
            int x = 0;
            Dictionary<int, PriorityTask> result = new Dictionary<int, PriorityTask>();
            while (time < timeLimit)
            {
                int cores = rand.Next(1, compSystemParams.coresCount);
                int memory = rand.Next(1, compSystemParams.memoryCount);
                time += ExponentialDistr(lambda, rand.NextDouble());
                double workTime = rand.NextDouble() * compSystemParams.maxTaskWorkTime;
                double freeMemoryTime = rand.NextDouble() * compSystemParams.maxFreeMemoryTime;
                double maxWaitTime = rand.NextDouble() * compSystemParams.maxTimeForWait;
                result.Add(x, new PriorityTask(x, cores, memory, time, workTime, freeMemoryTime, maxWaitTime, x));
                x++;
            }

            return result;
        }

        private static double ExponentialDistr(double lambda, double x)
        {
            //return 1 - Math.Exp(-lambda * x);
            return -Math.Log(x) / lambda;
        }


    }
}
