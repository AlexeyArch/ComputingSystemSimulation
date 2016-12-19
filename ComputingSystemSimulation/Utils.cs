using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    public class Utils
    {
        public static double ExponentialDistr(double lambda, double x)
        {
          //  return 1 - Math.Exp(-lambda * x);
            return -Math.Log(x) / lambda;
        }
    }
}
