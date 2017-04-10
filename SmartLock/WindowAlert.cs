using System;
using Microsoft.SPOT;

using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using GT = Gadgeteer; 

namespace SmartLock
{
    public class WindowAlert : WindowTimed
    {
        private Button positiveButton;
        private Button negativeButton;

        private TextBlock alertText;

        public WindowAlert(Window fallbackWindow, int period) : base(fallbackWindow, period)
        {
            Window window = new Window();
            window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.AlertWindow));

            // Load access window elements
            alertText = (TextBlock) window.GetChildByName("alert_text");
            positiveButton = (Button) window.GetChildByName("positive_button");
            negativeButton = (Button)window.GetChildByName("negative_button");

            alertText.Visible = false;
            positiveButton.Visible = false;
            negativeButton.Visible = false;

            SetWindow(window);
        }

        public void setPositiveButton(String positiveText, OnPress PressedEvent)
        {
            positiveButton.Visible = true;
            positiveButton.Text = positiveText;
            positiveButton.PressEvent += PressedEvent;
        }

        public void setNegativeButton(String negativeText, OnPress PressedEvent)
        {
            negativeButton.Visible = true;
            negativeButton.Text = negativeText;
            negativeButton.PressEvent += PressedEvent;
        }

        public void setText(string alertTextString)
        {
            alertText.Visible = true;
            alertText.Text = alertTextString;
        }
    }
}
