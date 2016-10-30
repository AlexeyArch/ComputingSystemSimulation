using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingSystemSimulation
{
    public class EventsCalendar
    {
        private List<Event> events = new List<Event>();
        private EventComparer comparer = new EventComparer();

        public EventsCalendar() { }

        /// <summary>
        /// Добавление события в календарь
        /// </summary>
        /// <param name="e"></param>
        public void AddEvent(Event e)
        {
            events.Add(e);

            //сортировку нужно оптимизировать,
            //при вставке элемента не обязятально сортировать весь список
            events.Sort(comparer);
        }

        /// <summary>
        /// Получение текущего события
        /// </summary>
        public Event GetEvent()
        {
            Event e = events[0];
            events.RemoveAt(0);
            return e; 
        }

        public int EventsCount()
        {
            return events.Count;
        }
    }

    //условие сортировки
    internal class EventComparer : IComparer<Event>
    {
        public int Compare(Event x, Event y)
        {
            return x.beginTimestamp.CompareTo(y.beginTimestamp);
        }
    }
}
