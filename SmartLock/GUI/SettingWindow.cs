using GHI.Glide;
using GHI.Glide.UI;
using Microsoft.SPOT;

namespace SmartLock.GUI
{
    public class SettingWindow : InputWindow
    {

        private int id;

        public SettingWindow(int id, bool hasDotButton)
            : base(hasDotButton, true)
        {
            this.id = id;
            string currentValue = SettingsManager.Get(id);
            SetInput(currentValue);
        }

        protected override void benter_TapEvent(object sender)
        {
            SettingsManager.Set(id, Input);
            Dismiss();
        }
    }
}