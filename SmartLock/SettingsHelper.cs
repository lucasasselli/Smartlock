using System;
using System.Collections;
using Microsoft.SPOT;


namespace SmartLock
{
    internal static class SettingsHelper
    {
        // Settings
        public const int ServerIp = 0;
        public const int ServerPort = 1;
        public const int LockId = 2;
        public const int RequestPeriod = 3;

        // Default array
        private static string[] defaultArray = { "192.168.1.101", "8000", "1", "120000" };

        // Setting array
        private static ArrayList settingArray = new ArrayList();

        public static void Init()
        {
            // Load the settings from a temporary Arraylist
            if (CacheAccess.Load(settingArray, CacheAccess.SettingsFile)) { 
                if (settingArray.Count == defaultArray.Length)
                {
                    return;
                }
            }

            // Something went wrong, load defaults
            Utils.ArrayToList(defaultArray, settingArray);
        }

        public static string Get(int id)
        {
            if (id < settingArray.Count)
            {
                return settingArray[id] as string;
            }
            else
            {
                Debug.Print("ERROR: Setting id out of bounds!");
                return string.Empty;
            }
        }

        public static bool Set(int id, string content)
        {
            if (id < settingArray.Count)
            {
                settingArray[id] = content;
                return CacheAccess.Store(settingArray, CacheAccess.SettingsFile);
            }
            else
            {
                Debug.Print("ERROR: Setting id out of bounds!");
                return false;
            }
        }
    }

}
