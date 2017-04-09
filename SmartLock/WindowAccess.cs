using System;
using Microsoft.SPOT;

using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using GT = Gadgeteer; 

namespace SmartLock
{
    public class WindowAccess : WindowTimed
    {
        private Image accessImage;
        private TextBlock accessText;

        public WindowAccess(Window fallbackWindow, int period) : base(fallbackWindow, period)
        {
            Window window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.AccessWindow));

            // Load access window elements
            accessImage = (Image) window.GetChildByName("access_imm");
            accessText = (TextBlock) window.GetChildByName("access_tb");

            SetWindow(window);
        }

        public void Show(bool mode)
        {
            if (mode)
            {

                accessImage.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.alert_ok), Bitmap.BitmapImageType.Bmp);
                accessText.Text = "Access Allowed!";
            }
            else
            {
                accessImage.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.alert_alt), Bitmap.BitmapImageType.Bmp);
                accessText.Text = "Access Denied!";
            }

            accessImage.Render(); // Adapt to imagebox
            accessImage.Invalidate(); // Send image to display
            accessText.Invalidate(); // Send text to display

            base.Show();
        }
    }
}
