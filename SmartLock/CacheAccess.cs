using System;
using System.Collections;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;

namespace SmartLock
{
    internal class CacheAccess
    {
        private const string UserCacheFile = "users";
        private const string LogCacheFile = "log";

        private readonly SDCard sdCard;

        public CacheAccess(SDCard sdCard)
        {
            this.sdCard = sdCard;
        }

        // Data Management
        public bool LoadUsers(ArrayList destination)
        {
            return Load(destination, UserCacheFile);
        }

        public bool LoadLogs(ArrayList destination)
        {
            return Load(destination, LogCacheFile);
        }

        public bool StoreUsers(ArrayList userList)
        {
            return Store(userList, UserCacheFile);
        }

        public bool StoreLogs(ArrayList logList)
        {
            return Store(logList, LogCacheFile);
        }

        // Inner methods
        private bool Load(ArrayList list, string file)
        {
            // Init SD card
            if (!Init())
                return false;

            // Check if file exist
            if (!FileExists(file))
                return false;

            byte[] data;


            try
            {
                var f = sdCard.StorageDevice.OpenRead(file);

                // If the file is empty do not parse
                if (f.Length == 0)
                {
                    Debug.Print("ERROR: File \"" + file + "\" is empty!");
                    return false;
                }

                data = new byte[f.Length];
                f.Read(data, 0, data.Length);
                f.Close();
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while reading \"" + file + "\": " + e);
                return false;
            }

            try
            {
                var temp = (ArrayList) Reflection.Deserialize(data, typeof(ArrayList));
                Utils.ArrayListCopy(temp, list);
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while deserializing \"" + file + "\": " + e);
                return false;
            }

            Debug.Print("File \"" + file + "\" successfully loaded into object");

            return true;
        }

        private bool Store(ArrayList list, string file)
        {
            // Init SD card
            if (!Init())
                return false;

            FileExists(file);

            byte[] data;

            try
            {
                // Serialize data
                data = Reflection.Serialize(list, typeof(ArrayList));

                // This shouldn't happen, just in case...
                if (data.Length == 0)
                {
                    Debug.Print("ERROR: Serialized data \"" + file + "\" is null!");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while serializing \"" + file + "\": " + e);
                return false;
            }

            try
            {
                // Write data
                var f = sdCard.StorageDevice.OpenWrite(file);
                f.Write(data, 0, data.Length);
                f.Close();
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while writing \"" + file + "\": " + e);
                return false;
            }

            Debug.Print("Object successfully stored into \"" + file + "\" file");

            return true;
        }

        private bool Init()
        {
            if (!sdCard.IsCardInserted)
            {
                Debug.Print("ERROR: SD slot is empty!");
                return false;
            }

            if (!sdCard.IsCardMounted)
            {
                Debug.Print("SD Card appears to be unmounted! Mounting...");
                if (!sdCard.Mount())
                {
                    Debug.Print("ERROR: Mounting failed!");
                    return false;
                }
            }

            return true;
        }

        private bool FileExists(string file)
        {
            // Check if file exists
            var root = sdCard.StorageDevice.RootDirectory;
            var foundFiles = sdCard.StorageDevice.ListFiles(root);

            var exist = false;
            foreach (var aFile in foundFiles)
                if (string.Compare(file, aFile) == 0)
                {
                    exist = true;
                    break;
                }

            if (!exist)
                Debug.Print("File \"" + file + "\" not found!");

            return exist;
        }
    }
}