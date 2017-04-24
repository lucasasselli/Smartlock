using Microsoft.SPOT;
using System.Collections;
using GHI.Glide;
using GHI.Glide.UI;
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

        private static ArrayList windowStack = new ArrayList();

        public static void Show(ManageableWindow manageable)
        {
            windowStack.Add(manageable);
            Glide.MainWindow = manageable.Window;
            notifyChanged(MainWindow);
        }

        public static void Back()
        {
            int lastWindow = windowStack.Count-1;
            if (lastWindow < 0)
            {
                // Error popping empty window stack
                Debug.Print("ERROR: Window stack is empty!");
            }
            else
            {
                // Remove last window from the stack
                windowStack.RemoveAt(lastWindow);

                if (lastWindow == 0)
                {
                    Glide.MainWindow = MainWindow.Window;
                    notifyChanged(MainWindow);
                }
                else
                {
                    // Show previous window
                    ManageableWindow manageable = windowStack[lastWindow - 1] as ManageableWindow;
                    Glide.MainWindow = manageable.Window;
                    notifyChanged(manageable);
                }
            }
        }

        private static void notifyChanged(ManageableWindow manageable)
        {
            if (WindowChanged != null && manageable.Id != -1) WindowChanged(manageable.Id);
        }
    }

    /*
     * ManageableWindow:
     * window wrapper to be used with WindowManager. Adds an id to each windows that is passed to the "WindowChanged" event.
     */
    public class ManageableWindow
    {
        private const int defaultId = -1;

        public ManageableWindow()
        {
            Id = defaultId;
        }

        public Window Window { get; protected set; }
        public int Id { get; set; }

        // Shows the this window
        public virtual void Show()
        {
            WindowManger.Show(this);
        }

        public virtual void Dismiss()
        {
            WindowManger.Back();
        }
    }
}