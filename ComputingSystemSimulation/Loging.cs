
using System;
using System.IO;

namespace ComputingSystemSimulation
{
    class Loging
    {
        public static void WriteLogFile ( string log, bool append = true, string filename = "log.txt")
        {
            StreamWriter sw = new StreamWriter(filename, append);
            sw.WriteLine("[" + DateTime.UtcNow.ToString("hh:mm:ss") + "]: " + log);
            sw.Close();
        }

        public static void WriteLogConsole( string log, bool pause = false)
        {            
            Console.WriteLine("[" + DateTime.UtcNow.ToString("hh:mm:ss") + "]: " + log);
            if (pause) Console.ReadKey();
        }

        public static string LogEvent (TaskEvent te)
        {
            string log = te.type.ToString() + ": \n\t id = " + te.taskId + 
                        " st = " + te.beginTimestamp.ToString("0.0000") + 
                        " d = " + te.duration.ToString("0.0000");
            return log;
        }

        public static string LogCompSys (CompSystemParams csp)
        {
            string log = "State System:  cores = " + csp.nowCoresCount + "(" + csp.coresCount + ")"+ 
                         " memory = " + csp.nowMemoryCount + "(" + csp.memoryCount + ")";
            return log;
        }

    }
}
