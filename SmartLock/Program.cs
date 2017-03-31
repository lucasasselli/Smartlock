using System;
using System.Collections;
using System.Threading;

using GHI.Glide;
using GHI.Glide.Display;

namespace SmartLock
{
    public partial class Program
    {
        private const int pswLength = 5; //password max length
        private int numDigits; //current number of digits inside the password

        private Window window = new Window();
        private string password; //string of the password
        
        public void ProgramStarted()
        {
            numDigits = 0;
            window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.window));
            GlideTouch.Initialize();
            Display_Initialize();

            ethernetJ11D.UseThisNetworkInterface();
            ethernetJ11D.UseStaticIP("192.168.100.2", "255.255.255.0", "0.0.0.0");
            ethernetJ11D.NetworkUp += ethernetJ11D_NetworkUp;
            ethernetJ11D.NetworkDown += ethernetJ11D_NetworkDown;

            Glide.MainWindow = window;
            Thread.Sleep(Timeout.Infinite);
        } 
    }
}


