using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    class PriorityQueue
    {
        public PriorityQueue() { }

        public void priorityQueue(ref List<PriorityTask> tasksQueue2, ref EventsCalendar eventsCalendar, CompSystemParams compSystemParams, double SystemTime, Event e, bool trace = false)
        {
            if (tasksQueue2.Count() > 0)
            {
                PriorityTask ts = tasksQueue2[0];
                string log_task = Loging.LogTask(ts);

                if (compSystemParams.isFreeRes(ts))
                {
                    eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.BeginComputeTask,
                                                            ts.id,
                                                            e.beginTimestamp,
                                                            e.duration)
                                           );
                    tasksQueue2.RemoveAt(0);

                }

                else
                {
                    if (!isMaxWaitTime(ts, SystemTime))
                    {
                        for (int i = 1; i < tasksQueue2.Count(); i++)
                        {
                            if (compSystemParams.isFreeRes(tasksQueue2[i]))
                            {
                                Event q = eventsCalendar.GetEvent(tasksQueue2[i].addTime);
                                eventsCalendar.AddEvent(new TaskEvent(Event.EventTypes.BeginComputeTask,
                                                                tasksQueue2[i].id,
                                                                e.beginTimestamp,
                                                                q.duration)
                                                                );
                                e.beginTimestamp = e.beginTimestamp + q.duration;
                                eventsCalendar.AddEvent(e);
                                ts.addTime = e.beginTimestamp + q.duration;
                                tasksQueue2.RemoveAt(0);
                                tasksQueue2.Add(ts);
                                log_task = Loging.LogTask(tasksQueue2[i]);
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
        }

        private bool isMaxWaitTime(PriorityTask ts, double SystemTime)
        {
            if (ts.waitTime < SystemTime - ts.addTime)
                return false;
            else
                return true;
        }
    }
}
