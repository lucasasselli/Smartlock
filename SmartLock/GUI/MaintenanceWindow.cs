using System;
using GHI.Glide;
using GHI.Glide.UI;

namespace SmartLock.GUI
{
    public class MaintenanceWindow : ManageableWindow
    {
        // Time constants
        private const int WindowAlertPeriod = 10000;
        private const int WindowAlertId = 0;

        // TODO Create only one instance of setting Window
        
        public MaintenanceWindow(): base()
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
            WindowManger.Back();
        }

        void bip_TapEvent(object sender)
        {
            // Change server ip
            var settingWindow = new SettingWindow(SettingsManager.ServerIp, true);
            settingWindow.SetText("Insert server ip");
            settingWindow.Show();
        }

        void bport_TapEvent(object sender)
        {
            // Change server port
            var settingWindow = new SettingWindow(SettingsManager.ServerPort, false);
            settingWindow.SetText("Insert server port");
            settingWindow.Show();
        }

        void blockid_TapEvent(object sender)
        {
            // Change lock id
            var settingWindow = new SettingWindow(SettingsManager.LockId, false);
            settingWindow.SetText("Insert lock id");
            settingWindow.Show();
        }

        void bmaster_TapEvent(object sender)
        {
            // Change master pin
            var settingWindow = new SettingWindow(SettingsManager.MasterPin, false);
            settingWindow.SetText("Insert master pin");
            settingWindow.Show();
        }

        void bclear_TapEvent(object sender)
        {
            // Clear cache
            if (clearCache())
            {
                // Cache cleared successfully
                var cacheSuccessAlert = new AlertWindow(WindowAlertPeriod);
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
            if (!CacheManager.Delete(CacheManager.UsersCacheFile)) return false;
            if (!CacheManager.Delete(CacheManager.LogsCacheFile)) return false;

            return true;
        }
    }
}
