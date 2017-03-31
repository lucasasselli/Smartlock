using System;
using System.IO;
using System.Collections;
using System.Threading;
using System.Net;
using Json.NETMF;

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

        private void ServerRequest()
        {
            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream response_stream = response.GetResponseStream();
                    StreamReader response_reader = new StreamReader(response_stream);
                    string response_string = response_reader.ReadToEnd();
                    ArrayList UserList = JsonSerializer.DeserializeString(response_string) as ArrayList;
                    
                }
            }
        }

        private void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is up!"); //debug
            Debug.Print("My IP is: " + ethernetJ11D.NetworkSettings.IPAddress); //debug
        }

        private void ethernetJ11D_NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is down!"); //debug
        }
    }
}
