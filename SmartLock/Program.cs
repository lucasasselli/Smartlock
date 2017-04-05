using System;
using System.Collections;
using System.Threading;

using GHI.Glide;
using GHI.Glide.Display;
using GT = Gadgeteer; //GT = because the ambiguous reference to system.threading.timers and Gadgeteer.timer
using Gadgeteer.Modules.GHIElectronics;

namespace SmartLock
{
    public partial class Program
    {
        private static bool flagConnectionOn; //online/offline
        private static bool flagFirstConnection; //first connection after power up
        private static bool flagAuthorizedAccess; //access authorized/denied
        private static bool flagEmptyUserList;
        private static bool flagPendingLog; //there are logs to be sent to database
        private ArrayList UserList = new ArrayList(); //user list
        private ArrayList Logs = new ArrayList(); //log list
        private const int pswLength = 5; //password max length
        private int numDigits; //current number of digits inside the password
        private GT.Timer timerServerReq;

        private string password; //string of the password
        
        public void ProgramStarted()
        {
            flagConnectionOn = false;
            flagFirstConnection = true;
            flagAuthorizedAccess = false;
            flagEmptyUserList = true;
            flagPendingLog = false;
            numDigits = 0;
            Display_Initialize();
            timerServerReq = new GT.Timer(120000); //milliseconds -> 2 min
            timerServerReq.Tick += timerServerReq_Tick;

            ethernetJ11D.UseThisNetworkInterface();
            ethernetJ11D.UseDHCP();
            ethernetJ11D.NetworkUp += ethernetJ11D_NetworkUp;
            ethernetJ11D.NetworkDown += ethernetJ11D_NetworkDown;

            Thread.Sleep(Timeout.Infinite); //sleep forever
        }

        private void timerServerReq_Tick(GT.Timer timerServerReq)
        {
            if (flagConnectionOn)
                ServerRequest();
        }

        private void unlockDoor()
        {
            //TODO
        }
    }
}


