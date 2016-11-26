using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    class QueueOfTasks
    {
        private List<PriorityTask> events = new List<PriorityTask>();
        private TaskComparer comparer = new TaskComparer();

        public QueueOfTasks() { }

        public void AddTask(PriorityTask e)
        {
            events.Add(e);
            //events.Sort(comparer);
        }

        public void Sorting()
        {
            events.Sort(comparer);
        }

        public PriorityTask GetTask()
        {
            PriorityTask e = events[0];
            events.RemoveAt(0);
            return e;
        }

        public int TasksCount()
        {
            return events.Count;
        }
    }

    internal class TaskComparer : IComparer<PriorityTask>
    {
        public int Compare(PriorityTask x, PriorityTask y)
        {
            return x.priority.CompareTo(y.priority);
        }
    }
}
