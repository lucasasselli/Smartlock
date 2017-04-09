using System;
using System.Collections;
using System.Reflection;

using Microsoft.SPOT;

using Json.NETMF;

namespace SmartLock
{
    class Json
    {
        public static bool ParseNamedArray(string arrayName, string jsonStr, ArrayList list, Type objType)
        {
            Hashtable initHash = JsonSerializer.DeserializeString(jsonStr) as Hashtable;

            if (initHash == null)
            {
                Debug.Print("ERROR: Not JSON!");
                return false;
            }

            ArrayList hashList = (ArrayList)initHash[arrayName];

            if (hashList == null)
            {
                Debug.Print("ERROR: JSON doesn't contain array \"" + arrayName + "\"!");
                return false;
            }

            try
            {
                foreach (Hashtable hash in hashList)
                {
                    MethodInfo[] methods = objType.GetMethods();

                    // Construct new object of type objType
                    Object obj = objType.GetConstructor(new Type[0]).Invoke(new Object[0]);

                    foreach (MethodInfo method in methods)
                    {
                        string methodName = method.Name;
                        if (methodName.Length > 5)
                        {
                            string prefix = methodName.Substring(0, 4);
                            if (String.Compare(prefix, "set_") == 0)
                            {
                                // Is a setter
                                string fieldName = methodName.Substring(4);
                                object objFromHash = hash[fieldName];
                                if (objFromHash != null)
                                {
                                    method.Invoke(obj, new object[] { objFromHash });
                                }
                            }
                        }
                    }

                    list.Add(obj);
                }
            }
            catch (Exception e)
            {
                Debug.Print("Error parsing JSON list!");
                return false;
            }

            return true;
        }

        public static string BuildNamedArray(string arrayName, ArrayList list)
        {
            Hashtable hash = new Hashtable();
            hash.Add(arrayName, list);

            return JsonSerializer.SerializeObject(hash);

        }
    }
}
