using System;
using System.IO;
using System.Text;
using System.Collections;

using Microsoft.SPOT;

using Gadgeteer.Modules.GHIElectronics;

namespace SmartLock
{
    class CacheAccess
    {
        private const String USER_CACHE_FILE = "users";
        private const String LOG_CACHE_FILE = "log";       

        private SDCard sdCard;

        public CacheAccess(SDCard sdCard)
        {
            this.sdCard = sdCard;
        }

        // Data Management
        public bool LoadUsers(ArrayList destination)
        {
            return Load(destination, USER_CACHE_FILE);
        }

        public bool LoadLogs(ArrayList destination)
        {
            return Load(destination, LOG_CACHE_FILE);
        }

        public bool StoreUsers(ArrayList userList)
        {
            return Store(userList, USER_CACHE_FILE);
        }

        public bool StoreLogs(ArrayList logList)
        {
            return Store(logList, LOG_CACHE_FILE);
        }

        // Inner methods
        private bool Load(ArrayList list, String file)
        {
            // Init SD card
            if (!Init(file))
            {
                return false;
            }

            // Check if file exist
            if (!FileExists(file))
            {
                return false;
            }

            byte[] data;


            try
            {
                FileStream f = sdCard.StorageDevice.OpenRead(file);

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
                Debug.Print("ERROR: Exception while reading \"" + file + "\": " + e.ToString());
                return false;
            }

            try
            {
                ArrayList temp = (ArrayList)Reflection.Deserialize(data, typeof(ArrayList));
                Utils.ArrayListCopy(temp, list);
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while deserializing \"" + file + "\": " + e.ToString());
                return false;
            }

            Debug.Print("File \"" + file + "\" successfully loaded into \"" + list.ToString() + "\" object");

            return true;
        }

        private bool Store(ArrayList list, String file)
        {
            // Init SD card
            if (!Init(file))
            {
                return false;
            }

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
                Debug.Print("ERROR: Exception while serializing \"" + file + "\": " + e.ToString());
                return false;
            }

            try
            {
                // Write data
                FileStream f = sdCard.StorageDevice.OpenWrite(file);
                f.Write(data, 0, data.Length);
                f.Close();
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while writing \"" + file + "\": " + e.ToString());
                return false;
            }

            Debug.Print("Object \"" + list.ToString() + "\" successfully stored into \"" + file + "\" file");

            return true;
        }

        private bool Init(String file){

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

        bool FileExists(String file)
        {
            // Check if file exists
            string root = sdCard.StorageDevice.RootDirectory;
            string[] foundFiles = sdCard.StorageDevice.ListFiles(root);

            bool exist = false;
            foreach (string aFile in foundFiles)
            {
                if (String.Compare(file, aFile) == 0)
                {
                    exist = true;
                    break;
                }
            }

            if (!exist)
            {
                Debug.Print("File \"" + file + "\" not found!");
            }

            return exist;
        }
    }
}
