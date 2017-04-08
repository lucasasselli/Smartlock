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
        // Define
        private const int pswLength = 5; // Password max length
        private const int timerSecondWindowCount = 1500; // 1.5 sec

        // Global variables
        private int numDigits; // Current number of digits inside the password
        private string password; // String of the password
        
        // Timers        
        private GT.Timer timerSecondWindow; // Timer for access or denied window

        private DataHelper dataHelper;

        public void ProgramStarted()
        {
            Debug.Print("Program started!");
            
            // Display Setup
            numDigits = 0;
            Display_Initialize();

            timerSecondWindow = new GT.Timer(timerSecondWindowCount); // 1.5 sec
            timerSecondWindow.Tick += timerSecondWindow_Tick;

            // Data Setup
            dataHelper = new DataHelper(ethernetJ11D, sdCard);

            // NFC Setup
            adafruit_PN532.TagFound += TagFound;
            adafruit_PN532.StartScan(1000, 100);
        }

        // Unlock the Door
        private void UnlockDoor()
        {
            //TODO
        }
    }
}


