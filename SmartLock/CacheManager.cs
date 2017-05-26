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
        private const int mountAttempts = 3;
        private const int mountAttemptPeriod = 50;
        public const string UsersCacheFile = "users";
        public const string LogsCacheFile = "logs";
        public const string SettingsFile = "settings";

        public static bool mountErrorFlag = false;
        public static bool slotErrorFlag = false;

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
                    DebugOnly.Print("ERROR: File \"" + file + "\" is empty!");
                    return false;
                }

                data = new byte[f.Length];
                f.Read(data, 0, data.Length);
                f.Close();
                VolumeInfo.GetVolumes()[0].FlushAll();
            }
            catch (Exception e)
            {
                DebugOnly.Print("ERROR: Exception while reading \"" + file + "\": " + e);
                return false;
            }

            try
            {
                var temp = (ArrayList)Reflection.Deserialize(data, typeof(ArrayList));
                list.CopyFrom(temp);
            }
            catch (Exception e)
            {
                DebugOnly.Print("ERROR: Exception while deserializing \"" + file + "\": " + e);
                return false;
            }

            DebugOnly.Print("File \"" + file + "\" successfully loaded into object");

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
                    DebugOnly.Print("ERROR: Serialized data \"" + file + "\" is null!");
                    return false;
                }
            }
            catch (Exception e)
            {
                DebugOnly.Print("ERROR: Exception while serializing \"" + file + "\": " + e);
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
                DebugOnly.Print("ERROR: Exception while writing \"" + file + "\": " + e);
                return false;
            }

            DebugOnly.Print("Object successfully stored into \"" + file + "\" file");

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
                DebugOnly.Print("ERROR: Exception while deleting \"" + file + "\": " + e);
                return false;
            }

            DebugOnly.Print("File \"" + file + "\" successfully deleted");

            return true;
        }

        private static bool MountCheck()
        {
            if (!sdCard.IsCardInserted)
            {
                DebugOnly.Print("ERROR: SD slot is empty!");

                if (!slotErrorFlag)
                {
                    // First slot error, send it
                    if (DataHelper.IsInitialized())
                    {
                        slotErrorFlag = true; // Leave it here, otherwise it's recursive!!!
                        DataHelper.AddLog(new Log(Log.TypeError, "SD Card slot is empty!"));
                    }
                }
                return false;
            }

            if (!sdCard.IsCardMounted)
            {
                DebugOnly.Print("SD Card appears to be unmounted! Mounting...");

                for (int i = 0; i < mountAttempts; i++)
                {
                    if (sdCard.IsCardMounted) return true;
                    if (sdCard.Mount()) return true;
                    System.Threading.Thread.Sleep(mountAttemptPeriod);
                }

                // Mounting error
                DebugOnly.Print("ERROR: Mounting failed!");
                if (!mountErrorFlag)
                {
                    // First mount error, send it
                    if (DataHelper.IsInitialized())
                    {
                        DataHelper.AddLog(new Log(Log.TypeError, "Unable to mount SDcard!"));
                        mountErrorFlag = true;
                    }
                }
                return false;
            }

            slotErrorFlag = false;
            mountErrorFlag = false;
            return true;
        }

        private static bool FileExists(string file)
        {
            // Check if file exists
            String root;
            String[] foundFiles;
            try
            {
                root = sdCard.StorageDevice.RootDirectory;
                foundFiles = sdCard.StorageDevice.ListFiles(root);
            }
            catch (Exception)
            {
                return false;
            }

            var exist = false;
            foreach (var aFile in foundFiles)
                if (string.Compare(file, aFile) == 0)
                {
                    exist = true;
                    break;
                }

            if (!exist)
                DebugOnly.Print("File \"" + file + "\" not found!");

            return exist;
        }
    }
}