using System;
using System.Collections;
using System.Threading;

using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;

namespace SmartLock
{
    public partial class Program
    {
        private Window window = new Window();
        private TextBlock tb1;
        private PasswordBox pb1;
        private Image imm1;
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

        private void Display_Initialize()
        {
            window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.window));
            GlideTouch.Initialize();
            bt0 = (Button)window.GetChildByName("b0");
            bt1 = (Button)window.GetChildByName("b1");
            bt2 = (Button)window.GetChildByName("b2");
            bt3 = (Button)window.GetChildByName("b3");
            bt4 = (Button)window.GetChildByName("b4");
            bt5 = (Button)window.GetChildByName("b5");
            bt6 = (Button)window.GetChildByName("b6");
            bt7 = (Button)window.GetChildByName("b7");
            bt8 = (Button)window.GetChildByName("b8");
            bt9 = (Button)window.GetChildByName("b9");
            btdel = (Button)window.GetChildByName("bdel");
            btac = (Button)window.GetChildByName("bac");
            tb1 = (TextBlock)window.GetChildByName("t1");
            imm1 = (Image)window.GetChildByName("imm1");
            pb1 = (PasswordBox)window.GetChildByName("p1");

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

            Glide.MainWindow = window;
        }

        private void bt0_TapEvent(object sender)
        {
            password = password + '0';
            pb1.Text = password;
            pb1.Invalidate(); //update passwordbox
            checkPsw();
        }

        private void bt1_TapEvent(object sender)
        {
            password = password + '1';
            pb1.Text = password;
            pb1.Invalidate();
            checkPsw();
        }

        private void bt2_TapEvent(object sender)
        {
            password = password + '2';
            pb1.Text = password;
            pb1.Invalidate();
            checkPsw();
        }

        private void bt3_TapEvent(object sender)
        {
            password = password + '3';
            pb1.Text = password;
            pb1.Invalidate();
            checkPsw();
        }

        private void bt4_TapEvent(object sender)
        {
            password = password + '4';
            pb1.Text = password;
            pb1.Invalidate();
            checkPsw();
        }

        private void bt5_TapEvent(object sender)
        {
            password = password + '5';
            pb1.Text = password;
            pb1.Invalidate();
            checkPsw();
        }

        private void bt6_TapEvent(object sender)
        {
            password = password + '6';
            pb1.Text = password;
            pb1.Invalidate();
            checkPsw();
        }

        private void bt7_TapEvent(object sender)
        {
            password = password + '7';
            pb1.Text = password;
            pb1.Invalidate();
            checkPsw();
        }

        private void bt8_TapEvent(object sender)
        {
            password = password + '8';
            pb1.Text = password;
            pb1.Invalidate();
            checkPsw();
        }

        private void bt9_TapEvent(object sender)
        {
            password = password + '9';
            pb1.Text = password;
            pb1.Invalidate();
            checkPsw();
        }

        private void btdel_TapEvent(object sender)
        {
            if (password.Length != 0)
            {
                password = password.Substring(0, password.Length - 1);
                pb1.Text = password;
                pb1.Invalidate();
                numDigits--;
            }
        }

        private void btac_TapEvent(object sender)
        {
            password = String.Empty;
            pb1.Text = password;
            pb1.Invalidate();
            numDigits = 0;
        }

        private void checkPsw()
        {
            numDigits++;
            if (numDigits == pswLength)
            {
                tb1.Text = "Checking...";
                tb1.Invalidate();

                //TODO

                numDigits = 0;
                password = String.Empty;
                pb1.Text = password;
                pb1.Invalidate();
                window.FillRect(tb1.Rect); //clear textbox
                tb1.Text = ""; //clear textbox
                tb1.Invalidate();
            }
        }
    }
}

