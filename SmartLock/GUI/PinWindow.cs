using GHI.Glide;
using GHI.Glide.UI;
using Microsoft.SPOT;

namespace SmartLock.GUI
{
    public class PinWindow : ManageableWindow
    {
        public delegate void PinEventHandler(string pin);

        // Constants
        private const int PinLength = 5; // pin max length

        private readonly Image dataSourceImm;

        // Pin Window element
        private readonly PasswordBox passwordBox;

        // Global variables
        private int numDigits; // Current number of digits inside the pin

        private string pin; // String of the pin

        public PinWindow(int id) : base(id)
        {
            // Data Setup
            numDigits = 0;

            // Load graphical resources
            Window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.PinWindow));

            // Initialization
            GlideTouch.Initialize();

            // Load window elements
            var bt0 = (Button) Window.GetChildByName("b0");
            var bt1 = (Button) Window.GetChildByName("b1");
            var bt2 = (Button) Window.GetChildByName("b2");
            var bt3 = (Button) Window.GetChildByName("b3");
            var bt4 = (Button) Window.GetChildByName("b4");
            var bt5 = (Button) Window.GetChildByName("b5");
            var bt6 = (Button) Window.GetChildByName("b6");
            var bt7 = (Button) Window.GetChildByName("b7");
            var bt8 = (Button) Window.GetChildByName("b8");
            var bt9 = (Button) Window.GetChildByName("b9");
            var btdel = (Button) Window.GetChildByName("bdel");
            var btac = (Button) Window.GetChildByName("bac");
            passwordBox = (PasswordBox) Window.GetChildByName("p1");
            dataSourceImm = (Image) Window.GetChildByName("data_source");

            bt0.TapEvent += bt0_TapEvent;
            bt1.TapEvent += bt1_TapEvent;
            bt2.TapEvent += bt2_TapEvent;
            bt3.TapEvent += bt3_TapEvent;
            bt4.TapEvent += bt4_TapEvent;
            bt5.TapEvent += bt5_TapEvent;
            bt6.TapEvent += bt6_TapEvent;
            bt7.TapEvent += bt7_TapEvent;
            bt8.TapEvent += bt8_TapEvent;
            bt9.TapEvent += bt9_TapEvent;
            btdel.TapEvent += btdel_TapEvent;
            btac.TapEvent += btac_TapEvent;

            // Current data source is unknown
            SetDataSource(DataHelper.DataSourceUnknown);
        }

        // Event handling
        public event PinEventHandler PinFound;

        public void SetDataSource(int dataSource)
        {
            switch (dataSource)
            {
                case DataHelper.DataSourceCache:
                    dataSourceImm.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.data_source_cache),
                        Bitmap.BitmapImageType.Bmp);
                    break;

                case DataHelper.DataSourceRemote:
                    dataSourceImm.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.data_source_remote),
                        Bitmap.BitmapImageType.Bmp);
                    break;

                default:
                    dataSourceImm.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.data_source_error),
                        Bitmap.BitmapImageType.Bmp);
                    break;
            }

            dataSourceImm.Render();
            dataSourceImm.Invalidate();
        }

        private void CheckPin()
        {
            numDigits++;
            if (numDigits == PinLength) //reached pin length
            {
                // Calls the PinFound event
                if (PinFound != null) PinFound(pin);

                // Reset interface
                numDigits = 0;
                pin = string.Empty; //clear pin
                passwordBox.Text = pin;
                passwordBox.Invalidate();
            }
        }

        private void bt0_TapEvent(object sender)
        {
            pin = pin + '0';
            passwordBox.Text = pin;
            passwordBox.Invalidate(); //update pinbox
            CheckPin();
        }

        private void bt1_TapEvent(object sender)
        {
            pin = pin + '1';
            passwordBox.Text = pin;
            passwordBox.Invalidate();
            CheckPin();
        }

        private void bt2_TapEvent(object sender)
        {
            pin = pin + '2';
            passwordBox.Text = pin;
            passwordBox.Invalidate();
            CheckPin();
        }

        private void bt3_TapEvent(object sender)
        {
            pin = pin + '3';
            passwordBox.Text = pin;
            passwordBox.Invalidate();
            CheckPin();
        }

        private void bt4_TapEvent(object sender)
        {
            pin = pin + '4';
            passwordBox.Text = pin;
            passwordBox.Invalidate();
            CheckPin();
        }

        private void bt5_TapEvent(object sender)
        {
            pin = pin + '5';
            passwordBox.Text = pin;
            passwordBox.Invalidate();
            CheckPin();
        }

        private void bt6_TapEvent(object sender)
        {
            pin = pin + '6';
            passwordBox.Text = pin;
            passwordBox.Invalidate();
            CheckPin();
        }

        private void bt7_TapEvent(object sender)
        {
            pin = pin + '7';
            passwordBox.Text = pin;
            passwordBox.Invalidate();
            CheckPin();
        }

        private void bt8_TapEvent(object sender)
        {
            pin = pin + '8';
            passwordBox.Text = pin;
            passwordBox.Invalidate();
            CheckPin();
        }

        private void bt9_TapEvent(object sender)
        {
            pin = pin + '9';
            passwordBox.Text = pin;
            passwordBox.Invalidate();
            CheckPin();
        }

        private void btdel_TapEvent(object sender)
        {
            if (pin.Length != 0)
            {
                pin = pin.Substring(0, pin.Length - 1);
                passwordBox.Text = pin;
                passwordBox.Invalidate();
                numDigits--;
            }
        }

        private void btac_TapEvent(object sender)
        {
            pin = string.Empty; //clear pin
            passwordBox.Text = pin;
            passwordBox.Invalidate();
            numDigits = 0;
        }
    }
}