using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ComputingSystemSimulation
{
    public class CompSystemParams
    {
        public int coresCount { get; private set; }
        public int reserveCoresCount { get; private set; }
        public int memoryCount { get; private set; }
        public double maxTaskWorkTime { get; private set; }
        public double maxFreeMemoryTime { get; private set; }
        public double maxTimeForWait { get; private set; }

        //текущее количество свободных ядер
        public int nowCoresCount { get; set; }
        //текущее количество свободной памяти
        public int nowMemoryCount { get; set; }

        public CompSystemParams() { }

        public bool ReadParamsFromXMLFile()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("../../CompSystemParams.xml");
                coresCount = Convert.ToInt32(doc.DocumentElement.ChildNodes[0].InnerText);
                memoryCount = Convert.ToInt32(doc.DocumentElement.ChildNodes[1].InnerText);
                maxTaskWorkTime = Convert.ToDouble(doc.DocumentElement.ChildNodes[2].InnerText);
                maxFreeMemoryTime = Convert.ToDouble(doc.DocumentElement.ChildNodes[3].InnerText);
                reserveCoresCount = Convert.ToInt32(doc.DocumentElement.ChildNodes[4].InnerText);
                maxTimeForWait = Convert.ToDouble(doc.DocumentElement.ChildNodes[5].InnerText);

                nowCoresCount = coresCount;
                nowMemoryCount = memoryCount;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        } 

        //проверка, хватит ли ресурсов на задачу
        public bool isFreeRes (BaseTask task)
        {
            if (task.requiredCores > nowCoresCount
                || task.requiredMemory > nowMemoryCount)
                return false;
            return true;
        }

        //отнимание ресурсов
        public void takeRes (int core, int memory)
        {
            nowCoresCount -= core;
            nowMemoryCount -= memory;
        }

        //отнимание ресурсов
        public void returnRes(int core, int memory)
        {
            nowCoresCount += core;
            nowMemoryCount += memory;
        }
    }
}
