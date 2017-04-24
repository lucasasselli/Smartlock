using GHI.Glide;
using GHI.Glide.UI;
using Microsoft.SPOT;

namespace SmartLock.GUI
{
    public class PinWindow : InputWindow
    {
        public delegate void PinEventHandler(string pin);

        public PinWindow() : base(false, false)
        {
            SetDataSource(DataHelper.DataSourceUnknown);
        }

        // Event handling
        public event PinEventHandler PinFound;

        public void SetDataSource(int dataSource)
        {
            switch (dataSource)
            {
                case DataHelper.DataSourceCache:
                    SetIcon(Resources.GetBytes(Resources.BinaryResources.data_source_cache));
                    break;

                case DataHelper.DataSourceRemote:
                    SetIcon(Resources.GetBytes(Resources.BinaryResources.data_source_remote));
                    break;

                default:
                    SetIcon(Resources.GetBytes(Resources.BinaryResources.data_source_error));
                    break;
            }
        }

        protected override void benter_TapEvent(object sender)
        {
            if (PinFound != null) PinFound(Input);
            Clear();
        }
    }
}