using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Drawing;
using System.Windows;
using Forms = System.Windows.Forms;
using Windows.Devices.Power;
using System.Windows.Media.Animation;

namespace BatteryIcon
{
    public partial class App : Application
    {
        private static bool open = false;
        private static bool notified = false;
        //bool reportRequested = false;

        protected static Forms.NotifyIcon _notifyIcon = new Forms.NotifyIcon();
        private static Forms.PowerStatus _pwr = Forms.SystemInformation.PowerStatus;

        private MainWindow battForm = new MainWindow();
        private About abForm = new About();

        public App()
        {
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            SetIcon(_pwr);
            SetText(_pwr);

            _notifyIcon.MouseClick += notifyIcon_MouseClick;

            _notifyIcon.ContextMenu = new Forms.ContextMenu();
            //to-do: make context menu dark theme
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("About", OnSettingsClicked));
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("Power", OnPowerClicked));
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("Settings", OnSettingsClicked));
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("Battery Status", OnStatusClicked));
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
            //allow user to change between number and percentage icon, autostart with windows, and dark/light theme
            //along with Battery Icon (name), Taskbar icon, version number, and author 2022
            //user should be able to check for updates and uninstall application
            abForm.Left = SystemParameters.WorkArea.Width - 350 - 2;
            abForm.Top = SystemParameters.WorkArea.Height - 270 - 2;
            //form.SetDesktopLocation(MousePosition.X - form.Width / 2, MousePosition.Y - form.Height - 20);
            abForm.Show();
            abForm.Activate();
            abWindowOpened();
            sendAObj(abForm);
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

                    battForm.Left = SystemParameters.WorkArea.Width - 350 - 2;
                    battForm.Top = SystemParameters.WorkArea.Height - 270 - 2;
                    battForm.ShowInTaskbar = false;
                    //form.SetDesktopLocation(MousePosition.X - form.Width / 2, MousePosition.Y - form.Height - 20);
                    battForm.Show();
                    battForm.Activate();
                    battWindow_Opened();
                    sendMObj(battForm);

                    //form.TopMost = true;
                }
                else
                {
                    open = false;
                    battForm.Hide();
                    if (abForm.IsActive)
                    {
                        abForm.Hide();
                    }
                }

            }


        }


        public void SetIcon(Forms.PowerStatus _pwr)
        {

            Double BatteryLifeRemaining = Math.Round(_pwr.BatteryLifeRemaining / 60.0);
            int Batterylife = (int)(_pwr.BatteryLifePercent * 100);

            if (_pwr.PowerLineStatus == Forms.PowerLineStatus.Unknown)
            {
                _notifyIcon.Icon = new Icon("Resources\\no_battery.ico");
                battForm.SetIcon("Resources\\no_battery.ico");
            }
            else if (_pwr.PowerLineStatus == Forms.PowerLineStatus.Online)
            {
                _notifyIcon.Icon = new Icon("Resources\\charging.ico");
                battForm.SetIcon("Resources\\charging.ico");
            }
            else
            {
                switch (Batterylife)
                {
                    case int val when (val >= 98):
                        _notifyIcon.Icon = new Icon("Resources\\full.ico");
                        battForm.SetIcon("Resources\\full.ico");
                        break;

                    case int val when (val >= 80 && val <= 97):
                        _notifyIcon.Icon = new Icon("Resources\\high.ico");
                        battForm.SetIcon("Resources\\high.ico");
                        break;

                    case int val when (val >= 80 && val <= 97):
                        _notifyIcon.Icon = new Icon("Resources\\high.ico");
                        battForm.SetIcon("Resources\\high.ico");
                        break;

                    case int val when (val >= 50 && val <= 79):
                        _notifyIcon.Icon = new Icon("Resources\\mid.ico");
                        battForm.SetIcon("Resources\\mid.ico");
                        break;

                    case int val when (val >= 25 && val <= 49):
                        _notifyIcon.Icon = new Icon("Resources\\middle.ico");
                        battForm.SetIcon("Resources\\middle.ico");
                        if (notified == true)
                        {
                            notified = false;
                        }
                        break;

                    case int val when (val >= 16 && val <= 24):
                        _notifyIcon.Icon = new Icon("Resources\\low.ico");
                        battForm.SetIcon("Resources\\low.ico");

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
                        break;

                    case int val when (val >= 6 && val <= 15):
                        _notifyIcon.Icon = new Icon("Resources\\medium_low.ico");
                        battForm.SetIcon("Resources\\medium_low.ico");
                        break;

                    case int val when (val <= 5):
                        _notifyIcon.Icon = new Icon("Resources\\very_low.ico");
                        battForm.SetIcon("Resources\\very_low.ico");
                        break;

                    default:
                        _notifyIcon.Icon = new Icon("Resources\\no_battery.ico");
                        battForm.SetIcon("Resources\\no_battery.ico");
                        break;
                }
            }
           

        }

        public void SetText(Forms.PowerStatus _pwr)
        {
            Double BatteryLifeRemaining = Math.Round(_pwr.BatteryLifeRemaining / 60.0);
            Double Batterylife = (int)(_pwr.BatteryLifePercent * 100);
            battForm.SetPercent(Batterylife.ToString() + "%");

            if (_pwr.PowerLineStatus == Forms.PowerLineStatus.Online)
            {
                _notifyIcon.Text = Batterylife.ToString() + "% available (plugged in)";
                battForm.SetTimeRemaining(Batterylife.ToString() + "% available (plugged in)");
            }
            else
            {
                int hrs = (int)(BatteryLifeRemaining / 60);
                int mins = (int)(BatteryLifeRemaining % 60);
                if (hrs == 0)
                {
                    _notifyIcon.Text = Batterylife.ToString() + "%" + " remaining";
                    battForm.SetTimeRemaining(Batterylife.ToString() + "%" + " remaining");
                }
                else
                {
                    _notifyIcon.Text = $"{hrs} hr {mins} min " + " " + "(" + Batterylife.ToString() + "%" + ") remaining";
                    battForm.SetTimeRemaining($"{hrs} hr {mins} min " + " " + "(" + Batterylife.ToString() + "%" + ") remaining");
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
            sendMObj(battForm);
            /*TextBlock txt3 = new TextBlock { Text = "Charge rate (mW): " + report.ChargeRateInMilliwatts.ToString() };
            TextBlock txt4 = new TextBlock { Text = "Design energy capacity (mWh): " + report.DesignCapacityInMilliwattHours.ToString() };
            TextBlock txt5 = new TextBlock { Text = "Fully-charged energy capacity (mWh): " + report.FullChargeCapacityInMilliwattHours.ToString() };
            TextBlock txt6 = new TextBlock { Text = "Remaining energy capacity (mWh): " + report.RemainingCapacityInMilliwattHours.ToString() };*/

        }

        private void battWindow_Opened()
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                                        (Duration)TimeSpan.FromSeconds(0.15));
            battForm.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void abWindowOpened()
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                                        (Duration)TimeSpan.FromSeconds(0.10));
            abForm.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        public void sendMObj(MainWindow battForm)
        {
            battForm.receive(battForm);
        }

        public void sendAObj(About abForm)
        {
            abForm.receive(abForm);
        }
    }
}