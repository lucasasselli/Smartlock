using System;
using GHI.Glide;
using GHI.Glide.UI;

namespace SmartLock.GUI
{
    public class MaintenanceWindow : ManageableWindow
    {
        // Time constants
        private const int WindowAccessPeriod = 2000;
        private const int WindowAlertPeriod = 10000;

        private const int WindowAlertId = 0;

        public MaintenanceWindow(int id): base(id)
        {
            Window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.MaintenanceWindow));

            // Load window elements
            var bback = (Button)Window.GetChildByName("bback");
            var bip = (Button)Window.GetChildByName("bip");
            var bport = (Button)Window.GetChildByName("bport");
            var blockid = (Button)Window.GetChildByName("blockid");
            var bmaster = (Button)Window.GetChildByName("bmaster");
            var bclear = (Button)Window.GetChildByName("bclear");

            bback.TapEvent += bback_TapEvent;
            bip.TapEvent += bip_TapEvent;
            bport.TapEvent += bport_TapEvent;
            blockid.TapEvent += blockid_TapEvent;
            bmaster.TapEvent += bmaster_TapEvent;
            bclear.TapEvent += bclear_TapEvent;
        }

        void bback_TapEvent(object sender)
        {
            // Back to main window
            WindowManger.ShowMainWindow();
        }

        void bip_TapEvent(object sender)
        {
            // Change server ip
        }

        void bport_TapEvent(object sender)
        {
            // Change server port
        }

        void blockid_TapEvent(object sender)
        {
            // Change lock id
        }

        void bmaster_TapEvent(object sender)
        {
            // Change master ip
        }

        void bclear_TapEvent(object sender)
        {
            // Clear cache
            if (clearCache())
            {
                // Cache cleared successfully
                var cacheSuccessAlert = new AlertWindow(WindowAlertId, WindowAlertPeriod);
                cacheSuccessAlert.SetText("Cache cleared!");
                cacheSuccessAlert.SetPositiveButton("Ok", delegate { cacheSuccessAlert.Dismiss(); });
                cacheSuccessAlert.Show();
            }
            else
            {
                // Cache clear failed
            }
        }

        private bool clearCache()
        {
            if (!CacheAccess.Delete(CacheAccess.UsersCacheFile)) return false;
            if (!CacheAccess.Delete(CacheAccess.LogsCacheFile)) return false;

            return true;
        }
    }
}
