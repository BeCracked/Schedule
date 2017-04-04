using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Schedule
{
    /// <summary>
    ///     An always sorted (ealier to later) list of scheduleable objects.
    /// </summary>
    /// <typeparam name="T">has to implement <see cref="IScheduleable" />.</typeparam>
    public class Schedule<T> : List<T> where T : IScheduleable
    {
        /// <summary>
        ///     Compares the <see cref="IScheduleable.Start" /> of two <see cref="T" /> to see which starts earlier.
        /// </summary>
        /// <param name="s1">First <see cref="T" /></param>
        /// <param name="s2">Second <see cref="T" /></param>
        /// <returns>
        ///     -1 if
        ///     <param name="s1"> is earlier.</param>
        ///     0 if equal
        ///     1 if
        ///     <param name="s2"> is earlier.</param>
        /// </returns>
        public static int StartComparison(T s1, T s2)
        {
            if (s1?.Start == null) throw new ArgumentNullException(nameof(s1.Start));
            if (s2?.Start == null) throw new ArgumentNullException(nameof(s2.Start));

            long delta = s1.Start.Subtract(s2.Start).Ticks;
            if (delta == 0) return 0;

            return delta > 0 ? 1 : -1;
        }

        /// <summary>
        ///     Checks if an item can be added to this Schedule. Throws Exception if otherwise.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public new void Add(T item)
        {
            // Catch null
            if (item == null) throw new ArgumentNullException(nameof(item));

            // Check if item does not conflict with this schedule
            if (!IsTimeFrameFree(item.Start, item.End))
                throw new ScheduleConflictException(nameof(item));

            base.Add(item);
            Sort(StartComparison);
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            foreach (T o in collection)
                try
                {
                    Add(o);
                }
                catch (ScheduleConflictException e)
                {
                    Console.WriteLine(e);
                    throw new ScheduleConflictException("An item in the collection conflicted with the schedule.", e);
                }
        }

        /// <summary>
        ///     Checks if nothing is scheduled at a given DateTime.
        /// </summary>
        /// <param name="moment">The point in time that is to be checked.</param>
        /// <returns>True if nothing is scheduled at the given <paramref name="moment" />.</returns>
        public bool IsMomentFree(DateTime moment)
        {
            if (moment == null) throw new ArgumentNullException(nameof(moment));
            return !Exists(x => x.Start < moment && x.End > moment);
        }

        /// <summary>
        ///     Checks if a timeframe is free of scheduled items.
        /// </summary>
        /// <param name="start">Start of the timeframe.</param>
        /// <param name="end">End of the timeframe.</param>
        /// <returns>True if timeframe is free, false else.</returns>
        public bool IsTimeFrameFree(DateTime start, DateTime end)
        {
            // Catch nulls
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (end == null) throw new ArgumentNullException(nameof(end));
            // Catch negative timeframe
            if (start > end) throw new ArgumentOutOfRangeException(nameof(end), "TimeFrame must not be negative!");

            /*
            // Start and end moments have to be free
            if (!(IsMomentFree(start) && IsMomentFree(end))) return false;
            */
            return !Exists(x => x.Start >= start && x.End <= end);
        }

        /// <summary>
        ///     Checks if a timeframe is free of scheduled items.
        /// </summary>
        /// <param name="start">Start of the timeframe.</param>
        /// <param name="duration">Duration of the timeframe.</param>
        /// <returns>True if timeframe is free, false else.</returns>
        public bool IsTimeFrameFree(DateTime start, TimeSpan duration)
        {
            // Catch nulls
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (duration == null) throw new ArgumentNullException(nameof(duration));
            // Catch negative timeframe
            if (duration.Ticks < 0)
                throw new ArgumentOutOfRangeException(nameof(duration), "TimeFrame must not be negative!");

            return IsTimeFrameFree(start, start.Add(duration));
        }

        /// <summary>
        ///     Get the scheduled item at the given <paramref name="moment"></paramref>.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns>The <see cref="T" /> that is scheduled at the given <paramref name="moment" />. Null if there is none.</returns>
        [CanBeNull]
        public T GetScheduled(DateTime moment)
        {
            return Find(x => x.Start <= moment && x.End >= moment);
        }
    }

    [Serializable]
    public class ScheduleConflictException : Exception
    {
        public ScheduleConflictException()
        {
        }

        public ScheduleConflictException(string message) : base(message)
        {
        }

        public ScheduleConflictException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ScheduleConflictException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}