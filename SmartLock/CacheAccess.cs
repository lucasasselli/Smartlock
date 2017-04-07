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

        public void StoreUsers(ArrayList userList)
        {
            Store(userList, USER_CACHE_FILE);
        }

        public void StoreLogs(ArrayList logList)
        {
            Store(logList, USER_CACHE_FILE);
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
           
            FileStream f = sdCard.StorageDevice.OpenRead(file);

            if (f.Length == 0)
            {
                Debug.Print("ERROR: File \"" + file + "\" is empty!");
                return false;
            }

            byte[] data = new byte[f.Length];
            f.Read(data, 0, data.Length);
            f.Close();

            try
            {
                ArrayList temp = (ArrayList)Reflection.Deserialize(data, typeof(ArrayList));
                Utils.ArrayListCopy(temp, list);
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while deserializing \"" + file + "\"");
                return false;
            }


            return true;
        }

        private bool Store(ArrayList list, String file)
        {
            // Init SD card
            if (!Init(file))
            {
                return false;
            }

            if (!FileExists(file))
            {
                // Create file
            }

            FileStream f = sdCard.StorageDevice.OpenWrite(file);

            // Parse Arraylist to Json
            byte[] data = Reflection.Serialize(list, typeof(ArrayList));
            f.Write(data, 0, data.Length);
            f.Close();

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

            if (foundFiles.Length > 0)
                Debug.Print("Files in SD root: ");
            else
                Debug.Print("Folder is empty!");

            bool exist = false;
            foreach (string aFile in foundFiles)
            {
                Debug.Print(aFile);
                if (String.Compare(file, aFile) == 0)
                {
                    Debug.Print("File " + file + " found!");
                    exist = true;
                }
            }

            return exist;
        }
    }
}
