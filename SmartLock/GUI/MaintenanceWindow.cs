using System;
using GHI.Glide;
using GHI.Glide.UI;

namespace SmartLock.GUI
{
    public class MaintenanceWindow : WindowManager.ManageableWindow
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
            var blockip = (Button)Window.GetChildByName("blockip");
            var bserverip = (Button)Window.GetChildByName("bserverip");
            var bport = (Button)Window.GetChildByName("bport");
            var blockid = (Button)Window.GetChildByName("blockid");
            var bmaster = (Button)Window.GetChildByName("bmaster");
            var bperiod = (Button)Window.GetChildByName("bperiod");
            var bretry = (Button)Window.GetChildByName("bretry");
            var bclear = (Button)Window.GetChildByName("bclear");
            var breboot = (Button)Window.GetChildByName("breboot");

            bback.TapEvent += bback_TapEvent;
            bserverip.TapEvent += bserverip_TapEvent;
            blockip.TapEvent += blockip_TapEvent;
            bport.TapEvent += bport_TapEvent;
            blockid.TapEvent += blockid_TapEvent;
            bmaster.TapEvent += bmaster_TapEvent;
            bperiod.TapEvent += bperiod_TapEvent;
            bretry.TapEvent += bretry_TapEvent;
            bperiod.TapEvent += bperiod_TapEvent;
            bclear.TapEvent += bclear_TapEvent;
            breboot.TapEvent += breboot_TapEvent;
        }

        void bback_TapEvent(object sender)
        {
            // Back to main window
            WindowManager.Back();
        }

        void bserverip_TapEvent(object sender)
        {
            // Change server ip
            var settingWindow = new SettingWindow(SettingsManager.ServerIp, true);
            settingWindow.SetText("Insert server ip");
            settingWindow.Show();
        }

        void blockip_TapEvent(object sender)
        {
            // Change Lock ip
            var settingWindow = new SettingWindow(SettingsManager.LockIp, true);
            settingWindow.SetText("Insert lock ip");
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
            // Change period pin
            var settingWindow = new SettingWindow(SettingsManager.MasterPin, false);
            settingWindow.SetText("Insert master pin");
            settingWindow.Show();
        }

        void bperiod_TapEvent(object sender)
        {
            // Change routine period
            var settingWindow = new SettingWindow(SettingsManager.RoutinePeriod, false);
            settingWindow.SetText("Insert routine period (milliseconds)");
            settingWindow.Show();
        }

        void bretry_TapEvent(object sender)
        {
            // Change routine retry
            var settingWindow = new SettingWindow(SettingsManager.RetryPeriod, false);
            settingWindow.SetText("Insert retry period (milliseconds)");
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

        void breboot_TapEvent(object sender)
        {
            var rebootAlert = new AlertWindow(WindowAlertPeriod);
            rebootAlert.SetText("Do you really want to reboot?");
            rebootAlert.SetPositiveButton("Yes", delegate
            {
                Microsoft.SPOT.Hardware.PowerState.RebootDevice(true);
            });
            rebootAlert.SetNegativeButton("No", delegate { rebootAlert.Dismiss(); });
            rebootAlert.Show();
        }

        private bool clearCache()
        {
            if (!CacheManager.Delete(CacheManager.UsersCacheFile)) return false;
            if (!CacheManager.Delete(CacheManager.LogsCacheFile)) return false;

            return true;
        }
    }
}
