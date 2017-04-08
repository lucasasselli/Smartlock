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
    public class Display
    {
        // Constants
        private const int pinLength = 5; // pin max length
        private const int timerSecondWindowCount = 1500; // 1.5 sec

        // Event handling
        public event PinEventHandler PinFound;
        public delegate void PinEventHandler(string pin);

        // Timers        
        private GT.Timer timerSecondWindow; // Timer for access or denied window

        // Global variables
        private int numDigits; // Current number of digits inside the pin
        private string pin; // String of the pin

        private Window PinWindow = new Window();
        private Window AccessWindow = new Window();

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

        // Access window elements
        private Bitmap immAccessAllowed;
        private Bitmap immAccessDenied;
        private Image accessImm;
        private TextBlock accessTb;

        public Display()
        {
            // Data Setup
            numDigits = 0;

            timerSecondWindow = new GT.Timer(timerSecondWindowCount); // 1.5 sec
            timerSecondWindow.Tick += timerAccessWindow_Tick;
            
            // Load graphical resources
            PinWindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.PinWindow));
            AccessWindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.AccessWindow));
            immAccessAllowed = new Bitmap(Resources.GetBytes(Resources.BinaryResources.ImmAccessAllowed), Bitmap.BitmapImageType.Bmp);
            immAccessDenied = new Bitmap(Resources.GetBytes(Resources.BinaryResources.ImmAccessDenied), Bitmap.BitmapImageType.Bmp);

            // Initialization
            GlideTouch.Initialize();

            // Load pin window elements
            bt0 = (Button) PinWindow.GetChildByName("b0");
            bt1 = (Button) PinWindow.GetChildByName("b1");
            bt2 = (Button) PinWindow.GetChildByName("b2");
            bt3 = (Button) PinWindow.GetChildByName("b3");
            bt4 = (Button) PinWindow.GetChildByName("b4");
            bt5 = (Button) PinWindow.GetChildByName("b5");
            bt6 = (Button) PinWindow.GetChildByName("b6");
            bt7 = (Button) PinWindow.GetChildByName("b7");
            bt8 = (Button) PinWindow.GetChildByName("b8");
            bt9 = (Button) PinWindow.GetChildByName("b9");
            btdel = (Button) PinWindow.GetChildByName("bdel");
            btac = (Button) PinWindow.GetChildByName("bac");
            pb1 = (PasswordBox) PinWindow.GetChildByName("p1");

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

            // Load access window elements
            accessImm = (Image) AccessWindow.GetChildByName("access_imm");
            accessTb = (TextBlock) AccessWindow.GetChildByName("access_tb");

            Glide.MainWindow = PinWindow;
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

        // Show Access window
        public void ShowAccessWindow(bool flagAuthorizedAccess)
        {
            if (flagAuthorizedAccess)
            {

                accessImm.Bitmap = immAccessAllowed;
                accessTb.Text = "Access Allowed!";
            }
            else
            {
                accessImm.Bitmap = immAccessDenied;
                accessTb.Text = "Access Denied!";
            }

            accessImm.Render(); // Adapt to imagebox
            accessImm.Invalidate(); // Send image to display
            accessTb.Invalidate(); // Send text to display
            Glide.MainWindow = AccessWindow;

            // Set second window for a time interval
            timerSecondWindow.Start(); 
        }

        // Remove second window
        private void timerAccessWindow_Tick(GT.Timer timerAccessWindow)
        {
            Glide.MainWindow = PinWindow; //back to main window
            timerAccessWindow.Stop();
        }
    }
}

