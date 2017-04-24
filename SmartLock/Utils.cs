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
    }
}
