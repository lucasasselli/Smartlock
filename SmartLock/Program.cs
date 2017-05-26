using System;
using Microsoft.SPOT;
using GHI.Glide;
using SmartLock.GUI;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.Luca_Sasselli;
using GT = Gadgeteer;

namespace SmartLock
{
    public partial class Program
    {
        // Time constants
        private const int WindowAccessPeriod = 2000;
        private const int WindowAlertPeriod = 10000;
        private const int NfcScanPeriod = 1000;
        private const int NfcScanTimeout = 200;

        // Window ID constants
        private const int WindowPinId = 0;
        private const int WindowScanId = 1;

        // Nfc setup
        private string pendingPin;

        // Windows
        PinWindow pinWindow = new PinWindow();
        AccessWindow accessWindow = new AccessWindow(WindowAccessPeriod);
        AlertWindow scanWindow = new AlertWindow(WindowAlertPeriod);
        MaintenanceWindow maintenanceWindow = new MaintenanceWindow();

        // Door
        private const int unlockPeriod = 60000;
        private bool authorizedAccess = false;
        private bool doorOpen = false;
        private bool unlockTimerRunning = false;
        private bool pendingUnlockError = false;
        private GT.Timer unlockedTimer;

        public void ProgramStarted()
        {
            DebugOnly.Print("Program started!");

            // Initialize screen
            WindowManager.Init();

            // Set scan window
            scanWindow.SetText("Please scan your NFC card now...");
            scanWindow.SetNegativeButton("Cancel", delegate { scanWindow.Dismiss(); });

            // Windows and window manager
            pinWindow.Id = WindowPinId;
            scanWindow.Id = WindowScanId;

            // Button
            button.ButtonPressed += DoorOpen;
            button.ButtonReleased += DoorClosed;

            doorOpen = button.Pressed;
            if (doorOpen)
            {
                pinWindow.SetText("Door open");
            }
            else
            {
                pinWindow.SetText("Door closed");
            }

            unlockedTimer = new GT.Timer(unlockPeriod);
            unlockedTimer.Tick += UnlockTimeout;

            pinWindow.Show();

            // Event Setup
            adafruit_PN532.TagFound += TagFound;
            adafruit_PN532.Error += NfcError;
            pinWindow.PinFound += PinFound;
            DataHelper.DataSourceChanged += DataSourceChanged;
            WindowManager.WindowChanged += WindowChanged;

            // Start NFC
            adafruit_PN532.StartScan(NfcScanPeriod, NfcScanTimeout);

            //Init
            CacheManager.Init(sdCard);
            SettingsManager.Init();
            DataHelper.Init(ethernetJ11D);
          
            adafruit_PN532.Init();
        }

        /*
         * TAG FOUND EVENT:
         * This event occurs when the user passes a NFC card near the reader.
         * It checks if the UID is valid and unlock the door if so.
         */
        private void TagFound(string uid)
        {
            if (!scanWindow.IsShowing())
            {
                // Check authorization
                var authorized = DataHelper.CheckCardId(uid);

                // Show access window
                accessWindow.Show(authorized);

                // Log the event
                string logText;
                Log log;

                if (authorized)
                {
                    // Access granted
                    UnlockDoor();
                    logText = "Card \"" + uid + "\" inserted. Authorized access.";
                    log = new Log(Log.TypeAccess, null, uid, logText);
                }
                else
                {
                    // Access denied
                    logText = "Card \"" + uid + "\" inserted. Access denied!";
                    log = new Log(Log.TypeInfo, logText);
                }

                DebugOnly.Print(logText);

                // Add log to loglist
                DataHelper.AddLog(log);
            }
            else
            {
                // Log new cardID
                var newCardIdLog = new Log(
                    Log.TypeAccess,
                    pendingPin,
                    uid,
                    "Card \"" + uid + "\" has been added for pin \"" + pendingPin + "\".");

                // Update CardID in userList
                DataHelper.AddCardId(pendingPin, uid);

                // Send urgent log
                DataHelper.AddLog(newCardIdLog, true);

                var cardAddedAlert = new AlertWindow(WindowAlertPeriod);
                cardAddedAlert.SetText("NFC card added!");
                cardAddedAlert.SetPositiveButton("Ok", delegate { cardAddedAlert.Dismiss(); });
                cardAddedAlert.Show();

                DebugOnly.Print("Card added to user with pin \"" + pendingPin + "\"");
            }
        }

        /*
         * PIN FOUND EVENT:
         * This event occurs when the user inserts a pin code. It checks if the pin is valid and unlock the door if so.
         * If the pin has no related CardId it prompts the user to add it.
         */
        private void PinFound(string pin)
        {
            string masterPin = SettingsManager.Get(SettingsManager.MasterPin);

            // Check master pin
            if (String.Compare(masterPin, pin) == 0)
            {
                maintenanceWindow.Show();
                return;
            }
            
            // Check authorization
            var authorized = DataHelper.CheckPin(pin);
            var nullCardId = DataHelper.PinHasNullCardId(pin);

            // Log the event
            string logText;
            Log log;

            if (authorized)
            {
                // Access granted
                UnlockDoor();
                logText = "Pin \"" + pin + "\" inserted. Authorized access.";
                log = new Log(Log.TypeAccess, pin, logText);
            }
            else
            {
                // Access denied
                logText = "Pin \"" + pin + "\" inserted. Access denied!";
                log = new Log(Log.TypeInfo, logText);
            }

            DebugOnly.Print(logText);

            // Add log to loglist
            DataHelper.AddLog(log);

            if (nullCardId && authorized)
            {
                // Null CardID detected, prompt the user to set one
                var nullCardIdAlert = new AlertWindow(WindowAlertPeriod);
                nullCardIdAlert.SetText(
                    "It happears that this user has no related NFC card.\nDo you want to scan it now?");
                nullCardIdAlert.SetPositiveButton("Yes", delegate
                {
                    // User wants to add a new NFC card
                    pendingPin = pin;
                    scanWindow.Show();
                });
                nullCardIdAlert.SetNegativeButton("No", delegate
                {
                    // User doesn't want to add a new NFC card
                    nullCardIdAlert.Dismiss();
                });
                nullCardIdAlert.Show();
            }
            else
            {
                // Everything is fine
                accessWindow.Show(authorized);
            }
        }

        /*
         * UNLOCK DOOR
         * Called by either PinFound or TagFound to unlock the door.
         */
        private void UnlockDoor()
        {
            // Authorize access
            authorizedAccess = true;

            unlockTimerRunning = true;
            unlockedTimer.Restart();

            // Blink LED
            Mainboard.SetDebugLED(true);
            System.Threading.Thread.Sleep(200);
            Mainboard.SetDebugLED(false);
        }

        /*
         * DATA SOURCE CHANGED EVENT:
         * This event occurs when the data source for the user list changes.
         */
        private void DataSourceChanged(int dataSource)
        {
            pinWindow.SetDataSource(dataSource);
        }

        /*
         * PIN WINDOW VISIBLE EVENT:
         * This event occours when the visibility of pinWindow changes.
         */
        private void WindowChanged(int id)
        {
            if (id == WindowPinId || id == WindowScanId)
                adafruit_PN532.StartScan(NfcScanPeriod, NfcScanTimeout);
            else
                adafruit_PN532.StopScan();
        }

        /*
         * NFC ERROR EVENT:
         * This event occurs when the nfc module is not responding to commands.
         */
        private void NfcError()
        {
            DebugOnly.Print("NFC Module is not responding!");
            Log log = new Log(Log.TypeError, "NFC Module is not responding!");
            DataHelper.AddLog(log);
        }

        private void DoorOpen(Button sender, Button.ButtonState state)
        {
            doorOpen = true;

            pinWindow.SetText("Door open");
            DebugOnly.Print("Door open!");

            if (!authorizedAccess)
            {
                // Security breach
                DebugOnly.Print("Security breach! Unauthorized access!");
                DataHelper.AddLog(new Log(Log.TypeError, "Security breach! Unauthorized access!"));

                var breachWindow = new AlertWindow(20000);
                breachWindow.SetText("BREACH ALERT!!! \n UNAUTHORIZED ACCESS!!! \n\n\n\n Please contact the administrator immediately.");
                breachWindow.Show();
            }
        }

        private void DoorClosed(Button sender, Button.ButtonState state)
        {
            authorizedAccess = false;
            doorOpen = false;

            pinWindow.SetText("Door closed");
            DebugOnly.Print("Door closed!");

            // Stop timer
            unlockTimerRunning = false;
            unlockedTimer.Stop();

            // Log if there is a pending error
            if (pendingUnlockError)
            {
                pendingUnlockError = false;
                DebugOnly.Print("Door finally closed");
                DataHelper.AddLog(new Log(Log.TypeError, "Door finally closed"));
            }
        }

        private void UnlockTimeout(GT.Timer timerAccessWindow)
        {
            if (unlockTimerRunning)
            {

                authorizedAccess = false;

                // Stop timer
                unlockTimerRunning = false;
                unlockedTimer.Stop();

                // New pending unlock error
                pendingUnlockError = true;

                // Send log
                DebugOnly.Print("Unlock timeout! Door is not closed!!!");
                DataHelper.AddLog(new Log(Log.TypeError, "Unlock timeout! Door is not closed!!!"));
            }
        }
    }
}