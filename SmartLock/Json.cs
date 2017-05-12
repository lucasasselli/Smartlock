using System;
using System.Collections;
using Json.NETMF;
using Microsoft.SPOT;

namespace SmartLock
{
    internal class Json
    {
        public static bool ParseNamedArray(string arrayName, string jsonStr, ArrayList list, Type objType)
        {
            var initHash = JsonSerializer.DeserializeString(jsonStr) as Hashtable;

            if (initHash == null)
            {
                DebugOnly.Print("ERROR: Not JSON!");
                return false;
            }

            var hashList = (ArrayList) initHash[arrayName];

            if (hashList == null)
            {
                DebugOnly.Print("ERROR: JSON doesn't contain array \"" + arrayName + "\"!");
                return false;
            }

            try
            {
                foreach (Hashtable hash in hashList)
                {
                    var methods = objType.GetMethods();

                    // Construct new object of type objType
                    var constructorInfo = objType.GetConstructor(new Type[0]);
                    if (constructorInfo != null)
                    {
                        var obj = constructorInfo.Invoke(new object[0]);

                        foreach (var method in methods)
                        {
                            var methodName = method.Name;
                            if (methodName.Length > 5)
                            {
                                var prefix = methodName.Substring(0, 4);
                                if (string.Compare(prefix, "set_") == 0)
                                {
                                    // Is a setter
                                    var fieldName = methodName.Substring(4);
                                    var objFromHash = hash[fieldName];
                                    if (objFromHash != null)
                                        method.Invoke(obj, new[] {objFromHash});
                                }
                            }
                        }

                        list.Add(obj);
                    }
                }
            }
            catch (Exception e)
            {
                DebugOnly.Print("ERROR: exception while parsing JSON list: " + e);
                return false;
            }

            return true;
        }

        public static string BuildNamedArray(string arrayName, ArrayList list)
        {
            var hash = new Hashtable();
            hash.Add(arrayName, list);

            return JsonSerializer.SerializeObject(hash);
        }
    }
}