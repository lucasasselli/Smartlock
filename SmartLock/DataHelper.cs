using System;
using System.Collections;
using System.Threading;

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

        // Thread
        private bool threadRunning;
        private Thread threadRoutine;
        private ManualResetEvent threadWaitForStop;
        private const int THREAD_PERIOD_LONG = 120000; // milliseconds -> 2 min
        private const int THREAD_PERIOD_SHORT = 10000; // milliseconds -> 10 sec

        // Event handling
        public event DSChangedEventHandler DataSourceChanged;
        public delegate void DSChangedEventHandler(int dataSource);

        // Ethernet object
        private EthernetJ11D ethernetJ11D;

        // Data source
        public const int DATA_SOURCE_UNKNOWN = 0;
        public const int DATA_SOURCE_ERROR = 1;
        public const int DATA_SOURCE_CACHE = 2;
        public const int DATA_SOURCE_REMOTE = 3;
        private int dataSource;

        public DataHelper(EthernetJ11D ethernetJ11D, SDCard sdCard)
        {
            // Create wrapped objects
            cacheAccess = new CacheAccess(sdCard);
            databaseAccess = new DatabaseAccess();

            threadRoutine = new Thread(new ThreadStart(ServerRoutine));
            threadWaitForStop = new ManualResetEvent(false);

            this.ethernetJ11D = ethernetJ11D;
            ethernetJ11D.UseThisNetworkInterface();
            ethernetJ11D.NetworkSettings.EnableDhcp();
            ethernetJ11D.NetworkUp += NetworkUp;
            ethernetJ11D.NetworkDown += NetworkDown;

            // Data is not yet loaded, data source is unknown
            ChangeDataSource(DATA_SOURCE_UNKNOWN);
        }

        public void Init()
        {
            // Load users from cache
            if (cacheAccess.LoadUsers(tempUserList))
            {
                Debug.Print(tempUserList.Count + " users loaded from cache!");
                Utils.ArrayListCopy(tempUserList, userList);

                // Data source is now cache
                ChangeDataSource(DATA_SOURCE_CACHE);
            }
            else
            {
                // Empty data cache is assumed as an error!
                DataSourceChanged(DATA_SOURCE_ERROR);
            }

            // Load logs from cache if any
            if (cacheAccess.LoadLogs(tempLogList))
            {
                Debug.Print(tempLogList.Count + " logs loaded from cache!");
                Utils.ArrayListCopy(tempLogList, logList);
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

        public bool PinHasNullCardID(String Pin)
        {
            foreach (UserForLock user in userList)
            {
                if (String.Compare(Pin, user.Pin) == 0)
                {
                    return user.CardID == null;
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

            // Start ServerRoutine
            StartRoutine();

        }

        // Network is offline event
        private void NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is down!");

            // Data source is now cache
            if (userList.Count > 0)
            {
                ChangeDataSource(DATA_SOURCE_CACHE);
            }
            else
            {
                ChangeDataSource(DATA_SOURCE_ERROR);
            }

            // Stop ServerRoutine
            StopRoutine();
        }

        /*
         * SERVER ROUTINE:
         * ServerRoutine is the only thread of this class. It periodically updates the current user data, and
         * if logs are stored into logList, it sends the logs to the server.
         */

        private void ServerRoutine()
        {
            while (threadRunning)
            {
                if (ethernetJ11D.IsNetworkUp)
                {
                    Debug.Print("Beginning server polling routine...");

                    // Clean temporary list
                    tempUserList.Clear();

                    Debug.Print("Requesting user list to server...");

                    if (databaseAccess.RequestUsers(tempUserList))
                    {
                        // Copy content to main list
                        userList.Clear();
                        Utils.ArrayListCopy(tempUserList, userList);

                        // Store cache copy
                        cacheAccess.StoreUsers(userList);

                        // Data source is now remote
                        ChangeDataSource(DATA_SOURCE_REMOTE);

                        Debug.Print(userList.Count + " users received from server");
                    }
                    else
                    {
                        Debug.Print("ERROR: User list request failed!");

                        // Data source is now cache
                        if (userList.Count > 0)
                        {
                            ChangeDataSource(DATA_SOURCE_CACHE);
                        }
                        else
                        {
                            ChangeDataSource(DATA_SOURCE_ERROR);
                        }
                    }

                    if (logList.Count > 0)
                    {
                        Debug.Print(logList.Count + " stored logs must be sent to server!");
                        // Send accumulated logs
                        if (databaseAccess.SendLogs(logList))
                        {
                            Debug.Print("Logs sent to server");

                            // Log list sent to server successfully: delete loglist
                            logList.Clear();
                            cacheAccess.StoreLogs(logList);
                        }
                        else
                        {
                            Debug.Print("ERROR: Log sending failed!");
                        }
                    }
                }
                else
                {
                    Debug.Print("ERROR: No connection, skipping scheduled server polling routine.");
                }

                // Plan next connection
                if (dataSource != DATA_SOURCE_REMOTE && ethernetJ11D.IsNetworkUp)
                    threadWaitForStop.WaitOne(THREAD_PERIOD_SHORT, true);
                else
                    threadWaitForStop.WaitOne(THREAD_PERIOD_LONG, true);
            }
        }

        public void StartRoutine()
        {
            threadRunning = true;
            if (!threadRoutine.IsAlive)
            {
                threadRoutine.Start();
            }
            else
            {
                threadWaitForStop.Set();
            }
        }

        public void StopRoutine()
        {
            threadRunning = false;
        }

        /*
         * DATA SOURCE:
         * The attribute dataSorce stores the source of the user data currently being used.
         * dataSource is determinred according to the following rules:
         * - If the data is not being loaded, dataSource is DATA_SOURCE_UNKNOWN
         * - If the network is down, dataSource is DATA_SOURCE_CACHE
         * - If the network id up and last ServerRoutine was succesfull, dataSource is DATA_SOURCE_REMOTE
         * - In any other case, dataSource is DATA_SOURCE_CACHE
         */

        // Changes the current data source and throws event DataSourceChanged
        private void ChangeDataSource(int dataSource)
        {
            if (this.dataSource != dataSource)
            {
                // Notify the event only if the data has actually changed!
                if (DataSourceChanged != null)
                {
                    DataSourceChanged(dataSource);
                }
            }

            this.dataSource = dataSource;
        }

        // Returns the current data source
        public int GetDataSource()
        {
            return dataSource;
        }
    }
}
