using System;
using System.Collections.Generic;

namespace ComputingSystemSimulation
{
    public class EventGenerator
    {
        /// <summary>
        /// Генератор задач
        /// </summary>
        /// <param name="compSystem">параметры ВС</param>
        /// <param name="lambda">интерсивность распределения</param>
        /// <param name="timeLimit">при превышении лимита постановка задач прекращается</param>
        /// <returns></returns>
        public static Dictionary<int, BaseTask> GenerateTasks(CompSystem compSystem)
        {
            Random rand = new Random();
            double time = 0.0;
            int x = 1;
            Dictionary<int, BaseTask> result = new Dictionary<int, BaseTask>();
            while (time < compSystem.simulationTimeLimit)
            {
                int cores = rand.Next(1, compSystem.maxTaskCoresCount);
                int memory = rand.Next(1, compSystem.maxTaskMemoryCount);
                double interval = Utils.ExponentialDistr(compSystem.supplyIntensity, rand.NextDouble());
                double workTime = Utils.ExponentialDistr(compSystem.workIntensity, rand.NextDouble());
                double maxWaitTime = (compSystem.priority) ? rand.NextDouble() * compSystem.maxTimeForWait : 0;
                double freeMemoryTime = workTime * compSystem.freeMemoryTimeRatio;
                time += interval;
                result.Add(x, new BaseTask(x, cores, memory, time, workTime, freeMemoryTime, maxWaitTime));
                x++;
            }

            return result;
        }

        public static List<CrashEvent> GenerateCrashes(CompSystem compSystem)
        {
            Random rand = new Random();
            double time = compSystem.crashBeginTimestamp;
            List<CrashEvent> result = new List<CrashEvent>();
            while (time < compSystem.simulationTimeLimit)
            {
                double interval = Utils.ExponentialDistr(compSystem.crashIntensity, rand.NextDouble());
                time += interval;
                result.Add(new CrashEvent(time));
            }

            return result;
        }
    }
}
