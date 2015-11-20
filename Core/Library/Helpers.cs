using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF.Time;

namespace Clues.Library
{
    public static class Helpers
    {
        /// <summary>
        /// Provides an IEnumerable of AFTimeRange over a period specified by start time and end time.  The last value will never be greater than EndTime.
        /// </summary>
        /// <param name="startTime">Period beginning</param>
        /// <param name="endTime">Period End</param>
        /// <param name="days">The time range will be chunked into the number of days specified.</param>
        public static IEnumerable<AFTimeRange> EachNDay(DateTime startTime, DateTime endTime, int days)
        {
            for (var day = startTime.Date; day.Date <= endTime.Date; day = day.AddDays(days))
            {
                var et = day.AddDays(days) <= endTime ? day.AddDays(days) : endTime;
                var res = new AFTimeRange(day, et);

                yield return res;
            }
        }
    }
}
