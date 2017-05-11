using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Schedule
{
    /// <summary>
    ///     An always sorted (earlier to later) list of scheduleable objects.
    /// </summary>
    /// <typeparam name="T">has to implement <see cref="IScheduleable" />.</typeparam>
    public class Schedule<T> : List<T> where T : IScheduleable
    {
        //ToDo refactor this to a function since it may throw an exception.
        /// <summary>
        ///     The <see cref="IScheduleable.End">End</see> of the last <see cref="IScheduleable" />.
        /// </summary>
        /// <exception cref="NullReferenceException">Thrown when no <see cref="IScheduleable" /> is scheduled.</exception>
        public DateTime End
        {
            get
            {
                T lastOrDefault = this.LastOrDefault();
                if (lastOrDefault != null) return lastOrDefault.End;
                throw new NullReferenceException("Nothing is scheduled.");
            }
        }

        /// <summary>
        ///     Compares the <see cref="IScheduleable.Start" /> of two <see cref="IScheduleable" /> to see which starts earlier.
        /// </summary>
        /// <param name="s1">The first <see cref="IScheduleable" />.</param>
        /// <param name="s2">The second <see cref="IScheduleable" />.</param>
        /// <returns>
        ///     <para>-1 if <paramref name="s1" /> is earlier.</para>
        ///     <para>0 if equal.</para>
        ///     1 if <paramref name="s2" /> is earlier.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when any <paramref name="s1" />, <paramref name="s2" /> or their
        ///     <see cref="IScheduleable.Start">Start</see> is null.
        /// </exception>
        public static int StartComparison(T s1, T s2)
        {
            if (s1?.Start == null) throw new ArgumentNullException(nameof(s1.Start));
            if (s2?.Start == null) throw new ArgumentNullException(nameof(s2.Start));

            long delta = s1.Start.Subtract(s2.Start).Ticks;
            if (delta == 0) return 0;

            return delta > 0 ? 1 : -1;
        }

        /// <summary>
        ///     Checks if an item can be added to this <see cref="Schedule{T}" />. Throws Exception if otherwise.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item" /> is null.</exception>
        /// <exception cref="ScheduleConflictException">
        ///     Thrown when <paramref name="item" /> conflicts with an item in this <see cref="Schedule{T}" />.
        /// </exception>
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

        /// <summary>
        ///     Tries to add <paramref name="collection" /> to the
        /// </summary>
        /// <param name="collection">The collection to be added to this <see cref="Schedule{T}" />.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection" /> is null.</exception>
        /// <exception cref="ScheduleConflictException">
        ///     Thrown when an item in this schedule is conflicting with
        ///     <paramref name="collection" />.
        /// </exception>
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
        ///     Checks if a time frame is free of scheduled items.
        /// </summary>
        /// <param name="start">Start of the time frame.</param>
        /// <param name="end">End of the time frame.</param>
        /// <returns>True if time frame is free, false else.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="start" /> or <paramref name="end" /> are null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the time frame is negative.</exception>
        public bool IsTimeFrameFree(DateTime start, DateTime end)
        {
            // Catch nulls
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (end == null) throw new ArgumentNullException(nameof(end));
            // Catch negative time frame
            if (start > end) throw new ArgumentOutOfRangeException(nameof(end), "TimeFrame must not be negative!");

            /*
            // Start and end moments have to be free
            if (!(IsMomentFree(start) && IsMomentFree(end))) return false;
            */
            return !Exists(x => x.Start >= start && x.End <= end);
        }

        /// <summary>
        ///     Checks if a time frame is free of scheduled items.
        /// </summary>
        /// <param name="start">Start of the time frame.</param>
        /// <param name="duration">Duration of the time frame.</param>
        /// <returns>True if time frame is free, false else.</returns>
        public bool IsTimeFrameFree(DateTime start, TimeSpan duration)
        {
            // Catch nulls
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (duration == null) throw new ArgumentNullException(nameof(duration));
            // Catch negative time frame
            if (duration.Ticks < 0)
                throw new ArgumentOutOfRangeException(nameof(duration), "TimeFrame must not be negative!");

            return IsTimeFrameFree(start, start.Add(duration));
        }

        /// <summary>
        ///     Get the scheduled item at the given <paramref name="moment"></paramref>.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns>
        ///     The <see cref="IScheduleable" /> that is scheduled at the given <paramref name="moment" />. Null if there is
        ///     none.
        /// </returns>
        [CanBeNull]
        public T GetScheduled(DateTime moment)
        {
            return Find(x => x.Start <= moment && x.End >= moment);
        }

        #region Obsolete

        /// <summary>
        ///     Not supported!
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete]
        public new void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Not supported!
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete]
        public void Insert(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Not supported!
        /// </summary>
        /// <param name="index"></param>
        /// <param name="collection"></param>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete]
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    /// <summary>
    ///     This exception class is thrown if a unhandled schedule conflict occurred.
    /// </summary>
    [Serializable]
    public class ScheduleConflictException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ScheduleConflictException"/> class.
        /// </summary>
        public ScheduleConflictException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScheduleConflictException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public ScheduleConflictException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScheduleConflictException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public ScheduleConflictException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScheduleConflictException"/> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ScheduleConflictException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}