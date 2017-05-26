using System;
using Microsoft.SPOT;
using GHI.Processor;

namespace SmartLock
{
    public class Utils
    {
        static DateTime error = new DateTime(1955, 11, 12, 6, 38, 00);

        public static DateTime SafeRtc()
        {
            DateTime dt;
            try
            {
                dt = RealTimeClock.GetDateTime();
            }
            catch (Exception)
            {
                dt = error;
            }

            return dt;
        }
    }
}
