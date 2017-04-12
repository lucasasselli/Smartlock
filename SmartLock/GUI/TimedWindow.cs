using GT = Gadgeteer;

namespace SmartLock.GUI
{
    /*
     * TimedWindow:
     * ManageableWindow that is shown on screen for a limited amount of time.
     */
    public abstract class TimedWindow : ManageableWindow
    {
        private readonly GT.Timer timerShowWindow;

        protected TimedWindow(int id, int period) : base(id)
        {
            timerShowWindow = new GT.Timer(period);
            timerShowWindow.Tick += timerShowWindow_Tick;
        }

        // Shows the window for "period" time
        public override void Show()
        {
            base.Show();
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

        // Returns true if the window is currently being displayed
        public bool IsShowing()
        {
            return timerShowWindow.IsRunning;
        }
    }
}