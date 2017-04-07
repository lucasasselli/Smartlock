using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;

using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;

namespace SmartLock
{
    public partial class Program
    {
        private Window PinWindow = new Window();
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
        private Window AccessAllowedWindow = new Window();
        private Bitmap immAccessAllowed;
        private Image immOK;
        private TextBlock tbOK;
        private Window AccessDeniedWindow = new Window();
        private Bitmap immAccessDenied;
        private Image immAlt;
        private TextBlock tbAlt;

        private void Display_Initialize()
        {
            //load resources:
            PinWindow = GlideLoader.LoadWindow(Resources.GetString(
                Resources.StringResources.PinWindow));
            AccessAllowedWindow = GlideLoader.LoadWindow(Resources.GetString(
                Resources.StringResources.AccessAllowedWindow));
            AccessDeniedWindow = GlideLoader.LoadWindow(Resources.GetString(
                Resources.StringResources.AccessDeniedWindow));
            //immAccessAllowed = new Bitmap(Resources.GetBytes(
                //Resources.BinaryResources.ImmAccessAllowed), Bitmap.BitmapImageType.Jpeg);
            //immAccessDenied = new Bitmap(Resources.GetBytes(
                //Resources.BinaryResources.ImmAccessDenied), Bitmap.BitmapImageType.Jpeg);
            //initialization:
            GlideTouch.Initialize();
            bt0 = (Button)PinWindow.GetChildByName("b0");
            bt1 = (Button)PinWindow.GetChildByName("b1");
            bt2 = (Button)PinWindow.GetChildByName("b2");
            bt3 = (Button)PinWindow.GetChildByName("b3");
            bt4 = (Button)PinWindow.GetChildByName("b4");
            bt5 = (Button)PinWindow.GetChildByName("b5");
            bt6 = (Button)PinWindow.GetChildByName("b6");
            bt7 = (Button)PinWindow.GetChildByName("b7");
            bt8 = (Button)PinWindow.GetChildByName("b8");
            bt9 = (Button)PinWindow.GetChildByName("b9");
            btdel = (Button)PinWindow.GetChildByName("bdel");
            btac = (Button)PinWindow.GetChildByName("bac");
            pb1 = (PasswordBox)PinWindow.GetChildByName("p1");
            immOK = (Image)AccessAllowedWindow.GetChildByName("immOK");
            tbOK = (TextBlock)AccessAllowedWindow.GetChildByName("tbOK");
            immAlt = (Image)AccessDeniedWindow.GetChildByName("immAlt");
            tbAlt = (TextBlock)AccessDeniedWindow.GetChildByName("tbAlt");

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

            Glide.MainWindow = PinWindow;
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
            password = String.Empty; //clear password
            pb1.Text = password;
            pb1.Invalidate();
            numDigits = 0;
        }

        private void checkPsw()
        {
            numDigits++;
            if (numDigits == pswLength) //reached password length
            {
                if (!flagEmptyUserList) //if there are users
                {
                    foreach (UserForLock user in UserList)
                    {
                        if (password.Equals(user.Pin))
                        {
                            flagAuthorizedAccess = true;
                            break;
                        }
                    }
                }
                printAccessWindow(flagAuthorizedAccess);
                Log accessLog; //create a new log
                if (flagAuthorizedAccess)
                {
                    unlockDoor();
                    accessLog = new Log(2, "Pin " + password + " inserted. Authorized access.",
                        DateTime.Now.ToString());
                }
                else
                {
                    accessLog = new Log(2, "Pin " + password + " inserted. Access denied.",
                        DateTime.Now.ToString());
                }
                flagAuthorizedAccess = false; //reset flag
                Logs.Add(accessLog); //add log to log list
                if (flagConnectionOn)
                    ServerPOST(); //send log
                else
                    flagPendingLog = true; //pending logs
                numDigits = 0;
                password = String.Empty; //clear password
                pb1.Text = password;
                pb1.Invalidate();
            }
        }

        private void printAccessWindow(bool flagAuthorizedAccess)
        {
            if (flagAuthorizedAccess)
            {
                Glide.MainWindow = AccessAllowedWindow;
                //immOK.Bitmap = immAccessAllowed;
                //immOK.Render(); //adapt to imagebox
                tbOK.Text = "Access Allowed!";
                //immOK.Invalidate(); //send image to display
                tbOK.Invalidate(); //send text to display
            }
            else
            {
                Glide.MainWindow = AccessDeniedWindow;
                //immAlt.Bitmap = immAccessDenied;
                //immAlt.Render();
                tbAlt.Text = "Access Denied!";
                //immAlt.Invalidate();
                tbAlt.Invalidate();
            }
            timerSecondWindow.Start(); //set second window for a time interval
        }
    }
}

