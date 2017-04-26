using System;
using System.Collections;
using Microsoft.SPOT;

namespace ArrayListExtension
{
    public static class ArrayListExtension
    {
        public static void CopyFrom(this ArrayList to, ArrayList from)
        {
            to.Clear();
            foreach (Object elem in from)
            {
                to.Add(elem);
            }
        }

        public static ArrayList ToArrayList(this object[] array)
        {
            ArrayList arrayList = new ArrayList();
            foreach (object obj in array)
            {
                arrayList.Add(obj);
            }

            return arrayList;
        }
    }
}
