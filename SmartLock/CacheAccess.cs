using System;
using System.IO;
using System.Text;
using System.Collections;

using Microsoft.SPOT;
using Json.NETMF;

using Gadgeteer.Modules.GHIElectronics;

namespace SmartLock.Resources
{
    class CacheAccess
    {
        private const String CACHE_FILE = "cache";
        
        private ArrayList userList = new ArrayList();

        private SDCard sdCard;

        CacheAccess(SDCard sdCard)
        {
            this.sdCard = sdCard;
        }

        ArrayList Load()
        {
            FileStream f = sdCard.StorageDevice.OpenWrite(CACHE_FILE);

            // Parse Arraylist to Json
            string fileOutput = "";
            userList = JsonSerializer.DeserializeString(fileOutput) as ArrayList;
        }

        void Store(ArrayList userList)
        {
            
            if (userList.Count > 0) {
                FileStream f = sdCard.StorageDevice.OpenWrite(CACHE_FILE);

                // Parse Arraylist to Json
                string json = JsonSerializer.SerializeObject(userList);
                byte[] Data = Encoding.UTF8.GetBytes(json);
                f.Write(Data, 0, Data.Length);

            }
        }

    }
}
