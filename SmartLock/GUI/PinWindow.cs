using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;

using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using GT = Gadgeteer; 

namespace SmartLock
{
    public class PinWindow
    {
        // Constants
        private const int pinLength = 5; // pin max length
        private const int timerSecondWindowCount = 1500; // 1.5 sec

        // Event handling
        public event PinEventHandler PinFound;
        public delegate void PinEventHandler(string pin);

        // Global variables
        private int numDigits; // Current number of digits inside the pin
        private string pin; // String of the pin

        public Window window = new Window();

        // Pin Window element
        private PasswordBox pb1;
        private Button bt0;
        private Button bt1;
        private Button bt2;
        private Button bt3;
        private Button bt4;
        private Button bt5;
        private Button bt6;
        private Button bt7;
        private Button bt8;
        private Button bt9;
        private Button btac;
        private Button btdel;
        private Image dataSourceImm;

        public PinWindow()
        {
            // Data Setup
            numDigits = 0;
            
            // Load graphical resources
            window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.PinWindow));
        
            // Initialization
            GlideTouch.Initialize();

            // Load pin window elements
            bt0 = (Button) window.GetChildByName("b0");
            bt1 = (Button) window.GetChildByName("b1");
            bt2 = (Button) window.GetChildByName("b2");
            bt3 = (Button) window.GetChildByName("b3");
            bt4 = (Button) window.GetChildByName("b4");
            bt5 = (Button) window.GetChildByName("b5");
            bt6 = (Button) window.GetChildByName("b6");
            bt7 = (Button) window.GetChildByName("b7");
            bt8 = (Button) window.GetChildByName("b8");
            bt9 = (Button) window.GetChildByName("b9");
            btdel = (Button) window.GetChildByName("bdel");
            btac = (Button) window.GetChildByName("bac");
            pb1 = (PasswordBox) window.GetChildByName("p1");
            dataSourceImm = (Image)window.GetChildByName("data_source");

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

            // Set Pin window as main window
            Showwindow();

            // Current data source is unknown
            SetDataSource(DataHelper.DATA_SOURCE_UNKNOWN);
        }

        // Show Pin window
        public void Showwindow()
        {
            Glide.MainWindow = window;
        }

        public void SetDataSource(int dataSource)
        {
            switch (dataSource)
            {
                case DataHelper.DATA_SOURCE_CACHE:
                    dataSourceImm.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.data_source_cache), Bitmap.BitmapImageType.Bmp);
                    break;

                case DataHelper.DATA_SOURCE_REMOTE:
                    dataSourceImm.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.data_source_remote), Bitmap.BitmapImageType.Bmp);
                    break;

                default:
                    dataSourceImm.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.data_source_error), Bitmap.BitmapImageType.Bmp);
                    break;
            }

            dataSourceImm.Render();
            dataSourceImm.Invalidate();
        }

        private void CheckPin()
        {
            numDigits++;
            if (numDigits == pinLength) //reached pin length
            {
                // Calls the PinFound event
                PinFound(pin);

                // Reset interface
                numDigits = 0;
                pin = String.Empty; //clear pin
                pb1.Text = pin;
                pb1.Invalidate();
            }
        }

        private void bt0_TapEvent(object sender)
        {
            pin = pin + '0';
            pb1.Text = pin;
            pb1.Invalidate(); //update pinbox
            CheckPin();
        }

        private void bt1_TapEvent(object sender)
        {
            pin = pin + '1';
            pb1.Text = pin;
            pb1.Invalidate();
            CheckPin();
        }

        private void bt2_TapEvent(object sender)
        {
            pin = pin + '2';
            pb1.Text = pin;
            pb1.Invalidate();
            CheckPin();
        }

        private void bt3_TapEvent(object sender)
        {
            pin = pin + '3';
            pb1.Text = pin;
            pb1.Invalidate();
            CheckPin();
        }

        private void bt4_TapEvent(object sender)
        {
            pin = pin + '4';
            pb1.Text = pin;
            pb1.Invalidate();
            CheckPin();
        }

        private void bt5_TapEvent(object sender)
        {
            pin = pin + '5';
            pb1.Text = pin;
            pb1.Invalidate();
            CheckPin();
        }

        private void bt6_TapEvent(object sender)
        {
            pin = pin + '6';
            pb1.Text = pin;
            pb1.Invalidate();
            CheckPin();
        }

        private void bt7_TapEvent(object sender)
        {
            pin = pin + '7';
            pb1.Text = pin;
            pb1.Invalidate();
            CheckPin();
        }

        private void bt8_TapEvent(object sender)
        {
            pin = pin + '8';
            pb1.Text = pin;
            pb1.Invalidate();
            CheckPin();
        }

        private void bt9_TapEvent(object sender)
        {
            pin = pin + '9';
            pb1.Text = pin;
            pb1.Invalidate();
            CheckPin();
        }

        private void btdel_TapEvent(object sender)
        {
            if (pin.Length != 0)
            {
                pin = pin.Substring(0, pin.Length - 1);
                pb1.Text = pin;
                pb1.Invalidate();
                numDigits--;
            }
        }

        private void btac_TapEvent(object sender)
        {
            pin = String.Empty; //clear pin
            pb1.Text = pin;
            pb1.Invalidate();
            numDigits = 0;
        }
    }
}

