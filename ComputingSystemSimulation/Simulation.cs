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
                eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.AddTask, task.id, task.addTime, task.workTime));
        }

        public void StartSimulation()
        {
            while (eventsCalendar.EventsCount() > 0)
            {
                //получение ближайшего события
                Event e = eventsCalendar.GetEvent();
                switch(e.type)
                {
                    case Event.EventTypes.AddTask:
                        tasksQueue.Enqueue(tasks[(e as TaskEvent).taskId]);
                        break;
                }
            }
        }
    }
}
