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
        //private Dictionary<int, PriorityTask> tasks;
        //очередь задач
        private Queue<BaseTask> tasksQueue = new Queue<BaseTask>();

        private double SystemTime = 0;
        private double MaxTimeInQueue = 0;

        public Simulation()
        {
            compSystemParams.ReadParamsFromXMLFile();

            //генерируем задачи
            tasks = EventGenerator.GenerateTasks(compSystemParams, 0.7, 10);

            //добавление события постановки в очередь
            foreach (BaseTask task in tasks.Values)
            {
                TaskEvent te = new TaskEvent(Event.EventTypes.AddTask, task.id, task.addTime, task.workTime);
                eventsCalendar.AddEvent(te);
            }
           
        }

        public void StartSimulation(bool trace = false, double time = 100)
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
                        tasksQueue.Enqueue(tasks[(e as TaskEvent).taskId]);
                        //tasksQueue
                        break;

                    case Event.EventTypes.BeginComputeTask:

                        eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.EndComputeTask,
                                                                (e as TaskEvent).taskId,
                                                                e.beginTimestamp + e.duration,
                                                                0)
                                               );
                        compSystemParams.nowCoresCount -= tasks[(e as TaskEvent).taskId].requiredCores;
                        compSystemParams.nowMemoryCount -= tasks[(e as TaskEvent).taskId].requiredMemory;

                        if (SystemTime - tasks[(e as TaskEvent).taskId].addTime > MaxTimeInQueue)
                            MaxTimeInQueue = SystemTime - tasks[(e as TaskEvent).taskId].addTime;

                        break;

                    case Event.EventTypes.EndComputeTask:

                        eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.FreeMemory,
                                                                (e as TaskEvent).taskId,
                                                                e.beginTimestamp + tasks[(e as TaskEvent).taskId].freeMemoryTime,
                                                                0)
                                               );

                        
                        break;

                    case Event.EventTypes.FreeMemory:

                        compSystemParams.nowCoresCount += tasks[(e as TaskEvent).taskId].requiredCores;
                        compSystemParams.nowMemoryCount += tasks[(e as TaskEvent).taskId].requiredMemory;
                        
                        break;
                }

                Loging.WriteLogConsole(log, SystemTime);
                Loging.WriteLogFile(log, SystemTime);

                if (tasksQueue.Count > 0)
                {
                    BaseTask ts = tasksQueue.Peek();

                    string log_task = Loging.LogTask(ts);

                    if (compSystemParams.isFreeRes(ts))
                    {
                        eventsCalendar.AddEvent(new TaskEvent( Event.EventTypes.BeginComputeTask, 
                                                                (e as TaskEvent).taskId,  
                                                                e.beginTimestamp, 
                                                                e.duration) 
                                               );
                        tasksQueue.Dequeue(); 
                    }
                    if (trace)
                    {
                        Loging.WriteLogConsole(log_task, SystemTime, true);
                        
                    }
                        
                    Loging.WriteLogFile(log_task, SystemTime);
                }

                if (SystemTime >= time)
                {
                    Console.WriteLine("MaxTimeInQueue = " + MaxTimeInQueue.ToString("0.000"));
                    break;
                }
            }
        }
    }
}
