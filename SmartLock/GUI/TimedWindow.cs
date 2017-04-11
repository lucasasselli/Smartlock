using GHI.Glide.Display;
using GT = Gadgeteer; 

namespace SmartLock.GUI
{
    /*
     * Abstract wrapper class to generate and display timed windows.
     * The window must be created in an overridden constuctor and then set with the "SetWindow" method.
     * The constructor takes two arguments: "fallbackWindow" is the window to wich the TimedWindow returs after the timer 
     * has expired and "period" wich is the timer period.
     */

    public abstract class TimedWindow
    {
        private Window timedWindow;

        // Timers        
        private GT.Timer timerShowWindow;

        public TimedWindow(int period)
        {
            timerShowWindow = new GT.Timer(period);
            timerShowWindow.Tick += timerShowWindow_Tick;
        }

        // Shows the window for "period" time
        public void Show()
        {
            WindowManger.ShowWindow(timedWindow);
            timerShowWindow.Start();
        }

        // Dismisses the window before "period" has expired
        public void Dismiss()
        {
            WindowManger.ShowMainWindow();
            timerShowWindow.Stop();
        }

        // Makes the window static
        public void StopTimer()
        {
            timerShowWindow.Stop();
        }

        // Remove second window
        private void timerShowWindow_Tick(GT.Timer timerAccessWindow)
        {
            Dismiss();
        }

        // Sets the wrapped window
        public void SetWindow(Window timedWindow)
        {
            this.timedWindow = timedWindow;
        }

        // Returns true if the window is currently being displayed
        public bool IsShowing()
        {
            return timerShowWindow.IsRunning;
        }
    }
}
