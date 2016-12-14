using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ComputingSystemSimulation
{
    public class CompSystem
    {
        public class Core
        {
            public int id;
            public bool busy = false;
            public bool working = true;
            public int taskId = 0;

            public Core(int id, bool busy)
            {
                this.id = id;
            }

            public override string ToString()
            {
                return taskId.ToString();
            }
        }

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
        public double crashBeginTimestamp { get; private set; }

        public List<Core> workingCores;
        public Dictionary<int, Core> crashedCores;

        //текущее количество свободных ядер
        public int nowCoresCount { get; set; }
        //текущее количество свободной памяти
        public int nowMemoryCount { get; set; }

        public CompSystem() {
            ReadParamsFromXMLFile();

            nowCoresCount = coresCount;
            nowMemoryCount = memoryCount;

            workingCores = new List<Core>();
            crashedCores = new Dictionary<int, Core>();
            for (int i = 0; i < coresCount; i++)
                workingCores.Add(new Core(i, false));
        }

        ////поиск ядер с указанным значением, по умолчанию 0 - свободных
        //public List<int> getValueCore(int value = 0, int count = -1)
        //{
        //    List<int> res = new List<int>();
        //    if (count < 0) count = coresCount;
        //    for (int i=0; i < count; i++)
        //    {
        //        if (cores[i] == value) res.Add(i);
        //    }

        //    return res;
        //}

        //private void setValueCore(int index, int value)
        //{
        //    cores[index] = value;
        //}

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
                crashBeginTimestamp = Convert.ToDouble(doc.DocumentElement.ChildNodes[14].InnerText);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        } 

        //проверка, хватит ли ресурсов на задачу
        public bool IsFreeRes (BaseTask task)
        {
            if (task.requiredCores > nowCoresCount ||
                task.requiredMemory > nowMemoryCount)
                return false;
            else
                return true;
        }

        //отнимание ресурсов
        public void TakeRes(BaseTask task)
        {
            int counter = 0;
            int i = 0;
            while (counter < task.requiredCores)
            {
                if(!workingCores[i].busy)
                {
                    workingCores[i].busy = true;
                    workingCores[i].taskId = task.id;
                    counter++;
                }
                i++;
            }
            nowCoresCount -= task.requiredCores;
            nowMemoryCount -= task.requiredMemory;

            //List<int> freeCores = getValueCore(0, task.requiredCores);
            //for (int i = 0; i < cores.Count(); i++)
            //    setValueCore(freeCores[i], task.id);

            //task.setCores(freeCores);
        }

        //возврат ресурсов
        public void ReturnRes(BaseTask task)
        {
            foreach (Core core in workingCores)
                if (core.taskId == task.id)
                {
                    core.taskId = 0;
                    core.busy = false;
                }

            nowCoresCount += task.requiredCores;
            nowMemoryCount += task.requiredMemory;
        }

        public int CrashCore()
        {
            Random rand = new Random();
            int coreIndex = rand.Next(0, workingCores.Count() - 1);
            Core core = workingCores[coreIndex];
            workingCores.RemoveAt(coreIndex);
            if (!core.busy)
                nowCoresCount--;
            else
                core.busy = false;

            crashedCores.Add(core.id, core);
            return core.id;
        }

        public int GetTaskId2Core (int coreIndex)
        {
            return crashedCores[coreIndex].taskId;
        }

        public void RecoveryCore(int coreId)
        {
            workingCores.Add(crashedCores[coreId]);
            crashedCores.Remove(coreId);
            nowCoresCount++;
        }
    }
}
