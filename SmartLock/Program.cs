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
        // Main Objects
        private Display display;
        private DataHelper dataHelper;

        public void ProgramStarted()
        {
            Debug.Print("Program started!");          

            // Object init
            display = new Display();
            dataHelper = new DataHelper(ethernetJ11D, sdCard);

            // Event Setup
            adafruit_PN532.TagFound += TagFound;
            display.PinFound += PinFound;
            dataHelper.DataSourceChanged += DataSourceChanged;

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
            display.ShowAccessWindow(authorized);

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
                WindowAlert nullCardIDAlert = new WindowAlert(display.PinWindow, 10000);
                nullCardIDAlert.setText("It happears that this user has no card.\nDo you want to scan it now?");
                nullCardIDAlert.setPositiveButton("Yes", null);
                nullCardIDAlert.setNegativeButton("No", delegate(Object target)
                {
                    nullCardIDAlert.Dismiss();
                });
                nullCardIDAlert.Show();
            }
            else
            {
                display.ShowAccessWindow(authorized);
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
            display.SetDataSource(dataSource);
            if (dataSource == DataHelper.DATA_SOURCE_ERROR)
            {
                WindowAlert dataSourceAlert = new WindowAlert(display.PinWindow, 10000);
                dataSourceAlert.setText("Unable to load dataset from cache! The system will remain offline until connection is established.");
                dataSourceAlert.setPositiveButton("Ok", delegate(Object target) 
                {
                    dataSourceAlert.Dismiss();
                });
                dataSourceAlert.Show();
            }
        }
    }
}


