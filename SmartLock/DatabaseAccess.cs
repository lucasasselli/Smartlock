using System;
using System.Collections;
using System.Threading;
using System.Net;
using System.Runtime.Serialization.Json;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace SmartLock
{
    public partial class Program
    {
        private const string GadgeteerID = "1";
        private const string URL = "http://localhost:8000/SmartLockRESTService/data/?id=" + GadgeteerID;
        ArrayList UserList = new ArrayList();

        public class UserForLock
        {
            public string Pin { get; set; }
            public string CardID { get; set; }
            public string Expire { get; set; }
        }

        private void ServerRequest()
        {
            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Server error (HTTP "+response.StatusCode+": "+response.StatusDescription+").");
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Response));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    Response jsonResponse = objResponse as Response;
                    return jsonResponse;
            }
        }

        private void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is up!");
            Debug.Print("My IP is: " + ethernetJ11D.NetworkSettings.IPAddress);
        }

        private void ethernetJ11D_NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is down!");
        }
    }
}
