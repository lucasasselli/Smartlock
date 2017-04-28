using GHI.Glide;
using GHI.Glide.UI;
using Microsoft.SPOT;
using GT = Gadgeteer;

namespace SmartLock.GUI
{
    /*
     * TimedWindow:
     * ManageableWindow that is shown on screen for a limited amount of time.
     */
    public abstract class TimedWindow : WindowManager.ManageableWindow
    {
        private readonly GT.Timer timerShowWindow;
        private bool running;

        protected TimedWindow(int period) : base(false)
        {
            timerShowWindow = new GT.Timer(period);
            timerShowWindow.Tick += OnTick;
        }

        // Shows the window for "period" time
        public override void Show()
        {
            running = true;
            base.Show();
            timerShowWindow.Restart();
        }

        // Dismisses the window before "period" has expired
        public void Dismiss()
        {
            StopTimer();
            WindowManager.Back();
        }

        // Makes the window static
        public void StopTimer()
        {
            running = false;
            timerShowWindow.Stop();
        }

        // Remove second window
        private void OnTick(GT.Timer timerAccessWindow)
        {
            if (running)
            {
                Dismiss();
            }
        }

        // Remove second window
        protected void OnClose(object sender)
        {
            StopTimer();
        }

        // Returns true if the window is currently being displayed
        public bool IsShowing()
        {
            return timerShowWindow.IsRunning;
        }
    }

    /*
     * AlertWindow:
     * TimedWindow with optional multichoice events.
     */
    public class AlertWindow : TimedWindow
    {
        private readonly TextBlock alertText;
        private readonly Button negativeButton;
        private readonly Button positiveButton;

        public AlertWindow(int period)
            : base(period)
        {
            Window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.AlertWindow));
            Window.CloseEvent += OnClose;

            // Load access window elements
            alertText = (TextBlock)Window.GetChildByName("alert_text");
            positiveButton = (Button)Window.GetChildByName("positive_button");
            negativeButton = (Button)Window.GetChildByName("negative_button");

            alertText.Visible = false;
            positiveButton.Visible = false;
            negativeButton.Visible = false;
        }

        public void SetPositiveButton(string positiveText, OnPress pressedEvent)
        {
            positiveButton.Visible = true;
            positiveButton.Text = positiveText;
            positiveButton.PressEvent += pressedEvent;
        }

        public void SetNegativeButton(string negativeText, OnPress pressedEvent)
        {
            negativeButton.Visible = true;
            negativeButton.Text = negativeText;
            negativeButton.PressEvent += pressedEvent;
        }

        public void SetText(string alertTextString)
        {
            alertText.Visible = true;
            alertText.Text = alertTextString;
        }
    }

    /*
     * AccessWindow:
     * TimedWindow to show the result of login. 
     */
    public class AccessWindow : TimedWindow
    {
        private readonly Image accessImage;
        private readonly TextBlock accessText;

        public AccessWindow(int period)
            : base(period)
        {
            Window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.AccessWindow));
            Window.CloseEvent += OnClose;

            // Load access window elements
            accessImage = (Image)Window.GetChildByName("access_imm");
            accessText = (TextBlock)Window.GetChildByName("access_tb");
        }

        public void Show(bool mode)
        {
            if (mode)
            {
                accessImage.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.alert_ok),
                    Bitmap.BitmapImageType.Jpeg);
                accessText.Text = "Access Allowed!";
            }
            else
            {
                accessImage.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.alert_alt),
                    Bitmap.BitmapImageType.Jpeg);
                accessText.Text = "Access Denied!";
            }

            accessImage.Render(); // Adapt to imagebox
            accessImage.Invalidate(); // Send image to display
            accessText.Invalidate(); // Send text to display

            base.Show();
        }
    }
}