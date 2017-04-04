using System;
using System.Collections.Generic;

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
            Sort(StartComparison);
            base.Add(item);
        }

        /// <summary>
        ///     Checks if nothing is scheduled at a given DateTime.
        /// </summary>
        /// <param name="moment">The point in time that is to be checked.</param>
        /// <returns>True if nothing is scheduled at the given <paramref name="moment" />.</returns>
        public bool IsMomentFree(DateTime moment)
        {
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
            //ToDo implement
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Checks if a timeframe is free of scheduled items.
        /// </summary>
        /// <param name="start">Start of the timeframe.</param>
        /// <param name="duration">Duration of the timeframe.</param>
        /// <returns>True if timeframe is free, false else.</returns>
        public bool IsTimeFrameFree(DateTime start, TimeSpan duration)
        {
            //ToDo implement
            throw new NotImplementedException();
        }
    }
}