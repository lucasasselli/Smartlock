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
    public static class WindowManager
    {

        public delegate void WindowEventHandler(int windowId);
        public static event WindowEventHandler WindowChanged;

        private static ArrayList windowStack = new ArrayList();

        public static void Init()
        {
            GlideTouch.Initialize();
        }

        internal static void Push(ManageableWindow manageable)
        {
            int lastIndex = getLastIndex();

            if(lastIndex >= 0)
            {
                ManageableWindow currentWindow = windowStack[lastIndex] as ManageableWindow;
                currentWindow.Window.TriggerCloseEvent(null);

                // If the current window is not persistent replace it
                if(!currentWindow.Persistent)
                {
                    windowStack.RemoveAt(lastIndex);
                }
            }

            // Add window to stack
            windowStack.Add(manageable);

            // Show window
            Glide.MainWindow = manageable.Window;

            // Notify event
            notifyChanged(manageable);
        }

        public static void Back()
        {
            int lastIndex = getLastIndex();

            if (lastIndex < 0)
            {
                // Error popping empty window stack
                DebugOnly.Print("ERROR: Window stack is empty!");
            }
            else
            {
                // Remove last window from the stack
                windowStack.RemoveAt(lastIndex);

                // Show previous window
                ManageableWindow manageable = windowStack[lastIndex - 1] as ManageableWindow;
                Glide.MainWindow = manageable.Window;
                notifyChanged(manageable);
            }
        }

        private static int getLastIndex()
        {
            return windowStack.Count - 1;
        }

        internal static ManageableWindow getLastWindow()
        {
            int lastIndex = getLastIndex();
            if (lastIndex >= 0)
            {
                return windowStack[lastIndex] as ManageableWindow;
            }
            else
            {
                return null;
            }
        }

        private static void notifyChanged(ManageableWindow manageable)
        {
            if (WindowChanged != null) WindowChanged(manageable.Id);
        }

        /*
         * ManageableWindow:
         * window wrapper to be used with WindowManager. Adds an id to each windows that is passed to the "WindowChanged" event.
         */
        public class ManageableWindow
        {
            private const int defaultId = -1;

            public Window Window { get; protected set; }
            public int Id { get; set; }
            public bool Persistent { get; private set; }

            internal int stackPosition { get; set; }

            public ManageableWindow()
            {
                Id = defaultId;
                Persistent = true;
            }

            public ManageableWindow(bool persistent)
            {
                Id = defaultId;
                Persistent = persistent;
            }

            // Shows the this window
            public virtual void Show()
            {
                WindowManager.Push(this);
            }         
        }
    }
}