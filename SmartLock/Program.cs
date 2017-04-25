using System;
using Microsoft.SPOT;
using GHI.Glide;
using SmartLock.GUI;
using GT = Gadgeteer;

namespace SmartLock
{
    public partial class Program
    {
        // Time constants
        private const int WindowAccessPeriod = 2000;
        private const int WindowAlertPeriod = 10000;
        private const int NfcScanPeriod = 1000;
        private const int NfcScanTimeout = 200;

        // Window ID constants
        private const int WindowPinId = 0;
        private const int WindowScanId = 1;

        // Nfc setup
        private string pendingPin;

        // Main Objects
        private DataHelper dataHelper;

        // Windows
        PinWindow pinWindow = new PinWindow();
        AccessWindow accessWindow = new AccessWindow(WindowAccessPeriod);
        AlertWindow scanWindow = new AlertWindow(WindowAlertPeriod);
        MaintenanceWindow maintenanceWindow = new MaintenanceWindow();

        public void ProgramStarted()
        {
            Debug.Print("Program started!");

            // Static init
            CacheManager.Init(sdCard);
            SettingsManager.Init();

            dataHelper = new DataHelper(ethernetJ11D);

            // Event Setup
            adafruit_PN532.TagFound += TagFound;
            pinWindow.PinFound += PinFound;
            dataHelper.DataSourceChanged += DataSourceChanged;

            // Set scan window
            scanWindow.SetText("Please scan your NFC card now...");
            scanWindow.SetNegativeButton("Cancel", delegate { scanWindow.Dismiss(); });

            // Windows and window manager
            GlideTouch.Initialize();

            pinWindow.Id = WindowPinId;
            scanWindow.Id = WindowScanId;

            WindowManger.MainWindow = pinWindow;
            WindowManger.WindowChanged += WindowChanged;
            WindowManger.Show(pinWindow);

            dataHelper.Init();
            adafruit_PN532.Init();
        }

        /*
         * TAG FOUND EVENT:
         * This event occurs when the user passes a NFC card near the reader.
         * It checks if the UID is valid and unlock the door if so.
         */
        private void TagFound(string uid)
        {
            if (!scanWindow.IsShowing())
            {
                // Check authorization
                var authorized = dataHelper.CheckCardId(uid);

                // Show access window
                accessWindow.Show(authorized);

                // Log the event
                string logText;
                if (authorized)
                {
                    // Access granted
                    UnlockDoor();
                    logText = "Card \"" + uid + "\" inserted. Authorized access.";
                }
                else
                {
                    // Access denied
                    logText = "Card \"" + uid + "\" inserted. Access denied!";
                }

                Debug.Print(logText);
                var accessLog = new Log(Log.TypeAccess, uid, logText, DateTime.Now.ToString());
                dataHelper.AddLog(accessLog); //add log to log list
            }
            else
            {
                // Log new cardID
                var newCardIdLog = new Log(
                    Log.TypeAccess,
                    pendingPin,
                    uid,
                    "Card \"" + uid + "\" has been added for pin \"" + pendingPin + "\".",
                    DateTime.Now.ToString());

                // Update CardID in userList
                dataHelper.AddCardId(pendingPin, uid);

                dataHelper.AddLog(newCardIdLog);

                var cardAddedAlert = new AlertWindow(WindowAlertPeriod);
                cardAddedAlert.SetText("NFC card added!");
                cardAddedAlert.SetPositiveButton("Ok", delegate { cardAddedAlert.Dismiss(); });
                cardAddedAlert.Show();
            }
        }

        /*
         * PIN FOUND EVENT:
         * This event occurs when the user inserts a pin code. It checks if the pin is valid and unlock the door if so.
         * If the pin has no related CardId it prompts the user to add it.
         */
        private void PinFound(string pin)
        {
            string masterPin = SettingsManager.Get(SettingsManager.MasterPin);

            // Check master pin
            if (String.Compare(masterPin, pin) == 0)
            {
                maintenanceWindow.Show();
                return;
            }
            
            // Check authorization
            var authorized = dataHelper.CheckPin(pin);
            var nullCardId = dataHelper.PinHasNullCardId(pin);

            // Log the event
            string logText;
            if (authorized)
            {
                // Access granted
                UnlockDoor();
                logText = "Pin \"" + pin + "\" inserted. Authorized access.";
            }
            else
            {
                // Access denied
                logText = "Pin \"" + pin + "\" inserted. Access denied!";
            }


            Debug.Print(logText);
            var accessLog = new Log(Log.TypeAccess, pin, logText, DateTime.Now.ToString());
            dataHelper.AddLog(accessLog); //add log to log list

            if (nullCardId)
            {
                // Null CardID detected, prompt the user to set one
                var nullCardIdAlert = new AlertWindow(WindowAlertPeriod);
                nullCardIdAlert.SetText(
                    "It happears that this user has no related NFC card.\nDo you want to scan it now?");
                nullCardIdAlert.SetPositiveButton("Yes", delegate
                {
                    // User wants to add a new NFC card
                    pendingPin = pin;
                    nullCardIdAlert.StopTimer(); // Hacky solution, but prevents graphical glitches
                    scanWindow.Show();
                });
                nullCardIdAlert.SetNegativeButton("No", delegate
                {
                    // User doesn't want to add a new NFC card
                    nullCardIdAlert.Dismiss();
                });
                nullCardIdAlert.Show();
            }
            else
            {
                // Everything is fine
                accessWindow.Show(authorized);
            }
        }

        /*
         * UNLOCK DOOR
         * Called by either PinFound or TagFound to unlock the door.
         */
        private void UnlockDoor()
        {
            //TODO
        }

        /*
         * DATA SOURCE CHANGED EVENT:
         * This event occurs when the data source for the user list changes.
         */
        private void DataSourceChanged(int dataSource)
        {
            pinWindow.SetDataSource(dataSource);
            if (dataSource == DataHelper.DataSourceError)
            {
                var dataSourceAlert = new AlertWindow(WindowAlertPeriod);
                dataSourceAlert.SetText(
                    "Unable to load dataset from cache! The system will remain offline until connection is established.");
                dataSourceAlert.SetPositiveButton("Ok", delegate { dataSourceAlert.Dismiss(); });
                dataSourceAlert.Show();
            }
        }

        /*
         * PIN WINDOW VISIBLE EVENT:
         * This event occours when the visibility of pinWindow changes.
         */
        private void WindowChanged(int id)
        {
            if (id == WindowPinId || id == WindowScanId)
                adafruit_PN532.StartScan(NfcScanPeriod, NfcScanTimeout);
            else
                adafruit_PN532.StopScan();
        }
    }
}