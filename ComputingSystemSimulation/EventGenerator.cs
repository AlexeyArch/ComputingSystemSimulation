using System;
using System.Collections.Generic;

namespace ComputingSystemSimulation
{
    public class EventGenerator
    {
        /// <summary>
        /// Генератор задач
        /// </summary>
        /// <param name="compSystemParams">параметры ВС</param>
        /// <param name="lambda">интерсивность распределения</param>
        /// <param name="timeLimit">при превышении лимита постановка задач прекращается</param>
        /// <returns></returns>
        public static Dictionary<int, BaseTask> GenerateTasks(CompSystemParams compSystemParams, double lambda, double timeLimit, bool priority = false)
        {
            Random rand = new Random();
            double time = 0.0;
            int x = 0;
            Dictionary<int, BaseTask> result = new Dictionary<int, BaseTask>();
            while (time < timeLimit)
            {
                int cores = rand.Next(1, compSystemParams.coresCount);
                int memory = rand.Next(1, compSystemParams.memoryCount);
                time += ExponentialDistr(lambda, rand.NextDouble());
                double workTime =  rand.NextDouble() * compSystemParams.maxTaskWorkTime;
                double maxWaitTime = (priority)? rand.NextDouble() * compSystemParams.maxTimeForWait: 0;
                double freeMemoryTime = rand.NextDouble() * compSystemParams.maxFreeMemoryTime;
                result.Add(x, new BaseTask(x, cores, memory, time, workTime, freeMemoryTime, maxWaitTime));
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
