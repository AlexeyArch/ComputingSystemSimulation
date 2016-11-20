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
        //очередь задач
        private Queue<BaseTask> tasksQueue = new Queue<BaseTask>();

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

        public void StartSimulation()
        {
            while (eventsCalendar.EventsCount() > 0)
            {
                
                Event e = eventsCalendar.GetEvent();
                string log = Loging.LogCompSys(compSystemParams);
                log += "\n" + Loging.LogEvent(e as TaskEvent);
                switch(e.type)
                {
                    case Event.EventTypes.AddTask:
                        tasksQueue.Enqueue(tasks[(e as TaskEvent).taskId]);
                        break;

                    case Event.EventTypes.BeginComputeTask:

                        eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.EndComputeTask,
                                                                (e as TaskEvent).taskId,
                                                                e.beginTimestamp + e.duration,
                                                                e.duration)
                                               );
                        compSystemParams.nowCoresCount -= tasks[(e as TaskEvent).taskId].requiredCores;
                        compSystemParams.nowMemoryCount -= tasks[(e as TaskEvent).taskId].requiredMemory;

                        break;

                    case Event.EventTypes.EndComputeTask:

                        eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.FreeMemory,
                                                                (e as TaskEvent).taskId,
                                                                e.beginTimestamp + tasks[(e as TaskEvent).taskId].freeMemoryTime,
                                                                e.duration)
                                               );

                        
                        break;

                    case Event.EventTypes.FreeMemory:

                        compSystemParams.nowCoresCount += tasks[(e as TaskEvent).taskId].requiredCores;
                        compSystemParams.nowMemoryCount += tasks[(e as TaskEvent).taskId].requiredMemory;
                        
                        break;
                }

                if (tasksQueue.Count > 0)
                {
                    BaseTask ts = tasksQueue.Peek();

                    if (compSystemParams.isFreeRes(ts))
                    {
                        eventsCalendar.AddEvent(new TaskEvent( Event.EventTypes.BeginComputeTask, 
                                                                (e as TaskEvent).taskId,  
                                                                e.beginTimestamp, 
                                                                e.duration) 
                                               );
                        tasksQueue.Dequeue();
                    }

                }
                
                Loging.WriteLogConsole(log, true);
                Loging.WriteLogFile (log);

            }
        }
    }
}
