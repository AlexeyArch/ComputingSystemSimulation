using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            bool trace = false;
            double time = 10;
            // тип очереди: true - FIFO, false - PriorityQueue
            bool TypeOfQueue = false;

            Loging.WriteLogFile("", 0, false);
            Simulation simulation = new Simulation(TypeOfQueue);
            simulation.StartSimulation(TypeOfQueue, trace, time);

            Console.ReadKey();
        }
    }
}
