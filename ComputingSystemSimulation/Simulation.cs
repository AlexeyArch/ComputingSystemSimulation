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

            //генерируем отказы
            List<CrashEvent> crashes = EventGenerator.GenerateCrashes(compSystem);
            foreach (CrashEvent crash in crashes)
                eventsCalendar.AddEvent(crash);
        }

        private bool TryAddBeginComputeTaskEvent(BaseTask task, double beginTimestamp, int index)
        {
            //если хватает ресурсов, то добавляем события начала счета и убираем задачу из очереди
            if (compSystem.IsFreeRes(task))
            {
                //уменьшаем свободные ресурсы
                compSystem.TakeRes(task);

                eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.BeginComputeTask,
                                                                  task.id,
                                                                  beginTimestamp,
                                                                  task.workTime)
                                       );
                tasksQueue.RemoveAt(index);
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

                #region Log
                string log = Loging.LogCompSys(compSystem);
                log += "\n" + e.LogEvent();
                #endregion

                switch (e.type)
                {
                    #region AddTask
                    //событие добавление задачи в очередь
                    case Event.EventTypes.AddTask:
                        //добавляем задачу в очередь
                        tasksQueue.Add(tasks[(e as TaskEvent).taskId]);
                        break;
                    #endregion

                    #region BeginComputeTask
                    //событие начала счета задачи
                    case Event.EventTypes.BeginComputeTask:
                        //добавляем событие конца счета
                        eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.EndComputeTask,
                                                              (e as TaskEvent).taskId,
                                                              e.beginTimestamp + e.duration,
                                                              0)
                                               );                  

                        //считаем время ожидания задачи в очереди
                        if (currentTime - tasks[(e as TaskEvent).taskId].addTime > MaxTimeInQueue)
                            MaxTimeInQueue = currentTime - tasks[(e as TaskEvent).taskId].addTime;
                        break;
                    #endregion

                    #region EndComputeTask
                    //событие конца счета
                    case Event.EventTypes.EndComputeTask:
                        //добавления события в календарь освобождения памяти, т.е. событие, которое произойдет когда освободится память
                        eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.FreeMemory,
                                                              (e as TaskEvent).taskId,
                                                              e.beginTimestamp + tasks[(e as TaskEvent).taskId].freeMemoryTime,
                                                              0)
                                               );
                        break;
                    #endregion

                    #region FreeMemory
                    //событие освобождения памяти
                    case Event.EventTypes.FreeMemory:
                        //освобождает ресурсы                        
                        compSystem.ReturnRes(tasks[(e as TaskEvent).taskId]);
                        break;
                    #endregion

                    #region CrashCore
                    //событие поломки ядра
                    case Event.EventTypes.CrashCore:
                        int coreId = compSystem.CrashCore();
                        Console.WriteLine("\nПоломка ядра " + coreId.ToString());
                        //если ядро выполняло не нулевую задачу
                        if (compSystem.GetTaskId2Core(coreId) != 0) eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.CancelTask,
                                                                                               compSystem.GetTaskId2Core(coreId),
                                                                                               e.beginTimestamp,
                                                                                            0)
                                                                                             );
                    
                        eventsCalendar.AddEvent(new RecoveryEvent(coreId,
                                                                  e.beginTimestamp +
                                                                  Utils.ExponentialDistr(compSystem.recoveryIntensity,
                                                                                         new Random().NextDouble())));
                        break;
                    #endregion

                    #region RecoveryCore
                    //событие поломки ядра
                    case Event.EventTypes.RecoveryCore:
                        Console.WriteLine("\nВосстановление ядра " + (e as RecoveryEvent).coreId.ToString());
                        compSystem.RecoveryCore((e as RecoveryEvent).coreId);
                        break;
                    #endregion

                    #region CancelTask
                    //событие отмена задачи
                    case Event.EventTypes.CancelTask:
                        Console.WriteLine("\nОтмена задачи " + (e as TaskEvent).taskId.ToString());
               
                        eventsCalendar.CancelTask((e as TaskEvent));
                        eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.FreeMemory,
                                                              (e as TaskEvent).taskId,
                                                              e.beginTimestamp + tasks[(e as TaskEvent).taskId].freeMemoryTime,
                                                              0)
                                               );
                        break;
                        #endregion
                }

                #region Log
                log += "\nTask count = " + tasksQueue.Count() + "\n Cores: ";
                foreach (CompSystem.Core core in compSystem.workingCores)
                    log += core.ToString() + " ";
                log += "\n";
                Loging.WriteLogConsole(log, currentTime, false);
                #endregion

                //если очередь не пуста
                if (tasksQueue.Count() == 0)
                    continue;

                BaseTask ts = tasksQueue[0];

                if (TryAddBeginComputeTaskEvent(ts, e.beginTimestamp, 0))
                {
                    Console.WriteLine("\nЗадача из начала очереди");
                    continue;
                }
  
                //если моделирование с перескоком задач, то проверяем, сколько по времени первая задача уже ожидает в очереди
                //if (ts.waitTime>0 && (ts.waitTime < SystemTime - ts.addTime))
                if (compSystem.maxTimeForWait > 0 && (compSystem.maxTimeForWait < currentTime - ts.addTime))
                {
                    //перебераем последуюущие задачи, пока не найдет ту, которой хватит ресурсов
                    for (int i = 1; i < tasksQueue.Count(); i++)
                    {
                        if (TryAddBeginComputeTaskEvent(tasksQueue[i], e.beginTimestamp, i))
                        {
                            Console.WriteLine("\nЗадача перескочила");
                            break;
                        }
                    }
                }
                    
                #region Log
                string log_task = Loging.LogTask(ts);
                if (trace)
                    Loging.WriteLogConsole(log_task, currentTime, true);
                Loging.WriteLogFile(log_task, currentTime);
                #endregion
            }

            Console.WriteLine("MaxTimeInQueue = " + MaxTimeInQueue.ToString("0.000"));
        }
    }
}
