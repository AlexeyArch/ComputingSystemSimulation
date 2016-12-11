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

        private double SystemTime = 0;
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

        public void StartSimulation(bool trace = false,  double time = 100)
        {
            //до тех пор, пока в календаре событий есть события
            while (eventsCalendar.EventsCount() > 0)
            {
                //получаем текущее событие
                Event e = eventsCalendar.GetEvent();
                string log = Loging.LogCompSys(compSystem);
                log += "\n" + Loging.LogEvent(e as TaskEvent);

                //получаем системное время, относительно текущего события
                SystemTime = e.beginTimestamp;

                switch (e.type)
                {
                    //событи добавление задачи в очередь
                    case Event.EventTypes.AddTask:
                            //добавляем задачу в очередь
                            tasksQueue.Add(tasks[(e as TaskEvent).taskId]);
                        break;

                    //событие начала счета задачи
                    case Event.EventTypes.BeginComputeTask:
                        //добавляем событие конца счета
                        eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.EndComputeTask,
                                                                (e as TaskEvent).taskId,
                                                                e.beginTimestamp + e.duration,
                                                                0)
                                               );                  
                        //уменьшем свободные ресурсы
                        compSystem.takeRes(tasks[(e as TaskEvent).taskId].requiredCores, tasks[(e as TaskEvent).taskId].requiredMemory);

                        List<int> freeCores = compSystem.getValueCore(0, tasks[(e as TaskEvent).taskId].requiredCores);
                        for (int i = 0; i < freeCores.Count(); i++)
                            compSystem.setValueCore(freeCores[i], (e as TaskEvent).taskId);

                        tasks[(e as TaskEvent).taskId].setCores(freeCores);



                        //считаем время ожидания задачи в очереди
                        if (SystemTime - tasks[(e as TaskEvent).taskId].addTime > MaxTimeInQueue)
                            MaxTimeInQueue = SystemTime - tasks[(e as TaskEvent).taskId].addTime;
                        break;

                    //событие конца счета
                    case Event.EventTypes.EndComputeTask:
                        //добавления события в календарь освобождения памяти, т.е. событие, которое произойдет когда освободится память
                        eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.FreeMemory,
                                                                    (e as TaskEvent).taskId,
                                                                    e.beginTimestamp + tasks[(e as TaskEvent).taskId].freeMemoryTime,
                                                                    0)
                                                   );
                        break;
                    
                    //событие освобождения памяти
                    case Event.EventTypes.FreeMemory:
                        //освобождает ресурсы                        
                        compSystem.returnRes(tasks[(e as TaskEvent).taskId].requiredCores, tasks[(e as TaskEvent).taskId].requiredMemory);
                        List<int> busyCores = compSystem.getValueCore((e as TaskEvent).taskId);
                        for (int i = 0; i < busyCores.Count(); i++)
                            compSystem.setValueCore(busyCores[i], 0);
                        break;
                }
                log += "\nTask count = " + tasksQueue.Count() + "\n Cores: ";
                for (int i = 0; i < compSystem.coresCount; i++)
                    log += compSystem.cores[i] + " ";
                log += "\n";
                Loging.WriteLogConsole(log, SystemTime, true);

                //если очередь не пуста
                if (tasksQueue.Count() > 0)
                {
                    //получаем первую задачу из очереди
                    BaseTask ts = tasksQueue[0];
                    string log_task = Loging.LogTask(ts);

                    //если хватает ресурсов, то добавляем события начала счета и убираем задачу из очереди
                    if (compSystem.isFreeRes(ts))
                    {
                        Console.WriteLine("\nЗадача из начала очереди");
                        eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.BeginComputeTask,
                                                                ts.id,
                                                                e.beginTimestamp,
                                                                ts.workTime)
                                               );
                        tasksQueue.RemoveAt(0);
                    } else //если ресусов для первой задачи не хватает
                    {
                        //если моделирование с перескоком задач, то проверяем, сколько по времени первая задача уже ожидает в очереди
                        //if (ts.waitTime>0 && (ts.waitTime < SystemTime - ts.addTime))
                        if (compSystem.maxTimeForWait > 0 && (compSystem.maxTimeForWait < SystemTime - ts.addTime))
                        {
                            //перебераем последуюущие задачи, пока не найдет ту, которойй хватит ресурсов
                            for (int i = 1; i < tasksQueue.Count(); i++)
                            {
                                if (compSystem.isFreeRes(tasksQueue[i]))
                                {

                                    Console.WriteLine("\nЗадача перескочила");
                                    eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.BeginComputeTask,
                                                                    tasksQueue[i].id,
                                                                    e.beginTimestamp,
                                                                    tasksQueue[i].workTime)
                                                                    );
                                    tasksQueue.RemoveAt(i);

                                   

                                    break;
                                }
                            }
                        }
                    }

                    if (trace)
                    {
                        Loging.WriteLogConsole(log_task, SystemTime, true);
                    }
                    Loging.WriteLogFile(log_task, SystemTime);
                }

                //выход из цикла при ограничении по времени
                if (SystemTime >= time) break;
            }

            Console.WriteLine("MaxTimeInQueue = " + MaxTimeInQueue.ToString("0.000"));

        }
    }
}
