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
            dataHelper.DataSourceChanged += display.SetDataSource;

            // Set initial data source
            display.SetDataSource(dataHelper.GetDataSource());
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
         * This event occurs when the user inserts a pin code.
         * It checks if the pin is valid and unlock the door if so.
         */
        void PinFound(string pin)
        {
            // Check authorization
            bool authorized = dataHelper.CheckPin(pin);

            // Show access window
            display.ShowAccessWindow(authorized);

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

            // TODO: if UID is null ask to add card
        }

        /*
         * UNLOCKDOOR
         * Called by either PinFound or TagFound to unlock the door.
         */
        void UnlockDoor()
        {
            //TODO
        }
    }
}


