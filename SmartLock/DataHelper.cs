using System;
using System.Collections;

using Microsoft.SPOT;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace SmartLock
{
    class DataHelper
    {
        // Main data object
        private ArrayList userList = new ArrayList();
        private ArrayList logList = new ArrayList();

        // Temp holders (CLEAN BEFORE USE!)
        private ArrayList tempUserList = new ArrayList();
        private ArrayList tempLogList = new ArrayList();

        // Wrapped classes
        CacheAccess cacheAccess;
        DatabaseAccess databaseAccess;

        // Flags
        private static bool flagFirstConnection; // first connection after power up

        // Timers
        private GT.Timer timerServerReq; // timer for server request
        private const int timerServerReqCount = 120000; // milliseconds -> 2 min

        // Ethernet instance
        EthernetJ11D ethernetJ11D;

        public DataHelper(EthernetJ11D ethernetJ11D, SDCard sdCard)
        {
            cacheAccess = new CacheAccess(sdCard);
            databaseAccess = new DatabaseAccess();

            flagFirstConnection = true;

            this.ethernetJ11D = ethernetJ11D;
            ethernetJ11D.UseThisNetworkInterface();
            ethernetJ11D.NetworkSettings.EnableDhcp();
            ethernetJ11D.NetworkUp += NetworkUp;
            ethernetJ11D.NetworkDown += NetworkDown;

            timerServerReq = new GT.Timer(timerServerReqCount);
            timerServerReq.Tick += timerServerReq_Tick;

            // Load users from cache
            if(cacheAccess.LoadUsers(tempUserList))
            {
                Utils.ArrayListCopy(tempUserList, userList);
            }

            // Load logs from cache if any
            if (cacheAccess.LoadLogs(tempLogList))
            {
                Utils.ArrayListCopy(tempUserList, userList);
            }
        }

        // Access Management
        public bool CheckCardID(String CardID)
        {
            foreach (UserForLock user in userList)
            {
                if (CardID.Equals(user.CardID))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckPin(String Pin)
        {
            foreach (UserForLock user in userList)
            {
                if (Pin.Equals(user.Pin))
                {
                    return true;
                }
            }

            return false;
        }

        public void AddLog(Log log)
        {
            logList.Add(log);

        }

        // Network is online event
        private void NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is up!"); // debug
            Debug.Print("My IP is: " + ethernetJ11D.NetworkSettings.IPAddress); // debug

            if (flagFirstConnection)
            {
                GetUsers();
                flagFirstConnection = false;
                timerServerReq.Start();
            }
            if (logList.Count > 0)
            {
                // Send accumulated logs
                databaseAccess.SendLogs(logList);
                // If success clear logs
            }
                

        }

        // Network is offline event
        private void NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is down!"); //debug
        }

        private void timerServerReq_Tick(GT.Timer timerServerReq)
        {
            if (ethernetJ11D.IsNetworkUp)
            {
                GetUsers();
            }
        }

        // 
        private void GetUsers()
        {
            // Clean temporary list
            tempUserList.Clear();

            if (databaseAccess.RequestUsers(tempUserList))
            {
                // Copy content to main list
                userList.Clear();
                Utils.ArrayListCopy(tempUserList, userList);
                cacheAccess.StoreUsers(userList);
            }
        }

        // Info
        public bool IsOnline()
        {
            return ethernetJ11D.IsNetworkUp;
        }
    }
}
