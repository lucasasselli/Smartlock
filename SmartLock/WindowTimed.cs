using System;
using Microsoft.SPOT;

using GHI.Glide;
using GHI.Glide.Display;
using GT = Gadgeteer; 

namespace SmartLock
{
    public abstract class WindowTimed
    {
        private Window fallbackWindow;
        private Window timedWindow;

        // Timers        
        private GT.Timer timerShowWindow;

        public WindowTimed(Window fallbackWindow, int period)
        {
            this.fallbackWindow = fallbackWindow;

            timedWindow = new Window();

            timerShowWindow = new GT.Timer(period); // 1.5 sec
            timerShowWindow.Tick += timerShowWindow_Tick;
        }

        protected void Show()
        {
            Glide.MainWindow = timedWindow;
            timerShowWindow.Start();
        }

        // Remove second window
        private void timerShowWindow_Tick(GT.Timer timerAccessWindow)
        {
            Glide.MainWindow = fallbackWindow;
            timerShowWindow.Stop();
        }

        protected void SetWindow(Window timedWindow){
            this.timedWindow = timedWindow;
        }
    }
}
