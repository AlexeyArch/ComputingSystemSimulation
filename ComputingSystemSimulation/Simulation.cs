using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    public class Simulation
    {
        //параметры ВС
        private CompSystemParams compSystemParams = new CompSystemParams();
        //календарь событий
        private EventsCalendar eventsCalendar = new EventsCalendar();
        //задачи
        private Dictionary<int, BaseTask> tasks;
        private Dictionary<int, PriorityTask> tasks2;
        //очередь задач
        private Queue<BaseTask> tasksQueue = new Queue<BaseTask>();
        private List<PriorityTask> tasksQueue2 = new List<PriorityTask>();

        private FIFOqueue fifo = new FIFOqueue();
        private PriorityQueue priorityQueue = new PriorityQueue();

        private double SystemTime = 0;
        private double MaxTimeInQueue = 0;

        public Simulation(bool TYPE = true)
        {
            compSystemParams.ReadParamsFromXMLFile();

            //генерируем задачи
            if (TYPE)
            {
                tasks = EventGenerator.GenerateTasks(compSystemParams, 0.7, 10);
                //добавление события постановки в очередь
                foreach (BaseTask task in tasks.Values)
                {
                    TaskEvent te = new TaskEvent(Event.EventTypes.AddTask, task.id, task.addTime, task.workTime);
                    eventsCalendar.AddEvent(te);
                }
            }

            else
            {
                tasks2 = PriorityEventGenerator.GenerateTasks(compSystemParams, 0.7, 10);
                //добавление события постановки в очередь
                foreach (BaseTask task in tasks2.Values)
                {
                    TaskEvent te = new TaskEvent(Event.EventTypes.AddTask, task.id, task.addTime, task.workTime);
                    eventsCalendar.AddEvent(te);
                }
            }
        }

        public void StartSimulation(bool TYPE = true, bool trace = false, double time = 100)
        {
            while (eventsCalendar.EventsCount() > 0)
            {
                
                Event e = eventsCalendar.GetEvent();
                string log = Loging.LogCompSys(compSystemParams);
                log += "\n" + Loging.LogEvent(e as TaskEvent);
                SystemTime = e.beginTimestamp;
                switch (e.type)
                {
                    case Event.EventTypes.AddTask:
                        if(TYPE)
                            tasksQueue.Enqueue(tasks[(e as TaskEvent).taskId]);
                        else
                            tasksQueue2.Add(tasks2[(e as TaskEvent).taskId]);
                        break;

                    case Event.EventTypes.BeginComputeTask:

                        eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.EndComputeTask,
                                                                (e as TaskEvent).taskId,
                                                                e.beginTimestamp + e.duration,
                                                                0)
                                               );
                        if (TYPE)
                        {
                            compSystemParams.nowCoresCount -= tasks[(e as TaskEvent).taskId].requiredCores;
                            compSystemParams.nowMemoryCount -= tasks[(e as TaskEvent).taskId].requiredMemory;
                            if (SystemTime - tasks[(e as TaskEvent).taskId].addTime > MaxTimeInQueue)
                                MaxTimeInQueue = SystemTime - tasks[(e as TaskEvent).taskId].addTime;
                        }
                        else
                        {
                            compSystemParams.nowCoresCount -= tasks2[(e as TaskEvent).taskId].requiredCores;
                            compSystemParams.nowMemoryCount -= tasks2[(e as TaskEvent).taskId].requiredMemory;
                            if (SystemTime - tasks2[(e as TaskEvent).taskId].addTime > MaxTimeInQueue)
                                MaxTimeInQueue = SystemTime - tasks2[(e as TaskEvent).taskId].addTime;
                        }

                        break;

                    case Event.EventTypes.EndComputeTask:
                        if(TYPE)
                            eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.FreeMemory,
                                                                    (e as TaskEvent).taskId,
                                                                    e.beginTimestamp + tasks[(e as TaskEvent).taskId].freeMemoryTime,
                                                                    0)
                                                   );
                        else
                            eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.FreeMemory,
                                                                    (e as TaskEvent).taskId,
                                                                    e.beginTimestamp + tasks2[(e as TaskEvent).taskId].freeMemoryTime,
                                                                    0)
                                                   );

                        break;

                    case Event.EventTypes.FreeMemory:
                        if (TYPE)
                        {
                            compSystemParams.nowCoresCount += tasks[(e as TaskEvent).taskId].requiredCores;
                            compSystemParams.nowMemoryCount += tasks[(e as TaskEvent).taskId].requiredMemory;
                        }
                        else
                        {
                            compSystemParams.nowCoresCount += tasks2[(e as TaskEvent).taskId].requiredCores;
                            compSystemParams.nowMemoryCount += tasks2[(e as TaskEvent).taskId].requiredMemory;
                        }

                        break;
                }

                Loging.WriteLogConsole(log, SystemTime);
                Loging.WriteLogFile(log, SystemTime);
                
                if(TYPE)
                    fifo.fifoQueue(ref tasksQueue, ref eventsCalendar, compSystemParams, SystemTime, e);
                else
                    priorityQueue.priorityQueue(ref tasksQueue2, ref eventsCalendar, compSystemParams, SystemTime, e);
                    
                if (SystemTime >= time)
                {
                    Console.WriteLine("MaxTimeInQueue = " + MaxTimeInQueue.ToString("0.000"));
                    break;
                }
            }
        }
    }
}
