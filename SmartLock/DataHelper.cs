using System;
using System.Collections;
using System.Threading;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GHI.Processor;

namespace SmartLock
{
    internal class DataHelper
    {
        public delegate void DsChangedEventHandler(int dataSource);

        private const int ThreadPeriodLong = 120000; // milliseconds -> 2 min
        private const int ThreadPeriodShort = 10000; // milliseconds -> 10 sec

        // Data source
        public const int DataSourceUnknown = 0;
        public const int DataSourceError = 1;
        public const int DataSourceCache = 2;
        public const int DataSourceRemote = 3;

        // Wrapped classes
        private readonly ServerManager serverManager;

        // Ethernet object
        private readonly EthernetJ11D ethernetJ11D;

        // Main data object
        private readonly ArrayList logList = new ArrayList();
        private readonly ArrayList userList = new ArrayList();

        // Temp holders (CLEAN BEFORE USE!)
        private readonly ArrayList tempLogList = new ArrayList();
        private readonly ArrayList tempUserList = new ArrayList();

        // Thread
        private bool threadRunning;
        private Thread threadRoutine;
        private readonly ManualResetEvent threadWaitForStop;

        private int dataSource;

        public DataHelper(EthernetJ11D ethernetJ11D)
        {
            serverManager = new ServerManager();

            threadWaitForStop = new ManualResetEvent(false);

            this.ethernetJ11D = ethernetJ11D;
            ethernetJ11D.UseThisNetworkInterface();
            ethernetJ11D.NetworkSettings.EnableDhcp();
            ethernetJ11D.NetworkUp += NetworkUp;
            ethernetJ11D.NetworkDown += NetworkDown;

            // Data is not yet loaded, data source is unknown
            ChangeDataSource(DataSourceUnknown);
        }

        // Event handling
        public event DsChangedEventHandler DataSourceChanged;

        public void Init()
        {
            // Load users from cache
            if (CacheManager.Load(tempUserList, CacheManager.UsersCacheFile))
            {
                Debug.Print(tempUserList.Count + " users loaded from cache!");
                Utils.ArrayListCopy(tempUserList, userList);

                // Data source is now cache
                ChangeDataSource(DataSourceCache);
            }
            else
            {
                // Empty data cache is assumed as an error!
                if (DataSourceChanged != null) DataSourceChanged(DataSourceError);
            }

            // Load logs from cache if any
            if (CacheManager.Load(tempLogList, CacheManager.LogsCacheFile))
            {
                Debug.Print(tempLogList.Count + " logs loaded from cache!");
                Utils.ArrayListCopy(tempLogList, logList);
            }
        }

        // Access Management
        public bool CheckCardId(string cardId)
        {
            foreach (UserForLock user in userList)
                if (string.Compare(cardId, user.CardID) == 0)
                    return true;

            return false;
        }

        public bool CheckPin(string pin)
        {
            foreach (UserForLock user in userList)
                if (string.Compare(pin, user.Pin) == 0)
                    return true;

            return false;
        }

        public bool PinHasNullCardId(string pin)
        {
            foreach (UserForLock user in userList)
                if (string.Compare(pin, user.Pin) == 0)
                    if (user.CardID != null)
                        if (string.Compare(string.Empty, user.CardID) == 0)
                            return true;
                        else
                            return false;
                    else
                        return true;

            return false;
        }

        public void AddCardId(string pin, string cardId)
        {
            foreach (UserForLock user in userList)
                if (user.Pin == pin)
                {
                    user.CardID = cardId;
                    break;
                }

            // Update cache copy
            CacheManager.Store(userList, CacheManager.UsersCacheFile);
        }

        public void AddLog(Log log)
        {
            logList.Add(log);

            // Update cache copy
            CacheManager.Store(logList, CacheManager.LogsCacheFile);
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
                ChangeDataSource(DataSourceCache);
            else
                ChangeDataSource(DataSourceError);

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

                    // Request current time
                    DateTime rtcDt = RealTimeClock.GetDateTime();
                    DateTime serverDt = serverManager.RequestTime();

                    if (DateTime.Compare(serverDt, DateTime.MinValue) != 0)
                    {
                        if (DateTime.Compare(serverDt, rtcDt) != 0)
                        {
                            // Found time mismatch
                            Debug.Print("ERROR: RTC/Server time mismatch! Server: " + serverDt.ToString() + ", RTC: " + rtcDt.ToString());
                            Debug.Print("Setting RTC...");
                            RealTimeClock.SetDateTime(serverDt);

                            Log log = new Log(Log.TypeError, "RTC/Server time mismatch! Server: " + serverDt.ToString() + ", RTC: " + rtcDt.ToString());
                        }
                    }
                    
                    // Clean temporary list
                    tempUserList.Clear();

                    Debug.Print("Requesting user list to server...");

                    if (serverManager.RequestUsers(tempUserList))
                    {
                        // Copy content to main list
                        userList.Clear();
                        Utils.ArrayListCopy(tempUserList, userList);

                        // Store cache copy
                        CacheManager.Store(userList, CacheManager.UsersCacheFile);

                        // Data source is now remote
                        ChangeDataSource(DataSourceRemote);

                        Debug.Print(userList.Count + " users received from server");
                    }
                    else
                    {
                        Debug.Print("ERROR: User list request failed!");

                        // Data source is now cache
                        if (userList.Count > 0)
                            ChangeDataSource(DataSourceCache);
                        else
                            ChangeDataSource(DataSourceError);
                    }

                    if (logList.Count > 0)
                    {
                        Debug.Print(logList.Count + " stored logs must be sent to server!");
                        // Send accumulated logs
                        if (serverManager.SendLogs(logList))
                        {
                            Debug.Print("Logs sent to server");

                            // Log list sent to server successfully: delete loglist
                            logList.Clear();
                            CacheManager.Store(logList, CacheManager.LogsCacheFile);
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
                if (dataSource != DataSourceRemote && ethernetJ11D.IsNetworkUp)
                    threadWaitForStop.WaitOne(ThreadPeriodShort, true);
                else
                    threadWaitForStop.WaitOne(ThreadPeriodLong, true);

                threadWaitForStop.Reset();
            }
        }

        public void StartRoutine()
        {
            threadRunning = true;

            if (threadRoutine != null)
            {
                if (!threadRoutine.IsAlive)
                {
                    threadRoutine = new Thread(ServerRoutine);
                    threadRoutine.Start();
                }
                else
                {
                    threadWaitForStop.Set();
                }
            }
            else
            {
                threadRoutine = new Thread(ServerRoutine);
                threadRoutine.Start();
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
                if (DataSourceChanged != null)
                    DataSourceChanged(dataSource);

            this.dataSource = dataSource;
        }

        // Returns the current data source
        public int GetDataSource()
        {
            return dataSource;
        }
    }
}