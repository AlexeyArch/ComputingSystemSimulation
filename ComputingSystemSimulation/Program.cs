using System;

namespace ComputingSystemSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            bool trace = false;

            Loging.WriteLogFile("", 0, false);
            Simulation simulation = new Simulation();
            simulation.StartSimulation(trace);

            Console.ReadKey();
        }
    }
}
