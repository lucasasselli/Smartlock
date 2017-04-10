using System;
using Microsoft.SPOT;

using GHI.Glide;
using GHI.Glide.Display;
using GT = Gadgeteer; 

namespace SmartLock
{
    /*
     * Abstract class to generate and display timed windows.
     * The window must be created in an overridden constuctor and then set with the "SetWindow" method.
     * The constructor takes two arguments: "fallbackWindow" is the window to wich the WindowTimed returs after the timer 
     * has expired and "period" wich is the timer period.
     */

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

        // Shows the window for "period" time
        public void Show()
        {
            Glide.MainWindow = timedWindow;
            timerShowWindow.Start();
        }

        // Dismisses the window before "period" has expired
        public void Dismiss()
        {
            Glide.MainWindow = fallbackWindow;
            timerShowWindow.Stop();
        }

        // Remove second window
        private void timerShowWindow_Tick(GT.Timer timerAccessWindow)
        {
            Dismiss();
        }

        protected void SetWindow(Window timedWindow){
            this.timedWindow = timedWindow;
        }
    }
}
