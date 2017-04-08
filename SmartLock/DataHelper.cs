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

        // Timers
        private GT.Timer timerServerReq; // timer for server request
        private const int timerServerReqCount = 120000; // milliseconds -> 2 min

        EthernetJ11D ethernetJ11D;

        public DataHelper(EthernetJ11D ethernetJ11D, SDCard sdCard)
        {
            // Create wrapped objects
            cacheAccess = new CacheAccess(sdCard);
            databaseAccess = new DatabaseAccess();

            this.ethernetJ11D = ethernetJ11D;
            ethernetJ11D.UseThisNetworkInterface();
            ethernetJ11D.NetworkSettings.EnableDhcp();
            ethernetJ11D.NetworkUp += NetworkUp;
            ethernetJ11D.NetworkDown += NetworkDown;

            timerServerReq = new GT.Timer(timerServerReqCount);
            timerServerReq.Tick += ServerRoutine;

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
                if (String.Compare(CardID, user.CardID) == 0)
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
                if (String.Compare(Pin, user.Pin) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddLog(Log log)
        {
            logList.Add(log);

            // Update cache copy
            cacheAccess.StoreLogs(logList);
        }

        // Network is online event
        private void NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is up!"); 
            Debug.Print("My IP is: " + ethernetJ11D.NetworkSettings.IPAddress);

           timerServerReq.Start();

        }

        // Network is offline event
        private void NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is down!"); //debug
        }

        private void ServerRoutine(GT.Timer timerServerReq)
        {
            if (ethernetJ11D.IsNetworkUp)
            {
                // Clean temporary list
                tempUserList.Clear();

                if (databaseAccess.RequestUsers(tempUserList))
                {
                    // Copy content to main list
                    userList.Clear();
                    Utils.ArrayListCopy(tempUserList, userList);

                    // Store cache copy
                    cacheAccess.StoreUsers(userList);
                }

                if (logList.Count > 0)
                {
                    // Send accumulated logs
                    if (databaseAccess.SendLogs(logList))
                    {
                        // Log list sent to server successfully: delete loglist
                        logList.Clear();
                        cacheAccess.StoreLogs(logList);
                    }
                }
            }
        }
    }
}
