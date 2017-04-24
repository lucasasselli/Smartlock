using GHI.Glide;
using GHI.Glide.UI;
using Microsoft.SPOT;

namespace SmartLock.GUI
{
    public abstract class InputWindow : ManageableWindow
    {
        // Window elements
        protected readonly Button bt0 ;
        protected readonly Button bt1;
        protected readonly Button bt2;
        protected readonly Button bt3;
        protected readonly Button bt4;
        protected readonly Button bt5;
        protected readonly Button bt6;
        protected readonly Button bt7;
        protected readonly Button bt8;
        protected readonly Button bt9;
        protected readonly Button btdel;
        protected readonly Button btac;
        protected readonly Button bdot;
        protected readonly Button bback;
        protected readonly Button benter;
        protected readonly Image icon;
        protected readonly TextBox textBox;
        protected readonly TextBlock text;

        // Global variables
        public string Input { set; get; } // String of the Input

        public InputWindow(bool hasDotButton, bool hasBackButton) : base()
        {
            // Load graphical resources
            Window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.InputWindow));

            // Load window elements
            bt0 = (Button)Window.GetChildByName("b0");
            bt1 = (Button)Window.GetChildByName("b1");
            bt2 = (Button)Window.GetChildByName("b2");
            bt3 = (Button)Window.GetChildByName("b3");
            bt4 = (Button)Window.GetChildByName("b4");
            bt5 = (Button)Window.GetChildByName("b5");
            bt6 = (Button)Window.GetChildByName("b6");
            bt7 = (Button)Window.GetChildByName("b7");
            bt8 = (Button)Window.GetChildByName("b8");
            bt9 = (Button)Window.GetChildByName("b9");
            bdot = (Button)Window.GetChildByName("bdot");
            bback = (Button)Window.GetChildByName("bback");
            btac = (Button)Window.GetChildByName("bac");
            btdel = (Button)Window.GetChildByName("bdel");
            benter = (Button)Window.GetChildByName("benter");
            textBox = (TextBox)Window.GetChildByName("textbox");
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
            bdot.TapEvent += bdot_TapEvent;
            bback.TapEvent += bback_TapEvent;
            btac.TapEvent += btac_TapEvent;
            btdel.TapEvent += btdel_TapEvent;
            benter.TapEvent += benter_TapEvent;

            bdot.Visible = hasDotButton;
            bback.Visible = hasBackButton;

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

        public void AddChar(char c)
        {
            Input = Input + c;
            textBox.Text = Input;
            textBox.Invalidate();
        }

        public void SetInput(string input)
        {
            Input = input;
            textBox.Text = Input;
            textBox.Invalidate();
        }

        public void Clear()
        {
            Input = string.Empty;
            textBox.Text = Input;
            textBox.Invalidate();
        }

        protected virtual void bt0_TapEvent(object sender)
        {
            AddChar('0');
        }

        protected virtual void bt1_TapEvent(object sender)
        {
            AddChar('1');
        }

        protected virtual void bt2_TapEvent(object sender)
        {
            AddChar('2');
        }

        protected virtual void bt3_TapEvent(object sender)
        {
            AddChar('3');
        }

        protected virtual void bt4_TapEvent(object sender)
        {
            AddChar('4');
        }

        protected virtual void bt5_TapEvent(object sender)
        {
            AddChar('5');
        }

        protected virtual void bt6_TapEvent(object sender)
        {
            AddChar('6');
        }

        protected virtual void bt7_TapEvent(object sender)
        {
            AddChar('7');
        }

        protected virtual void bt8_TapEvent(object sender)
        {
            AddChar('8');
        }

        protected virtual void bt9_TapEvent(object sender)
        {
            AddChar('9');
        }

        protected virtual void bback_TapEvent(object sender)
        {
            Dismiss();
        }

        protected virtual void bdot_TapEvent(object sender)
        {
            AddChar('.');
        }

        protected virtual void btdel_TapEvent(object sender)
        {
            if (Input.Length != 0)
            {
                Input = Input.Substring(0, Input.Length - 1);
                textBox.Text = Input;
                textBox.Invalidate();
            }
        }

        protected virtual void btac_TapEvent(object sender)
        {
            Input = string.Empty; //clear Input
            textBox.Text = Input;
            textBox.Invalidate();
        }

        protected virtual void benter_TapEvent(object sender)
        {
            // Empty
        }
    }
}
