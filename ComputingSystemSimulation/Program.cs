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
            Loging.WriteLogFile("", 0, false);
            Simulation simulation = new Simulation();
            simulation.StartSimulation();

            Console.ReadKey();
        }
    }
}
