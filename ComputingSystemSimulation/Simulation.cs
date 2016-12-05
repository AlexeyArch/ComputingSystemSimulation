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
        private List<BaseTask> tasksQueue = new List<BaseTask>();

        private double SystemTime = 0;
        private double MaxTimeInQueue = 0;
        private double MaxTimeWait; // максимальное время ожидания задачи в очереди
        private int rows = 2, cols = 1; // для записи в exel

        public Simulation(double _MaxTimeWait = 0.0)
        {
            compSystemParams.ReadParamsFromXMLFile();
            MaxTimeWait = _MaxTimeWait;
            //генерируем задачи
            tasks = EventGenerator.GenerateTasks(compSystemParams, 0.7, 10, (MaxTimeWait > 0)? true:false);
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
                string log = Loging.LogCompSys(compSystemParams);
                log += "\n" + Loging.LogEvent(e as TaskEvent);

                Loging.WriteLogFile(log, SystemTime);

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
                        compSystemParams.takeRes(tasks[(e as TaskEvent).taskId].requiredCores, tasks[(e as TaskEvent).taskId].requiredMemory);
                        //считаем время ожидания задачи в очереди
                        if (SystemTime - tasks[(e as TaskEvent).taskId].addTime > MaxTimeInQueue)
                            MaxTimeInQueue = SystemTime - tasks[(e as TaskEvent).taskId].addTime;

                        //запись в excel файл времени пибывания в очереди
                        Loging.writingInExcMethod(rows, cols, tasks[(e as TaskEvent).taskId].id, SystemTime - tasks[(e as TaskEvent).taskId].addTime);
                        rows++;

                        //запись лога в файл
                        string log_task = Loging.LogTask(tasks[(e as TaskEvent).taskId]);
                        Loging.WriteLogFile(log_task, SystemTime);

                        string SizeOfQueue = "Size of queue = " + tasksQueue.Count().ToString();
                        Loging.WriteLogFile(SizeOfQueue, SystemTime); 

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
                        compSystemParams.returnRes(tasks[(e as TaskEvent).taskId].requiredCores, tasks[(e as TaskEvent).taskId].requiredMemory);
                        break;
                }

               // Loging.WriteLogConsole(log, SystemTime, true);

                //если очередь не пуста
                if (tasksQueue.Count() > 0)
                {
                    //получаем первую задачу из очереди
                    BaseTask ts = tasksQueue[0];
                    string log_task = Loging.LogTask(ts);

                    //если хватает ресурсов, то добавляем события начала счета и убираем задачу из очереди
                    if (compSystemParams.isFreeRes(ts))
                    {
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
                        if (MaxTimeWait > 0 && (MaxTimeWait < SystemTime - ts.addTime))
                        {
                            //перебераем последуюущие задачи, пока не найдет ту, которойй хватит ресурсов
                            for (int i = 1; i < tasksQueue.Count(); i++)
                            {
                                if (compSystemParams.isFreeRes(tasksQueue[i]))
                                {
                                    eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.BeginComputeTask,
                                                                    tasksQueue[i].id,
                                                                    e.beginTimestamp,
                                                                    tasksQueue[i].workTime)
                                                                    );


                                    log_task = Loging.LogTask(tasksQueue[i]);

                                    break;
                                }
                            }
                        }
                    }

                    if (trace)
                    {
                        Loging.WriteLogConsole(log_task, SystemTime, true);
                    }
                    
                }

                //выход из цикла при ограничении по времени
                if (SystemTime >= time) break;
            }

            Console.WriteLine("MaxTimeInQueue = " + MaxTimeInQueue.ToString("0.000"));

        }
    }
}
