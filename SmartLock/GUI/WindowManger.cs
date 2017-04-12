using GHI.Glide;
using GHI.Glide.Display;

namespace SmartLock.GUI
{
    /* 
     * WindowManager:
     * allow to control the on screen window and generate events.
     */
    public static class WindowManger
    {
        public delegate void WindowEventHandler(int windowId);

        public static ManageableWindow MainWindow { get; set; }
        public static event WindowEventHandler WindowChanged;

        // Shows the main window
        public static void ShowMainWindow()
        {
            Glide.MainWindow = MainWindow.Window;
            if (WindowChanged != null) WindowChanged(MainWindow.Id);
        }

        // Shows the manageable window
        public static void ShowWindow(ManageableWindow manageable)
        {
            Glide.MainWindow = manageable.Window;
            if (WindowChanged != null) WindowChanged(manageable.Id);
        }
    }

    /*
     * ManageableWindow:
     * window wrapper to be used with WindowManager. Adds an id to each windows that is passed to the "WindowChanged" event.
     */
    public class ManageableWindow
    {
        public ManageableWindow(int id)
        {
            Id = id;
        }

        public Window Window { get; protected set; }
        public int Id { get; private set; }

        // Shows the this window
        public virtual void Show()
        {
            WindowManger.ShowWindow(this);
        }
    }
}