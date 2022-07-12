using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Threading;
using Forms = System.Windows.Forms;
using Windows.Devices.Power;
using Windows.UI.Core;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

namespace BatteryIcon
{
    // add rounded corners
    public partial class App : Application
    {
        private static bool open = false;
        private static bool notified = false;
        bool reportRequested = false;

        protected static Forms.NotifyIcon _notifyIcon = new Forms.NotifyIcon();
        private static Forms.PowerStatus _pwr = Forms.SystemInformation.PowerStatus;

        private MainWindow form = new MainWindow();
        public MainWindow access = new MainWindow();
        private Settings s = new Settings();

        public App()
        {
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;

        }
        protected override void OnStartup(StartupEventArgs e)
        {

            //_notifyIcon.Icon = new Icon("Resources\\no_battery.ico");
            SetIcon(_pwr);
            SetText(_pwr);

            _notifyIcon.MouseClick += notifyIcon_MouseClick;

            _notifyIcon.ContextMenu = new Forms.ContextMenu();
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("Power", OnPowerClicked));
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("Settings", OnSettingsClicked));
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("Battery Status", OnSettingsClicked));
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("Exit", OnExitClicked));
            _notifyIcon.Visible = true;

            base.OnStartup(e);

        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Icon = null;
            _notifyIcon.Dispose();
            Environment.Exit(0);
        }

        private void OnSettingsClicked(object sender, EventArgs e)
        {
            //display Github page, release notes, license and check for updates
            //allow user to change between number and percentage icon, autostart with windows, and dark/light theme
            //along with Battery Icon (name), Taskbar icon, version number, and author 2022
            //user should be able to check for updates and uninstall application
            s.Left = SystemParameters.WorkArea.Width - 350 - 2;
            s.Top = SystemParameters.WorkArea.Height - 250 - 2;
            //form.SetDesktopLocation(MousePosition.X - form.Width / 2, MousePosition.Y - form.Height - 20);
            s.Show();
            s.Activate();
        }

        private void OnStatusClicked(object sender, EventArgs e)
        {
            //battery type, wear level, power state, designed and fully charged capacity
            //current capacity, battery voltage, charge/discharge rate, current power source, 
            //battery charge, and remaining battery lifetime
            SetIcon(_pwr);
            SetText(_pwr);
        }

        private void OnPowerClicked(object sender, EventArgs e)
        {
            var cplPath = System.IO.Path.Combine(Environment.SystemDirectory, "control.exe");
            System.Diagnostics.Process.Start(cplPath, "/name Microsoft.PowerOptions");
        }

        private void notifyIcon_MouseClick(object sender, Forms.MouseEventArgs e)
        {

            Forms.MouseEventArgs mouseEventArgs = (Forms.MouseEventArgs)e;
            if (mouseEventArgs.Button == Forms.MouseButtons.Left)
            {
                if (open == false)
                {

                    open = true;
                    form.Left = SystemParameters.WorkArea.Width - 350 - 2;
                    form.Top = SystemParameters.WorkArea.Height - 250 - 2;
                    //form.SetDesktopLocation(MousePosition.X - form.Width / 2, MousePosition.Y - form.Height - 20);
                    form.Show();
                    form.Activate();
                    //form.TopMost = true;
                }
                else
                {
                    open = false;
                    form.Hide();
                }

            }


        }


        public void SetIcon(Forms.PowerStatus _pwr)
        {

            Double BatteryLifeRemaining = Math.Round(_pwr.BatteryLifeRemaining / 60.0);
            Double Batterylife = Math.Round(_pwr.BatteryLifePercent * 100);

            if (_pwr.PowerLineStatus == Forms.PowerLineStatus.Unknown)
            {
                _notifyIcon.Icon = new Icon("Resources\\no_battery.ico");
                form.SetIcon("Resources\\no_battery.ico");
            }
            else if (_pwr.PowerLineStatus == Forms.PowerLineStatus.Online)
            {
                _notifyIcon.Icon = new Icon("Resources\\charging.ico");
                form.SetIcon("Resources\\charging.ico");
                
            }
            else if (Batterylife >= 80 && Batterylife <= 100)
            {
                _notifyIcon.Icon = new Icon("Resources\\high.ico");
                form.SetIcon("Resources\\high.ico");
            }
            else if (Batterylife >= 50 && Batterylife <= 79)
            {
                _notifyIcon.Icon = new Icon("Resources\\mid.ico");
                form.SetIcon("Resources\\mid.ico");
            }
            else if (Batterylife >= 25 && Batterylife <= 49)
            {
                _notifyIcon.Icon = new Icon("Resources\\middle.ico");
                form.SetIcon("Resources\\middle.ico");
                if (notified == true)
                {
                    notified = false;
                }
            }
            else if (Batterylife >= 16 && Batterylife <= 24)
            {
                _notifyIcon.Icon = new Icon("Resources\\low.ico");
                form.SetIcon("Resources\\low.ico");

                if (notified == false && (Batterylife >= 16 && Batterylife <= 20))
                {
                     new ToastContentBuilder()
                    .AddArgument("action", "viewConversation")
                    .AddArgument("conversationId", 9813)
                    .AddText("Low Battery")
                    .AddText("Battery charge is less than 20%")
                    .Show();
                    notified = true;
                }
            }
            else if (Batterylife >= 15)
            {
                _notifyIcon.Icon = new Icon("Resources\\verylow.ico");
                form.SetIcon("Resources\\verylow.ico");
            }
        }

        public void SetText(Forms.PowerStatus _pwr)
        {
            Double BatteryLifeRemaining = Math.Round(_pwr.BatteryLifeRemaining / 60.0);
            Double Batterylife = Math.Round(_pwr.BatteryLifePercent * 100);
            form.SetPercent(Batterylife.ToString() + "%");

            if (_pwr.PowerLineStatus == Forms.PowerLineStatus.Online)
            {
                _notifyIcon.Text = Batterylife.ToString() + "% available (plugged in)";
                form.SetTimeRemaining(Batterylife.ToString() + "% available (plugged in)");
            }
            else
            {
                int hrs = (int)(BatteryLifeRemaining / 60);
                int mins = (int)(BatteryLifeRemaining % 60);
                if (hrs == 0)
                {
                    _notifyIcon.Text = Batterylife.ToString() + "%" + " remaining";
                    form.SetTimeRemaining(Batterylife.ToString() + "%" + " remaining");
                }
                else
                {
                    _notifyIcon.Text = $"{hrs} hr {mins} min " + " " + "(" + Batterylife.ToString() + "%" + ") remaining";
                    form.SetTimeRemaining($"{hrs} hr {mins} min " + " " + "(" + Batterylife.ToString() + "%" + ") remaining");
                }
            }
        }

        private void RequestAggregateBatteryReport()
        {
            // Create aggregate battery object
            var aggBattery = Battery.AggregateBattery;

            // Get report
            var report = aggBattery.GetReport();

            // Update UI
            //AddReportUI(BatteryReportPanel, report, aggBattery.DeviceId);
        }

        private void AggregateBattery_ReportUpdated(Battery sender, object args)
        {
            //make async

            SetIcon(_pwr);
            SetText(_pwr);

        }

    }
}