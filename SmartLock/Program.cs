using System;
using System.Collections;
using System.Threading;

using Microsoft.SPOT;

using GHI.Glide;
using GHI.Glide.Display;
using GT = Gadgeteer; //GT = because the ambiguous reference to system.threading.timers and Gadgeteer.timer
using Gadgeteer.Modules.GHIElectronics;

namespace SmartLock
{
    public partial class Program
    {
        // Define
        private const int pswLength = 5; //password max length
        private const int timerSecondWindowCount = 1500; //1.5 sec

        // Global variables
        private int numDigits; //current number of digits inside the password
        private string password; //string of the password
        
        // Timers        
        private GT.Timer timerSecondWindow; //timer for access or denied window
        private DataHelper dataHelper; // TODO move to wrapper

        public void ProgramStarted()
        {
            Debug.Print("Program started!");
            
            numDigits = 0;
            Display_Initialize();

            timerSecondWindow = new GT.Timer(timerSecondWindowCount); // 1.5 sec
            timerSecondWindow.Tick += timerSecondWindow_Tick;

            dataHelper = new DataHelper(ethernetJ11D, sdCard); // TODO move to wrapper
        }

        private void timerSecondWindow_Tick(GT.Timer timerSecondWindow)
        {
            Glide.MainWindow = PinWindow; //back to main window
            timerSecondWindow.Stop();
        }

        private void unlockDoor()
        {
            //TODO
        }
    }
}


