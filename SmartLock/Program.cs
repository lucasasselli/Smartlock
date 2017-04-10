using System;
using System.Collections;
using System.Threading;

using Microsoft.SPOT;

using GHI.Glide;
using GHI.Glide.Display;
using GT = Gadgeteer; 
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.Luca_Sasselli;

namespace SmartLock
{
    public partial class Program
    {
        // Constants
        private const int WINDOW_ACCESS_PERIOD = 2000;
        private const int WINDOW_ALERT_PERIOD = 10000;
        
        // Main Objects
        private PinWindow windowPin;
        private AccessWindow accessWindow;
        private AlertWindow scanWindow;
        private DataHelper dataHelper;

        public void ProgramStarted()
        {
            Debug.Print("Program started!");          

            // Object init
            windowPin = new PinWindow();
            accessWindow = new AccessWindow(windowPin.window, WINDOW_ACCESS_PERIOD);
            scanWindow = new AlertWindow(windowPin.window, WINDOW_ALERT_PERIOD);
            dataHelper = new DataHelper(ethernetJ11D, sdCard);

            // Event Setup
            adafruit_PN532.TagFound += TagFound;
            windowPin.PinFound += PinFound;
            dataHelper.DataSourceChanged += DataSourceChanged;

            // Set scan window
            scanWindow.SetText("Please scan your NFC card now...");
            scanWindow.SetNegativeButton("Cancel", delegate(Object target)
            {
                scanWindow.Dismiss();
            });

            dataHelper.Init();
        }

        /*
         * TAG FOUND EVENT:
         * This event occurs when the user passes a NFC card near the reader.
         * It checks if the UID is valid and unlock the door if so.
         */
        void TagFound(string uid)
        {
            // Check authorization
            bool authorized = dataHelper.CheckCardID(uid);

            // Show access window
            accessWindow.Show(authorized);

            // Log the event
            Log accessLog; //create a new log
            string logText;
            if (authorized)
            {
                // Access granted
                UnlockDoor();
                logText = "Card " + uid + " inserted. Authorized access.";
            }
            else
            {
                // Access denied
                logText = "Card " + uid + " inserted. Access denied!";
            }
            Debug.Print(logText);
            accessLog = new Log(2, logText, DateTime.Now.ToString());
            dataHelper.AddLog(accessLog); //add log to log list
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
            Log accessLog; //create a new log
            string logText;
            if (authorized)
            {
                // Access granted
                UnlockDoor();
                logText = "Pin " + pin + " inserted. Authorized access.";
            }
            else
            {
                // Access denied
                logText = "Pin " + pin + " inserted. Access denied!";
            }


            Debug.Print(logText);
            accessLog = new Log(2, logText, DateTime.Now.ToString());
            dataHelper.AddLog(accessLog); //add log to log list

            if (nullCardID)
            {
                // Null CardID detected, prompt the user to set one
                AlertWindow nullCardIDAlert = new AlertWindow(windowPin.window, WINDOW_ALERT_PERIOD);
                nullCardIDAlert.SetText("It happears that this user has no related NFC card.\nDo you want to scan it now?");
                nullCardIDAlert.SetPositiveButton("Yes", delegate(Object target)
                {
                    // User wants to add a new NFC card
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
            windowPin.SetDataSource(dataSource);
            if (dataSource == DataHelper.DATA_SOURCE_ERROR)
            {
                AlertWindow dataSourceAlert = new AlertWindow(windowPin.window, 10000);
                dataSourceAlert.SetText("Unable to load dataset from cache! The system will remain offline until connection is established.");
                dataSourceAlert.SetPositiveButton("Ok", delegate(Object target) 
                {
                    dataSourceAlert.Dismiss();
                });
                dataSourceAlert.Show();
            }
        }
    }
}


