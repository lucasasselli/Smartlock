using System;
using System.Collections;

namespace SmartLock
{
    class Utils
    {
        public static void ArrayListCopy(ArrayList from, ArrayList to)
        {
            to.Clear();
            foreach (Object elem in from)
            {
                to.Add(elem);
            }
        }

        public static void ArrayToList(object[] array, ArrayList arraylist)
        {
            arraylist.Clear();
            foreach (object obj in array)
            {
                arraylist.Add(obj);
            }
        }

        public static DateTime StringToDateTime(string inputString)
        {
            // Date format is fine
            int year = UInt16.Parse(inputString.Substring(9, 4));
            int month = UInt16.Parse(inputString.Substring(5, 2));
            int day = UInt16.Parse(inputString.Substring(1, 2));
            int hour = UInt16.Parse(inputString.Substring(14, 2));
            int minute = UInt16.Parse(inputString.Substring(17, 2));
            int second = UInt16.Parse(inputString.Substring(20, 2));

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}
