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
            double time = 1000;
            // тип очереди: true - без приоритета, false - с приоритетом
            double MaxWaitTime = 10;

            Loging.WriteLogFile("", 0, false);
            Simulation simulation = new Simulation(MaxWaitTime);
            simulation.StartSimulation(trace, time);

            Console.ReadKey();
        }
    }
}
