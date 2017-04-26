using System;
using System.Collections;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using ArrayListExtension;

namespace SmartLock
{
    internal static class CacheManager
    {
        private const int mountAttempts = 10;
        private const int mountAttemptPeriod = 100;
        public const string UsersCacheFile = "users";
        public const string LogsCacheFile = "logs";
        public const string SettingsFile = "settings";

        private static SDCard sdCard;

        public static void Init(SDCard _sdCard)
        {
            sdCard = _sdCard;
        }

        public static bool Load(ArrayList list, string file)
        {
            // Init SD card
            if (!MountCheck())
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
                VolumeInfo.GetVolumes()[0].FlushAll();
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while reading \"" + file + "\": " + e);
                return false;
            }

            try
            {
                var temp = (ArrayList)Reflection.Deserialize(data, typeof(ArrayList));
                list.CopyFrom(temp);
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while deserializing \"" + file + "\": " + e);
                return false;
            }

            Debug.Print("File \"" + file + "\" successfully loaded into object");

            return true;
        }

        public static bool Store(ArrayList list, string file)
        {
            // Init SD card
            if (!MountCheck())
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
                VolumeInfo.GetVolumes()[0].FlushAll();
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while writing \"" + file + "\": " + e);
                return false;
            }

            Debug.Print("Object successfully stored into \"" + file + "\" file");

            return true;
        }

        public static bool Delete(string file)
        {
            // Init SD card
            if (!MountCheck())
                return false;

            // Check if file exist
            if (!FileExists(file))
                return true;

            try
            {
                // Write data
                sdCard.StorageDevice.Delete(file);
                VolumeInfo.GetVolumes()[0].FlushAll();

            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while deleting \"" + file + "\": " + e);
                return false;
            }

            Debug.Print("File \"" + file + "\" successfully deleted");

            return true;
        }

        private static bool MountCheck()
        {
            if (!sdCard.IsCardInserted)
            {
                Debug.Print("ERROR: SD slot is empty!");
                return false;
            }

            if (!sdCard.IsCardMounted)
            {
                Debug.Print("SD Card appears to be unmounted! Mounting...");

                for (int i = 0; i < mountAttempts; i++)
                {
                    if (sdCard.IsCardMounted) return true;
                    if (sdCard.Mount()) return true;
                    System.Threading.Thread.Sleep(mountAttemptPeriod);
                }

                Debug.Print("ERROR: Mounting failed!");
                return false;
            }

            return true;
        }

        private static bool FileExists(string file)
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