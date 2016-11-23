
using System;
using System.IO;

namespace ComputingSystemSimulation
{
    class Loging
    {
        public static void WriteLogFile ( string log, double SystemTime, bool append = true, string filename = "log.txt")
        {
            StreamWriter sw = new StreamWriter(filename, append);
            sw.WriteLine("[" + SystemTime + "]: " + log);
            //sw.WriteLine("[" + DateTime.UtcNow.ToString("hh:mm:ss") + "]: " + log);
            sw.Close();
        }

        public static void WriteLogConsole( string log, double SystemTime, bool pause = false)
        {
            Console.WriteLine("[" + SystemTime + "]: " + log);
            //Console.WriteLine("[" + DateTime.UtcNow.ToString("hh:mm:ss") + "]: " + log);
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

        public static string LogTask(BaseTask te)
        {
            string log = "Task:\n id = " + te.id +
                        " req_cores = " + te.requiredCores.ToString("0.00") +
                        " req_mem = " + te.requiredMemory.ToString("0.00") +
                        " add_time = " + te.addTime.ToString("0.00") +
                        " work_time = " + te.workTime.ToString("0.00") +
                        " free_mem = " + te.freeMemoryTime.ToString("0.00");
            return log;
        }
    }
}
