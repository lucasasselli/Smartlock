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
        private const String CACHE_FILE = "cache";
        
        private ArrayList userList = new ArrayList();

        private SDCard sdCard;

        CacheAccess(SDCard sdCard)
        {
            this.sdCard = sdCard;
        }

        ArrayList Load()
        {
            FileStream f = sdCard.StorageDevice.OpenRead(CACHE_FILE);
            byte[] data = new byte[f.Length];

            f.Read(data, 0, data.Length);

            userList = (ArrayList) Reflection.Deserialize(data, typeof(ArrayList));

            return userList;
        }

        void Store(ArrayList userList)
        {
            if (userList.Count > 0) {
                FileStream f = sdCard.StorageDevice.OpenWrite(CACHE_FILE);

                // Parse Arraylist to Json
                byte[] data = Reflection.Serialize(userList, typeof(ArrayList));
                f.Write(data, 0, data.Length);

                f.Close();
            }
        }
    }
}
