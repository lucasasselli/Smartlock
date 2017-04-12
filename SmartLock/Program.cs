using System;
using System.Collections;
using System.Threading;

using Microsoft.SPOT;

using GHI.Glide;
using GHI.Glide.Display;
using GT = Gadgeteer; 
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.Luca_Sasselli;
using SmartLock.GUI;

namespace SmartLock
{
    public partial class Program
    {
        // Time constants
        private const int WINDOW_ACCESS_PERIOD = 2000;
        private const int WINDOW_ALERT_PERIOD = 10000;
        private const int NFC_SCAN_PERIOD = 1000;
        private const int NFC_SCAN_TIMEOUT = 200;

        // Window ID constants
        private const int WINDOW_PIN_ID = 0;
        private const int WINDOW_ACCESS_ID = 1;
        private const int WINDOW_ALERT_ID = 2;
        private const int WINDOW_SCAN_ID = 3;
        
        // Main Objects
        private PinWindow pinWindow;
        private AccessWindow accessWindow;
        private AlertWindow scanWindow;
        private DataHelper dataHelper;

        // Nfc setup
        private string pendingPin;

        public void ProgramStarted()
        {
            Debug.Print("Program started!");          

            // Object init
            pinWindow = new PinWindow(WINDOW_PIN_ID);
            accessWindow = new AccessWindow(WINDOW_ACCESS_ID, WINDOW_ACCESS_PERIOD);
            scanWindow = new AlertWindow(WINDOW_SCAN_ID, WINDOW_ALERT_PERIOD);
            dataHelper = new DataHelper(ethernetJ11D, sdCard);

            // Event Setup
            adafruit_PN532.TagFound += TagFound;
            pinWindow.PinFound += PinFound;
            dataHelper.DataSourceChanged += DataSourceChanged;

            // Set scan window
            scanWindow.SetText("Please scan your NFC card now...");
            scanWindow.SetNegativeButton("Cancel", delegate(Object target)
            {
                scanWindow.Dismiss();
            });

            dataHelper.Init();

            WindowManger.MainWindow = pinWindow;
            WindowManger.WindowChanged += WindowChanged;
            WindowManger.ShowMainWindow();
        }

        /*
         * TAG FOUND EVENT:
         * This event occurs when the user passes a NFC card near the reader.
         * It checks if the UID is valid and unlock the door if so.
         */
        void TagFound(string uid)
        {
            if (!scanWindow.IsShowing())
            {
                // Check authorization
                bool authorized = dataHelper.CheckCardID(uid);

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
                Log accessLog = new Log(Log.TYPE_ACCESS, uid, logText, DateTime.Now.ToString());
                dataHelper.AddLog(accessLog); //add log to log list
            }
            else
            {
                // Log new cardID
                Log newCardIDLog = new Log(
                    Log.TYPE_ACCESS, 
                    pendingPin, 
                    uid, 
                    "Card \"" + uid + "\" has been added for pin \"" + pendingPin + "\".", 
                    DateTime.Now.ToString());

                // Update CardID in userList
                dataHelper.AddCardID(pendingPin, uid);

                dataHelper.AddLog(newCardIDLog);

                AlertWindow cardAddedAlert = new AlertWindow(WINDOW_ALERT_ID, WINDOW_ALERT_PERIOD);
                cardAddedAlert.SetText("NFC card added!");
                cardAddedAlert.SetPositiveButton("Ok", delegate(Object target)
                {
                    cardAddedAlert.Dismiss();
                });
                cardAddedAlert.Show();
            }
        }

        /*
         * PIN FOUND EVENT:
         * This event occurs when the user inserts a pin code. It checks if the pin is valid and unlock the door if so.
         * If the pin has no related CardId it prompts the user to add it.
         */
        void PinFound(string pin)
        {
            // Check authorization
            bool authorized = dataHelper.CheckPin(pin);
            bool nullCardID = dataHelper.PinHasNullCardID(pin);

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
            Log accessLog = new Log(Log.TYPE_ACCESS, pin, logText, DateTime.Now.ToString());
            dataHelper.AddLog(accessLog); //add log to log list

            if (nullCardID)
            {
                // Null CardID detected, prompt the user to set one
                AlertWindow nullCardIDAlert = new AlertWindow(WINDOW_ALERT_ID, WINDOW_ALERT_PERIOD);
                nullCardIDAlert.SetText("It happears that this user has no related NFC card.\nDo you want to scan it now?");
                nullCardIDAlert.SetPositiveButton("Yes", delegate(Object target)
                {
                    // User wants to add a new NFC card
                    pendingPin = pin;
                    nullCardIDAlert.StopTimer(); // Hacky solution, but prevents graphical glitches
                    scanWindow.Show();
                });
                nullCardIDAlert.SetNegativeButton("No", delegate(Object target)
                {
                    // User doesn't want to add a new NFC card
                    nullCardIDAlert.Dismiss();
                });
                nullCardIDAlert.Show();
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
        void UnlockDoor()
        {
            //TODO
        }

        /*
         * DATA SOURCE CHANGED EVENT:
         * This event occurs when the data source for the user list changes.
         */
        void DataSourceChanged(int dataSource)
        {
            pinWindow.SetDataSource(dataSource);
            if (dataSource == DataHelper.DATA_SOURCE_ERROR)
            {
                AlertWindow dataSourceAlert = new AlertWindow(WINDOW_ALERT_ID, WINDOW_ALERT_PERIOD);
                dataSourceAlert.SetText("Unable to load dataset from cache! The system will remain offline until connection is established.");
                dataSourceAlert.SetPositiveButton("Ok", delegate(Object target) 
                {
                    dataSourceAlert.Dismiss();
                });
                dataSourceAlert.Show();
            }
        }

        /*
         * PIN WINDOW VISIBLE EVENT:
         * This event occours when the visibility of pinWindow changes.
         */

        void WindowChanged(int ID)
        {
            if (ID == WINDOW_PIN_ID || ID == WINDOW_SCAN_ID)
            {
                adafruit_PN532.StartScan(NFC_SCAN_PERIOD, NFC_SCAN_TIMEOUT);
            }
            else
            {
                // Pin window not visible
                adafruit_PN532.StopScan();
            }
        }
    }
}

