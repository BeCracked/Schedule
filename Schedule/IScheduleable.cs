using System;

namespace Schedule
{
    public interface IScheduleable
    {
        DateTime Start { get; set; }

        DateTime End { get; }

        TimeSpan Duration { get; }
    }
}
