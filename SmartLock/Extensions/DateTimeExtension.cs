using System;
using Microsoft.SPOT;
using SmartLock;

namespace DateTimeExtension
{
    public static class DateTimeExtension
    {
        public static string ToMyString(this DateTime dt)
        {
            return dt.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public static DateTime ToDateTime(this string inputString) 
        {
            try
            {
                int year = UInt16.Parse(inputString.Substring(9, 4));
                int month = UInt16.Parse(inputString.Substring(5, 2));
                int day = UInt16.Parse(inputString.Substring(1, 2));
                int hour = UInt16.Parse(inputString.Substring(14, 2));
                int minute = UInt16.Parse(inputString.Substring(17, 2));
                int second = UInt16.Parse(inputString.Substring(20, 2));

                return new DateTime(year, month, day, hour, minute, second);
            }
            catch (Exception e)
            {
                DebugOnly.Print("ERROR: Exception while parsing datetime: " + e);
            }

            return DateTime.MinValue;
        }

        public static bool WeakCompare(this DateTime dt1, DateTime dt2)
        {
            // Check date
            if (dt1.Day != dt2.Day) return false;
            if (dt1.Month != dt2.Month) return false;
            if (dt1.Year != dt2.Year) return false;

            // Check time
            if (dt1.Hour != dt2.Hour) return false;
            if (dt1.Minute != dt2.Minute) return false;
            if (dt1.Second != dt2.Second) return false;

            return true;

        }
    }
}
