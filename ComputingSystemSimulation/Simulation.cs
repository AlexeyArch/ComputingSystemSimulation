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
        private CompSystem compSystem = new CompSystem();
        //календарь событий
        private EventsCalendar eventsCalendar = new EventsCalendar();
        //задачи
        private Dictionary<int, BaseTask> tasks;
        //очередь задач
        private List<BaseTask> tasksQueue = new List<BaseTask>();

        private double MaxTimeInQueue = 0;

        public Simulation()
        {
            //генерируем задачи
            tasks = EventGenerator.GenerateTasks(compSystem);
            //добавление события постановки в очередь
            foreach (BaseTask task in tasks.Values)
            {
               TaskEvent te = new TaskEvent(Event.EventTypes.AddTask, task.id, task.addTime, task.workTime);
               eventsCalendar.AddEvent(te);
            }
        }

        private bool AddBeginComputeTaskEvent(BaseTask task, double beginTimestamp, int index)
        {
            //если хватает ресурсов, то добавляем события начала счета и убираем задачу из очереди
            if (compSystem.isFreeRes(task))
            {
                eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.BeginComputeTask,
                                                                  task.id,
                                                                  beginTimestamp,
                                                                  task.workTime)
                                       );
                tasksQueue.RemoveAt(0);
                return true;
            }
            else
                return false;

        }

        public void StartSimulation(bool trace = false)
        {
            //до тех пор, пока в календаре событий есть события
            while (eventsCalendar.EventsCount() > 0)
            {
                //получаем текущее событие
                Event e = eventsCalendar.GetEvent();
                //получаем время относительно текущего события
                double currentTime = e.beginTimestamp;
                int taskId = (e as TaskEvent).taskId;

                #region Log
                string log = Loging.LogCompSys(compSystem);
                log += "\n" + Loging.LogEvent(e as TaskEvent);
                #endregion

                switch (e.type)
                {
                    #region AddTask
                    //событие добавление задачи в очередь
                    case Event.EventTypes.AddTask:
                        //добавляем задачу в очередь
                        tasksQueue.Add(tasks[taskId]);
                        break;
                    #endregion

                    #region BeginComputeTask
                    //событие начала счета задачи
                    case Event.EventTypes.BeginComputeTask:
                        //добавляем событие конца счета
                        eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.EndComputeTask,
                                                              taskId,
                                                              e.beginTimestamp + e.duration,
                                                              0)
                                               );                  
                        //уменьшаем свободные ресурсы
                        compSystem.takeRes(tasks[taskId]);

                        //считаем время ожидания задачи в очереди
                        if (currentTime - tasks[taskId].addTime > MaxTimeInQueue)
                            MaxTimeInQueue = currentTime - tasks[taskId].addTime;
                        break;
                    #endregion

                    #region EndComputeTask
                    //событие конца счета
                    case Event.EventTypes.EndComputeTask:
                        //добавления события в календарь освобождения памяти, т.е. событие, которое произойдет когда освободится память
                        eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.FreeMemory,
                                                              taskId,
                                                              e.beginTimestamp + tasks[taskId].freeMemoryTime,
                                                              0)
                                               );
                        break;
                    #endregion

                    #region FreeMemory
                    //событие освобождения памяти
                    case Event.EventTypes.FreeMemory:
                        //освобождает ресурсы                        
                        compSystem.returnRes(tasks[taskId]);
                        break;
                    #endregion
                }


                #region Log
                log += "\nTask count = " + tasksQueue.Count() + "\n Cores: ";
                for (int i = 0; i < compSystem.coresCount; i++)
                    log += compSystem.cores[i] + " ";
                log += "\n";
                Loging.WriteLogConsole(log, currentTime, true);
                #endregion

                //если очередь не пуста
                if (tasksQueue.Count() > 0)
                {
                    //получаем первую задачу из очереди
                    BaseTask ts = tasksQueue[0];

                        
                    if (AddBeginComputeTaskEvent(ts, e.beginTimestamp, 0))
                        Console.WriteLine("\nЗадача из начала очереди");
                    else //если ресусов для первой задачи не хватает
                    {
                        //если моделирование с перескоком задач, то проверяем, сколько по времени первая задача уже ожидает в очереди
                        //if (ts.waitTime>0 && (ts.waitTime < SystemTime - ts.addTime))
                        if (compSystem.maxTimeForWait > 0 && (compSystem.maxTimeForWait < currentTime - ts.addTime))
                        {
                            //перебераем последуюущие задачи, пока не найдет ту, которой хватит ресурсов
                            for (int i = 1; i < tasksQueue.Count(); i++)
                            {
                                if (AddBeginComputeTaskEvent(tasksQueue[i], e.beginTimestamp, i))
                                {
                                    Console.WriteLine("\nЗадача перескочила");
                                    break;
                                }
                            }
                        }
                    }

                    #region Log
                    string log_task = Loging.LogTask(ts);
                    if (trace)
                    {
                        Loging.WriteLogConsole(log_task, currentTime, true);
                    }
                    Loging.WriteLogFile(log_task, currentTime);
                    #endregion
                }

                //выход из цикла при ограничении по времени
                if (currentTime >= compSystem.simulationTimeLimit) break;
            }

            Console.WriteLine("MaxTimeInQueue = " + MaxTimeInQueue.ToString("0.000"));
        }
    }
}
