
using System;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace ComputingSystemSimulation
{
    class Loging
    {
        public static void WriteLogFile ( string log, double SystemTime, bool append = true, string filename = "log.txt")
        {
            StreamWriter sw = new StreamWriter(filename, append);
            sw.WriteLine("[" + SystemTime.ToString("0.000") + "]: " + log);
            sw.Close();
        }

        public static void WriteLogConsole( string log, double SystemTime, bool pause = false)
        {
            Console.WriteLine("[" + SystemTime.ToString("0.000000") + "]: " + log);
            if (pause) Console.ReadKey();
        }

        public static string LogEvent (TaskEvent te)
        {
            string log = te.type.ToString() + ": \n\t id = " + te.taskId + 
                        " \n\t begin_time = " + te.beginTimestamp.ToString("0.0000") + 
                        " \n\t duration_time = " + te.duration.ToString("0.0000");
            return log;
        }

        public static string LogCompSys (CompSystem csp)
        {
            string log = "State System:  cores = " + csp.nowCoresCount + "(" + csp.coresCount + ")"+ 
                         " memory = " + csp.nowMemoryCount + "(" + csp.memoryCount + ")";
            return log;
        }

        public static string LogTask(BaseTask te)
        {
            string log = "Task:\n\t id = " + te.id +
                        " \n\t req_cores = " + te.requiredCores.ToString("0.00") +
                        " \n\t req_mem = " + te.requiredMemory.ToString("0.00") +
                        " \n\t add_time = " + te.addTime.ToString("0.00") +
                        " \n\t work_time = " + te.workTime.ToString("0.00") +
                        " \n\t free_mem = " + te.freeMemoryTime.ToString("0.00");
            return log;
        }

        public static void writingInExcMethod(int rows, int cols, int id, double time)
        {
            try
            {
                string LogOfTasks = Path.GetFullPath("LogOfTasks.xlsx");
                
                if (File.Exists(LogOfTasks))
                {
                    Excel.Application ObjExcel = new Excel.Application();
                    Excel.Workbook ObjWorkBook;
                    Excel.Worksheet ObjWorkSheet;

                    ObjWorkBook = ObjExcel.Workbooks.Open(LogOfTasks);
                    ObjWorkSheet = (Excel.Worksheet)ObjWorkBook.ActiveSheet;
                    ObjExcel.Visible = false;

                    ObjWorkSheet.Cells[rows, cols] = id;
                    ObjWorkSheet.Cells[rows, cols + 1] = time;

                    ObjWorkBook.Save();
                    ObjWorkBook.Close();
                    ObjExcel.Quit();

                }
                else
                {
                    Excel.Application ObjExcele = new Excel.Application();
                    Excel.Workbook Book = ObjExcele.Workbooks.Add(Type.Missing);
                    Excel.Worksheet Sheet = (Excel.Worksheet)ObjExcele.ActiveSheet;

                    Sheet.Cells[rows, cols] = id;
                    Sheet.Cells[rows, cols + 1] = time;

                    Sheet.SaveAs(LogOfTasks);
                    Book.Close();
                    ObjExcele.Quit();
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Ошибка при составлении лога\n" + exc.Message);
            }
        }
    }
}
