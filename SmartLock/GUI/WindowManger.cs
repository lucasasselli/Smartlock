using GHI.Glide;
using GHI.Glide.Display;

namespace SmartLock.GUI
{
    /*
     * Wrapper class for Glide to allow swapping from/to a "mainWindow" and generating events when its visibility
     * changes.
     */

    public static class WindowManger
    {
        public static event VisibilityEventHandler VisibilityChanged;
        public delegate void VisibilityEventHandler(bool visible);

        public static Window MainWindow { get; set; }

        public static void ShowMainWindow()
        {
            Glide.MainWindow = MainWindow;
            VisibilityChanged(true);
        }

        public static void ShowWindow(Window window)
        {
            Glide.MainWindow = window;
            VisibilityChanged(false);
        }
    }
}
