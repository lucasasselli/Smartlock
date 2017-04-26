using System;
using System.Collections;
using Microsoft.SPOT;
using ArrayListExtension;


namespace SmartLock
{
    internal static class SettingsManager
    {
        // Settings
        public const int ServerIp = 0;
        public const int ServerPort = 1;
        public const int LockId = 2;
        public const int RequestPeriod = 3;
        public const int MasterPin = 4;

        // Default array
        private static string[] defaultArray = { "192.168.1.101", "8000", "1", "120000", "0000000000"};

        // Setting array
        private static ArrayList settingArray = new ArrayList();

        public static void Init()
        {
            // Load the settings from a temporary Arraylist
            if (CacheManager.Load(settingArray, CacheManager.SettingsFile)) { 
                if (settingArray.Count == defaultArray.Length)
                {
                    return;
                }
            }

            // Something went wrong, load defaults
            settingArray = defaultArray.ToArrayList();
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
                return CacheManager.Store(settingArray, CacheManager.SettingsFile);
            }
            else
            {
                Debug.Print("ERROR: Setting id out of bounds!");
                return false;
            }
        }
    }

}
