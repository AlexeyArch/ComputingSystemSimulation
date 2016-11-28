using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    class FIFOqueue
    {
        public FIFOqueue() { }

        public void fifoQueue(ref Queue<BaseTask> tasksQueue, ref EventsCalendar eventsCalendar, CompSystemParams compSystemParams, double SystemTime, Event e, bool trace = false)
        {
            if (tasksQueue.Count() > 0)
            {
                BaseTask ts = tasksQueue.Peek();
                string log_task = Loging.LogTask(ts);

                if (compSystemParams.isFreeRes(ts))
                {
                    eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.BeginComputeTask,
                                                            ts.id,
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
        }
    }
}
