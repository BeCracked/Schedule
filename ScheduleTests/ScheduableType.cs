using System;
using Schedule;

namespace ScheduleTests
{
    internal class ScheduableType : IScheduleable
    {
        public ScheduableType(DateTime start, TimeSpan duration)
        {
            if (duration.Ticks < 0)
                throw new ArgumentOutOfRangeException(nameof(duration), nameof(duration) + "is negative.");
            Start = start;
            End = Start.Add(duration);
        }

        public ScheduableType(DateTime start, DateTime end)
        {
            if (end < start)
                throw new ArgumentOutOfRangeException(nameof(end), "Is earlier than " + nameof(start) + ".");
            Start = start;
            End = end;
        }

        private DateTime _start;

        public DateTime Start
        {
            get { return _start; }
            set
            {
                _start = value;
                End = _start.Add(Duration);
            }
        }

        public DateTime End { get; private set; }
        public TimeSpan Duration => End.Subtract(Start);
    }
}