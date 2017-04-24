using GHI.Glide;
using GHI.Glide.UI;

namespace SmartLock.GUI
{
    /*
     * AlertWindow:
     * TimedWindow with optional multichoice events.
     */
    public class AlertWindow : TimedWindow
    {
        private readonly TextBlock alertText;
        private readonly Button negativeButton;
        private readonly Button positiveButton;

        public AlertWindow(int period) : base(period)
        {
            Window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.AlertWindow));

            // Load access window elements
            alertText = (TextBlock) Window.GetChildByName("alert_text");
            positiveButton = (Button) Window.GetChildByName("positive_button");
            negativeButton = (Button) Window.GetChildByName("negative_button");

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
}