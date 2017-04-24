using GHI.Glide;
using GHI.Glide.UI;
using Microsoft.SPOT;

namespace SmartLock.GUI
{
    /*
     * AccessWindow:
     * TimedWindow to show the result of login. 
     */
    public class AccessWindow : TimedWindow
    {
        private readonly Image accessImage;
        private readonly TextBlock accessText;

        public AccessWindow(int period) : base(period)
        {
            Window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.AccessWindow));

            // Load access window elements
            accessImage = (Image) Window.GetChildByName("access_imm");
            accessText = (TextBlock) Window.GetChildByName("access_tb");
        }

        public void Show(bool mode)
        {
            if (mode)
            {
                accessImage.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.alert_ok),
                    Bitmap.BitmapImageType.Bmp);
                accessText.Text = "Access Allowed!";
            }
            else
            {
                accessImage.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.alert_alt),
                    Bitmap.BitmapImageType.Bmp);
                accessText.Text = "Access Denied!";
            }

            accessImage.Render(); // Adapt to imagebox
            accessImage.Invalidate(); // Send image to display
            accessText.Invalidate(); // Send text to display

            base.Show();
        }
    }
}