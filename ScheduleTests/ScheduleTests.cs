using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScheduleTests;

namespace Schedule.Tests
{
    [TestClass]
    public class ScheduleTests
    {
        [TestMethod]
        public void IsMomentFreeTest()
        {
            #region EmptySchedule

            Schedule<ScheduableType> schedule = new Schedule<ScheduableType>();

            Assert.IsTrue(schedule.IsMomentFree(new DateTime()));
            Assert.IsTrue(schedule.IsMomentFree(new DateTime().Date));
            Assert.IsTrue(schedule.IsMomentFree(DateTime.MinValue));
            Assert.IsTrue(schedule.IsMomentFree(DateTime.MaxValue));
            Assert.IsTrue(schedule.IsMomentFree(DateTime.Today));
            Assert.IsTrue(schedule.IsMomentFree(DateTime.Now));

            #endregion
        }

        [TestMethod]
        public void StartComparisonTest()
        {
            //Earlier
            ScheduableType s1 = new ScheduableType(DateTime.Now, DateTime.Now.AddHours(1));
            //Later
            ScheduableType s2 = new ScheduableType(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2));

            //two same
            Assert.IsTrue(0 == Schedule<ScheduableType>.StartComparison(s1, s1));

            //Second earlier
            Assert.IsTrue(1 == Schedule<ScheduableType>.StartComparison(s2, s1));

            //First earlier
            Assert.IsTrue(-1 == Schedule<ScheduableType>.StartComparison(s1, s2));

            #region Exceptions

            try
            {
                Schedule<ScheduableType>.StartComparison(null, s1);
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                Schedule<ScheduableType>.StartComparison(s1, null);
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                Schedule<ScheduableType>.StartComparison(null, null);
            }
            catch (ArgumentNullException)
            {
            }

            #endregion
        }

        [TestMethod]
        public void IsTimeFrameFreeTest()
        {
            Schedule<ScheduableType> schedule = new Schedule<ScheduableType>();

            //negative TimeFrame
            try
            {
                schedule.IsTimeFrameFree(DateTime.Now.AddHours(1), DateTime.Now);
            }
            catch (ArgumentOutOfRangeException)
            {
            }


            // Empty Schedule
            Assert.IsTrue(schedule.IsTimeFrameFree(DateTime.Now, DateTime.Now.AddHours(1)));


            /*
                        //Earlier
                        ScheduableType s1 = new ScheduableType(DateTime.Now, DateTime.Now.AddHours(1));
                        //Later
                        ScheduableType s2 = new ScheduableType(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2));
                        */
        }
    }
}