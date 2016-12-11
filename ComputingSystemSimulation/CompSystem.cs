using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ComputingSystemSimulation
{
    public class CompSystem
    {
        public int coresCount { get; private set; }
        public int memoryCount { get; private set; }
        public int maxTaskCoresCount { get; private set; }
        public int maxTaskMemoryCount { get; private set; }
        public double supplyIntensity { get; private set; }
        public double workIntensity { get; private set; }
        public double maxTaskWorkTime { get; private set; }
        public double freeMemoryTimeRatio { get; private set; }
        public double crashIntensity { get; private set; }
        public double recoveryIntensity { get; private set; }
        public int reserveCoresCount { get; private set; }
        public double maxTimeForWait { get; private set; }
        public double simulationTimeLimit { get; private set; }
        public bool priority { get; private set; }

        public List<int> cores;

        //текущее количество свободных ядер
        public int nowCoresCount { get; set; }
        //текущее количество свободной памяти
        public int nowMemoryCount { get; set; }

        public CompSystem() {
            ReadParamsFromXMLFile();

            cores = new List<int>();
            for (int i = 0; i < coresCount; i++)
                cores.Add(0);
        }

        //поиск ядер с указанным значением, по умолчанию 0 - свободных
        public List<int> getValueCore(int value = 0, int count = -1)
        {
            List<int> res = new List<int>();
            if (count < 0) count = coresCount;
            for (int i=0; i< count; i++)
            {
                if (cores[i] == value) res.Add(i);
            }

            return res;
        }

        public void setValueCore(int index, int value)
        {
            cores[index] = value;
        }

        public bool ReadParamsFromXMLFile()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("../../CompSystemParams.xml");
                coresCount = Convert.ToInt32(doc.DocumentElement.ChildNodes[0].InnerText);
                memoryCount = Convert.ToInt32(doc.DocumentElement.ChildNodes[1].InnerText);
                maxTaskCoresCount = Convert.ToInt32(doc.DocumentElement.ChildNodes[2].InnerText);
                maxTaskMemoryCount = Convert.ToInt32(doc.DocumentElement.ChildNodes[3].InnerText);
                supplyIntensity = Convert.ToDouble(doc.DocumentElement.ChildNodes[4].InnerText);
                workIntensity = Convert.ToDouble(doc.DocumentElement.ChildNodes[5].InnerText);
                maxTaskWorkTime = Convert.ToDouble(doc.DocumentElement.ChildNodes[6].InnerText);
                freeMemoryTimeRatio = Convert.ToDouble(doc.DocumentElement.ChildNodes[7].InnerText);
                crashIntensity = Convert.ToDouble(doc.DocumentElement.ChildNodes[8].InnerText);
                recoveryIntensity = Convert.ToDouble(doc.DocumentElement.ChildNodes[9].InnerText);
                reserveCoresCount = Convert.ToInt32(doc.DocumentElement.ChildNodes[10].InnerText);
                maxTimeForWait = Convert.ToDouble(doc.DocumentElement.ChildNodes[11].InnerText);
                simulationTimeLimit = Convert.ToDouble(doc.DocumentElement.ChildNodes[12].InnerText);
                priority = Convert.ToBoolean(doc.DocumentElement.ChildNodes[13].InnerText);

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
            if (task.requiredCores > nowCoresCount ||
                task.requiredMemory > nowMemoryCount)
                return false;
            else
                return true;
        }

        //отнимание ресурсов
        public void takeRes(int core, int memory)
        {
            nowCoresCount -= core;
            nowMemoryCount -= memory;
        }

        //возврат ресурсов
        public void returnRes(int core, int memory)
        {
            nowCoresCount += core;
            nowMemoryCount += memory;
        }
    }
}
