using GHI.Glide;
using GHI.Glide.UI;
using Microsoft.SPOT;

namespace SmartLock.GUI
{
    public abstract class InputWindow : ManageableWindow
    {
        // Window elements
        private readonly Image icon;
        private readonly PasswordBox passwordBox;
        private readonly TextBlock text;

        // Global variables
        public string Pin { set; get; } // String of the Pin

        public InputWindow(int id, bool hasAuxButtons) : base(id)
        {
            // Load graphical resources
            Window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.InputWindow));

            // Load window elements
            var bt0 = (Button)Window.GetChildByName("b0");
            var bt1 = (Button)Window.GetChildByName("b1");
            var bt2 = (Button)Window.GetChildByName("b2");
            var bt3 = (Button)Window.GetChildByName("b3");
            var bt4 = (Button)Window.GetChildByName("b4");
            var bt5 = (Button)Window.GetChildByName("b5");
            var bt6 = (Button)Window.GetChildByName("b6");
            var bt7 = (Button)Window.GetChildByName("b7");
            var bt8 = (Button)Window.GetChildByName("b8");
            var bt9 = (Button)Window.GetChildByName("b9");
            var btdel = (Button)Window.GetChildByName("bdel");
            var btac = (Button)Window.GetChildByName("bac");
            var ba1 = (Button)Window.GetChildByName("ba1");
            var ba2 = (Button)Window.GetChildByName("ba2");
            var benter = (Button)Window.GetChildByName("benter");
            passwordBox = (PasswordBox)Window.GetChildByName("p1");
            icon = (Image)Window.GetChildByName("icon");
            text = (TextBlock)Window.GetChildByName("text");

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
            ba1.TapEvent += ba1_TapEvent;
            ba2.TapEvent += ba2_TapEvent;
            benter.TapEvent += benter_TapEvent;

            ba1.Visible = hasAuxButtons;
            ba2.Visible = hasAuxButtons;

            text.Visible = false;
            icon.Visible = false;
        }

        public void SetIcon(byte[] bytes)
        {
            icon.Visible = true;
            icon.Bitmap = new Bitmap(bytes, Bitmap.BitmapImageType.Bmp);
            icon.Render();
            icon.Invalidate();
        }

        public void SetText(string textString)
        {
            text.Visible = true;
            text.Text = textString;
            text.Invalidate();
        }

        private void bt0_TapEvent(object sender)
        {
            UpdatePinField('0');
        }

        private void bt1_TapEvent(object sender)
        {
            UpdatePinField('1');
        }

        private void bt2_TapEvent(object sender)
        {
            UpdatePinField('2');
        }

        private void bt3_TapEvent(object sender)
        {
            UpdatePinField('3');
        }

        private void bt4_TapEvent(object sender)
        {
            UpdatePinField('4');
        }

        private void bt5_TapEvent(object sender)
        {
            UpdatePinField('5');
        }

        private void bt6_TapEvent(object sender)
        {
            UpdatePinField('6');
        }

        private void bt7_TapEvent(object sender)
        {
            UpdatePinField('7');
        }

        private void bt8_TapEvent(object sender)
        {
            UpdatePinField('8');
        }

        private void bt9_TapEvent(object sender)
        {
            UpdatePinField('9');
        }

        private void btdel_TapEvent(object sender)
        {
            if (Pin.Length != 0)
            {
                Pin = Pin.Substring(0, Pin.Length - 1);
                passwordBox.Text = Pin;
                passwordBox.Invalidate();
            }
        }

        private void btac_TapEvent(object sender)
        {
            Pin = string.Empty; //clear Pin
            passwordBox.Text = Pin;
            passwordBox.Invalidate();
        }

        public virtual void ba1_TapEvent(object sender)
        {
            // Empty
        }

        public virtual void ba2_TapEvent(object sender)
        {
            // Empty
        }

        public virtual void benter_TapEvent(object sender)
        {
            // Empty
        }

        public void UpdatePinField(char c){
            Pin = Pin + c;
            passwordBox.Text = Pin;
            passwordBox.Invalidate();
        }

        public void ClearPinField()
        {
            Pin = string.Empty;
            passwordBox.Text = Pin;
            passwordBox.Invalidate();
        }


    }
}
