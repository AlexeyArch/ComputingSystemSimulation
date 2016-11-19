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
            Loging.WriteLogFile("", false);
            Simulation simulation = new Simulation();
            simulation.StartSimulation();

            Console.ReadKey();
        }
    }
}
