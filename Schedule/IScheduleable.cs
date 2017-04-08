using System;

namespace Schedule
{
    /// <summary>
    ///     Defines properties that allow a type to be scheduled.
    /// </summary>
    /// <remarks>
    ///     <para>Only <see cref="Start" /> is to be changed via the <see cref="set_Start">Setter</see>.</para>
    ///     <see cref="End" /> and <see cref="Duration" /> should be calculated.
    /// </remarks>
    /// <example>
    ///     <code>
    ///   class ScheduableType : IScheduleable
    ///   {
    ///       public ScheduableType(DateTime start, TimeSpan duration)
    ///           {
    ///                if (duration.Ticks &lt; 0)
    ///                    throw new ArgumentOutOfRangeException(nameof(duration), nameof(duration) + "is negative.");
    ///                Start = start;
    ///                End = Start.Add(duration);
    ///           }
    ///
    ///       public ScheduableType(DateTime start, DateTime end)
    ///       {
    ///           if (end &lt; start)
    ///           throw new ArgumentOutOfRangeException(nameof(end), "Is earlier than " + nameof(start) + ".");
    ///               Start = start;
    ///               End = end;
    ///       }
    ///
    ///       private DateTime _start;
    ///
    ///       public DateTime Start
    ///           {
    ///               get { return _start; }
    ///               set
    ///           {
    ///               _start = value;
    ///               End = _start.Add(Duration);
    ///           }
    ///      }
    ///
    ///      public DateTime End { get; private set; }
    ///      public TimeSpan Duration => End.Subtract(Start);
    ///   }
    ///   </code>
    /// </example>
    public interface IScheduleable
    {
        /// <summary>
        ///     The scheduled start the object.
        /// </summary>
        DateTime Start { get; set; }

        /// <summary>
        ///     The scheduled end of the object
        /// </summary>
        DateTime End { get; }

        /// <summary>
        ///     The scheduled duration of the object.
        /// </summary>
        TimeSpan Duration { get; }
    }
}