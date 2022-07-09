using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Threading;
using Forms = System.Windows.Forms;
using Windows.Devices.Power;
using Windows.UI.Core;
using Microsoft.Win32;


namespace BatteryIcon
{
    // add rounded corners
    public partial class App : Application
    {
        private bool open = false;
        bool reportRequested = false;
        protected static Forms.NotifyIcon _notifyIcon = new Forms.NotifyIcon();
        private static Forms.PowerStatus _pwr = Forms.SystemInformation.PowerStatus;
        private MainWindow form = new MainWindow();
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

            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Status", Image.FromFile("Resources\\charging.ico"), OnStatusClicked);
            _notifyIcon.ContextMenuStrip.Items.Add("Test", null, OnTestClicked);
            _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, OnExitClicked);
            _notifyIcon.Visible = true;

            base.OnStartup(e);

        }

        private void OnStatusClicked(object sender, EventArgs e)
        {
            new ToastContentBuilder()
            .AddArgument("action", "viewConversation")
            .AddArgument("conversationId", 9813)
            .AddText("Low Battery")
            .AddText("Battery charge is less than 20%")
            .Show();
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Icon = null;
            _notifyIcon.Dispose();
            Environment.Exit(0);
        }

        private void OnTestClicked(object sender, EventArgs e)
        {
            //test functions here
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
            else
            {
                //do nothing....... right click is for context menu
            }


        }


        public static void SetIcon(Forms.PowerStatus _pwr)
        {

            Double BatteryLifeRemaining = Math.Round(_pwr.BatteryLifeRemaining / 60.0);
            Double Batterylife = Math.Round(_pwr.BatteryLifePercent * 100);

            if (_pwr.PowerLineStatus == Forms.PowerLineStatus.Unknown)
            {
                _notifyIcon.Icon = new Icon("Resources\\no_battery.ico");
            }
            else if (_pwr.PowerLineStatus == Forms.PowerLineStatus.Online)
            {
                _notifyIcon.Icon = new Icon("Resources\\charging.ico");
            }
            else if (Batterylife >= 80 && Batterylife <= 100)
            {
                _notifyIcon.Icon = new Icon("Resources\\high.ico");
            }
            else if (Batterylife >= 50 && Batterylife <= 79)
            {
                _notifyIcon.Icon = new Icon("Resources\\mid.ico");
            }
            else if (Batterylife >= 25 && Batterylife <= 49)
            {
                _notifyIcon.Icon = new Icon("Resources\\middle.ico");
            }
            else if (Batterylife >= 16 && Batterylife <= 24)
            {
                _notifyIcon.Icon = new Icon("Resources\\low.ico");
            }
            else if (Batterylife >= 15)
            {
                _notifyIcon.Icon = new Icon("Resources\\verylow.ico");
            }
        }

        public static void SetText(Forms.PowerStatus _pwr)
        {
            Double BatteryLifeRemaining = Math.Round(_pwr.BatteryLifeRemaining / 60.0);
            Double Batterylife = Math.Round(_pwr.BatteryLifePercent * 100);

            if (_pwr.PowerLineStatus == Forms.PowerLineStatus.Online)
            {
                _notifyIcon.Text = Batterylife.ToString() + "% available (plugged in)";
            }
            else
            {
                int hrs = (int)(BatteryLifeRemaining / 60);
                int mins = (int)(BatteryLifeRemaining % 60);
                if (hrs == 0)
                {
                    _notifyIcon.Text = "(" + Batterylife.ToString() + "%" + ") remaining";
                }
                else
                {
                    _notifyIcon.Text = $"{hrs} hr {mins} min " + " " + "(" + Batterylife.ToString() + "%" + ") remaining";
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