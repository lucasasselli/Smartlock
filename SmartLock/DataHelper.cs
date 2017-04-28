using System;
using System.Collections;
using System.Threading;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;
using GTM = Gadgeteer.Modules;
using GHI.Processor;
using DateTimeExtension;
using ArrayListExtension;

namespace SmartLock
{
    internal static class DataHelper
    {

        private const int ThreadPeriodLong = 120000; // milliseconds -> 2 min
        private const int ThreadPeriodShort = 10000; // milliseconds -> 10 sec

        // Data source
        public const int DataSourceUnknown = 0;
        public const int DataSourceError = 1;
        public const int DataSourceCache = 2;
        public const int DataSourceRemote = 3;

        // Event handling
        public delegate void DsChangedEventHandler(int dataSource);
        public static event DsChangedEventHandler DataSourceChanged;

        // Ethernet object
        private static EthernetJ11D ethernetJ11D;

        // Main data object
        private static readonly ArrayList logList = new ArrayList();
        private static readonly ArrayList userList = new ArrayList();

        // Server + JSON stuff
        private const string DataRequest = "data";
        private const string TimeRequest = "time";
        private const string UserHeader = "AllowedUsers";
        private const string LogHeader = "Log";

        // Thread
        private static bool threadRunning;
        private static Thread threadRoutine;
        private static readonly ManualResetEvent threadWaitForStop = new ManualResetEvent(false);

        private static int dataSource;
        private static bool timeChecked = false;

        public static void Init(EthernetJ11D _ethernetJ11D)
        {

            ethernetJ11D = _ethernetJ11D;
            ethernetJ11D.UseThisNetworkInterface();
            ethernetJ11D.NetworkSettings.EnableDhcp();
            ethernetJ11D.NetworkUp += NetworkUp;
            ethernetJ11D.NetworkDown += NetworkDown;

            // Data is not yet loaded, data source is unknown
            ChangeDataSource(DataSourceUnknown);

            // Load users from cache
            if (CacheManager.Load(userList, CacheManager.UsersCacheFile))
            {
                Debug.Print(userList.Count + " users loaded from cache!");

                // Data source is now cache
                ChangeDataSource(DataSourceCache);
            }
            else
            {
                // Empty data cache is assumed as an error!
                if (DataSourceChanged != null) DataSourceChanged(DataSourceError);

                // Clear user list
                userList.Clear();
            }

            // Load logs from cache if any
            if (CacheManager.Load(logList, CacheManager.LogsCacheFile))
            {
                Debug.Print(logList.Count + " logs loaded from cache!");
            }
            else
            {
                // Clear log list
                logList.Clear();
            }
        }

        // Access Management
        public static bool CheckCardId(string cardId)
        {
            foreach (User user in userList)
                if (string.Compare(cardId, user.CardID) == 0)
                    return true;

            return false;
        }

        public static bool CheckPin(string pin)
        {
            foreach (User user in userList)
                if (string.Compare(pin, user.Pin) == 0)
                    return true;

            return false;
        }

        public static bool PinHasNullCardId(string pin)
        {
            foreach (User user in userList)
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

        public static void AddCardId(string pin, string cardId)
        {
            foreach (User user in userList)
                if (user.Pin == pin)
                {
                    user.CardID = cardId;
                    break;
                }

            // Update cache copy
            CacheManager.Store(userList, CacheManager.UsersCacheFile);
        }

        public static void AddLog(Log log)
        {
            logList.Add(log);

            // Update cache copy
            CacheManager.Store(logList, CacheManager.LogsCacheFile);

            // If log is error start routine immediately
            if (log.Type == Log.TypeError)
            {
                threadWaitForStop.Set();
            }
        }

        // Network is online event
        private static void NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is up! Waiting for ip...");

            // Start ServerRoutine
            StartRoutine();
        }

        // Network is offline event
        private static void NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
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

        private static void ServerRoutine()
        {
            while (threadRunning)
            {
                bool success = true;

                if (ethernetJ11D.IsNetworkUp)
                {
                    Debug.Print("Beginning server routine...");

                    // Check if ip is valid
                    if (String.Compare(ethernetJ11D.NetworkSettings.IPAddress, "0.0.0.0") != 0)
                    {
                        Debug.Print("My IP is: " + ethernetJ11D.NetworkSettings.IPAddress);
                    }
                    else
                    {
                        Debug.Print("ERROR: Current IP appears to be null!");
                        success = false;
                    }

                    // Request current time
                    if (success && !timeChecked)
                    {
                        success = requestTime();
                    }

                    // Send logs
                    if (success && logList.Count > 0)
                    {
                        Debug.Print(logList.Count + " stored logs must be sent to server!");
                        success = sendLogs();
                    }
                    
                    // Request users
                    if (success)
                    {
                        success = requestUsers();
                    }
                }
                else
                {
                    Debug.Print("ERROR: No connection, skipping scheduled server polling routine.");
                }

                // Plan next routine
                if (success)
                {
                    Debug.Print("Server routine completed! Next event in " + ThreadPeriodLong);
                    threadWaitForStop.WaitOne(ThreadPeriodLong, true);
                }
                else
                {
                    Debug.Print("Server routine failed! Next event in " + ThreadPeriodShort);
                    threadWaitForStop.WaitOne(ThreadPeriodShort, true);
                }

                threadWaitForStop.Reset();
            }
        }

        public static void StartRoutine()
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

        public static void StopRoutine()
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
        private static void ChangeDataSource(int _dataSource)
        {
            if (dataSource != _dataSource)
            {
                dataSource = _dataSource;

                if (DataSourceChanged != null)
                    DataSourceChanged(dataSource);
            }
        }

        // Returns the current data source
        public static int GetDataSource()
        {
            return dataSource;
        }


        /*
         * Server Access
         */
        private static string buildUrlFromSettings(string field)
        {
            string ServerIp = SettingsManager.Get(SettingsManager.ServerIp);
            string ServerPort = SettingsManager.Get(SettingsManager.ServerPort);
            string LockId = SettingsManager.Get(SettingsManager.LockId);
            return "http://" + ServerIp + ":" + ServerPort + "/SmartLockRESTService/" + field + "/?id=" + LockId;
        }


        // Loads userlist from server
        private static bool requestUsers()
        {
            // Create URL
            string url = buildUrlFromSettings(DataRequest);

            // Send request
            Debug.Print("Requesting user list to server...");
            Remote.Result result = Remote.Get(url);

            // Parse response
            if (result.Success)
            {
                ArrayList tempUserList = new ArrayList();

                if (Json.ParseNamedArray(UserHeader, result.Content, tempUserList, typeof(User)))
                {
                    // Copy content to main list
                    // NOTE: CopyFrom clears list automatically
                    userList.CopyFrom(tempUserList);

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
            }

            // Return result of the operation
            return result.Success;
        }

        // Get current time from server
        private static bool requestTime()
        {
            string url = buildUrlFromSettings(TimeRequest);

            Remote.Result result = Remote.Get(url);

            if (result.Success)
            {
                // Request current time
                Debug.Print("Requesting current time to server...");
                DateTime serverDt = result.Content.ToDateTime();
                DateTime rtcDt = RealTimeClock.GetDateTime();

                if (DateTime.Compare(serverDt, DateTime.MinValue) != 0)
                {
                    if (!serverDt.WeakCompare(rtcDt))
                    {
                        // Found time mismatch
                        Debug.Print("ERROR: RTC/Server time mismatch! Server: " + serverDt.ToMyString() + ", RTC: " + rtcDt.ToMyString());
                        Debug.Print("Setting RTC...");
                        Log log = new Log(Log.TypeInfo, "RTC/Server time mismatch! Server: " + serverDt.ToMyString() + ", RTC: " + rtcDt.ToMyString());
                        AddLog(log);

                        RealTimeClock.SetDateTime(serverDt);
                    }
                    else
                    {
                        Debug.Print("RTC already synced with server time!");
                    }

                    // RTC time is now valid
                    timeChecked = true;

                    return true;
                }
            }

            return false;
        }

        // Sends log to server
        private static bool sendLogs()
        {
            // Create JSON String
            var jsonString = Json.BuildNamedArray(LogHeader, logList);

            // Create URL
            string url = buildUrlFromSettings(DataRequest);

            // Send request
            Debug.Print("Sending logs to server...");
            Remote.Result result = Remote.Post(url, jsonString);

            if (result.Success)
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

            // Return result of the operation
            return result.Success;
        }
    }
}