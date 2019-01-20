using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesTest
{
    public class DateHelper
    {
        public static string GetTimeDifference(DateTime original, DateTime newDate)
        {
            TimeSpan ts = newDate - original;
            
            if (ts.Days > 0)
            {
                return ts.Days + " days ago";
            } else if (ts.Hours > 0)
            {
                return ts.Hours + " hours ago";
            } else if (ts.Minutes > 0)
            {
                return ts.Minutes + " minutes ago";
            } else if (ts.Seconds > 0)
            {
                return ts.Seconds + " seconds ago";
            }
            else
            {
                return "now";
            }
        }
    }
}
