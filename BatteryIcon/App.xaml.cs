using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Animation;
using Forms = System.Windows.Forms;
using Windows.Devices.Power;

namespace BatteryIcon
{
    public partial class App : Application
    {
        private static bool open = false;
        private static bool notified = false;
        //bool reportRequested = false;

        private static Forms.NotifyIcon _notifyIcon = new Forms.NotifyIcon();
        private static Forms.PowerStatus _pwr = Forms.SystemInformation.PowerStatus;

        private MainWindow battForm = new MainWindow();
        private About abForm = new About();
        private Status statForm = new Status();


        public App()
        {
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            SetIcon(_pwr);
            SetText(_pwr);
            sendMObj(battForm);
            sendAObj(abForm);
            _notifyIcon.MouseClick += notifyIcon_MouseClick;

            _notifyIcon.ContextMenu = new Forms.ContextMenu();
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("About", OnAboutClicked));
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("Power", OnPowerClicked));
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("Notify When Charge is Full", OnFullClicked));
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("Display Charge Percentage", OnChargeClicked));
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("Battery Status", OnStatusClicked));
            _notifyIcon.ContextMenu.MenuItems.Add(new Forms.MenuItem("Exit", OnExitClicked));
            _notifyIcon.Visible = true;
            //create notifyIcon
            //user can autostart with windows by adding exe to shell:startup
            base.OnStartup(e);

        }

        private void OnAboutClicked(object sender, EventArgs e)
        {
            abForm.Left = SystemParameters.WorkArea.Width - 350 - 2;
            abForm.Top = SystemParameters.WorkArea.Height - 270 - 2;
            abForm.Show();
            abForm.Activate();
            abWindowOpened();
            sendAObj(abForm);
            //if about is clicked, display About window
        }

        private void OnPowerClicked(object sender, EventArgs e)
        {
            var cplPath = System.IO.Path.Combine(Environment.SystemDirectory, "control.exe");
            System.Diagnostics.Process.Start(cplPath, "/name Microsoft.PowerOptions");
            //if power is clicked, open a window in Windows Control Panel's Power settings
        }

        private void OnFullClicked(object sender, EventArgs e)
        {
            if(BatteryIcon.Properties.Settings.Default.notifyFull == false)
            {
                BatteryIcon.Properties.Settings.Default.notifyFull = true;
                BatteryIcon.Properties.Settings.Default.Save();
                notify100(_pwr);
                MessageBox.Show("BatteryIcon will notify you when charge is 100%");
                //if option to notify user when battery is full is false,
                //set it to true and update data (user will be notified)
            }
            else
            {
                BatteryIcon.Properties.Settings.Default.notifyFull = false;
                BatteryIcon.Properties.Settings.Default.Save();
                notify100(_pwr);
                MessageBox.Show("BatteryIcon will not notify you when charge is 100%");
                //if option to notify user when battery is full is true,
                //set it to false and update data (user will not be notified)
            }
        }

        private void OnChargeClicked(object sender, EventArgs e)
        {
            if(BatteryIcon.Properties.Settings.Default.displayPercent == false)
            {
                BatteryIcon.Properties.Settings.Default.displayPercent = true;
                BatteryIcon.Properties.Settings.Default.Save(); 
                notify100(_pwr);
                SetIcon(_pwr);
                SetText(_pwr);
                sendMObj(battForm);
                sendAObj(abForm);
                //if option to display number in tray is false,
                //set it to true and update data (numerical percentage is used)
            }
            else
            {
                BatteryIcon.Properties.Settings.Default.displayPercent = false;
                BatteryIcon.Properties.Settings.Default.Save();
                notify100(_pwr);
                SetIcon(_pwr);
                SetText(_pwr);
                sendMObj(battForm);
                sendAObj(abForm);
                //if option to display number in tray is true,
                //set it to false and update data (battery icon is used)
            }
        }

        private void OnStatusClicked(object sender, EventArgs e)
        {
            var report = RequestAggregateBatteryReport();
            int wear = (int)((report.DesignCapacityInMilliwattHours / report.FullChargeCapacityInMilliwattHours) /
                (report.FullChargeCapacityInMilliwattHours) * 100);

            statForm.setPowerState(_pwr.PowerLineStatus.ToString());
            statForm.setWearLevel(wear.ToString() + "%");
            statForm.setRate(report.ChargeRateInMilliwatts.ToString() + " mW");
            statForm.setDesignCapacity(report.DesignCapacityInMilliwattHours.ToString() + " mWh");
            statForm.setFullyChargedCapacity(report.FullChargeCapacityInMilliwattHours.ToString() + " mWh");
            statForm.setCurrentCapacity(report.RemainingCapacityInMilliwattHours.ToString() + " mWh");

            statForm.Left = SystemParameters.WorkArea.Width - 350 - 2;
            statForm.Top = SystemParameters.WorkArea.Height - 270 - 2;
            statForm.Show();
            statForm.Activate();
            statWindowOpened();
            sendSObj(statForm);
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Icon = null;
            _notifyIcon.Dispose();
            Environment.Exit(0);
            //if exit is clicked, clear icon and exit application
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

                    //if main menu is not open and notifyIcon is clicked, open it and set bool open to true
                }
                else
                {
                    open = false;
                    battForm.Hide();
                    if (abForm.IsActive)
                    {
                        abForm.Hide();
                    }
                    //if main menu is open and notifyIcon is clicked, hide window and set bool open to false
                }

            }

        }

        public void SetIcon(Forms.PowerStatus _pwr)
        {

            Double BatteryLifeRemaining = Math.Round(_pwr.BatteryLifeRemaining / 60.0);
            int Batterylife = (int)(_pwr.BatteryLifePercent * 100);
            //if the button to display battery percent as number in tray is not clicked, 
            //display charge using color-coordinated and percent-reactive battery icon

            if(BatteryIcon.Properties.Settings.Default.displayPercent == false) 
            {
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
                                //when battery has left low levels, set ability to be notified about 
                                //low charge to true (i.e. user is not notified yet)
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
                                //when battery is within low levels, set ability to be notified about
                                //low charge to false (i.e. user is notified)
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
            else
            {
                Bitmap bmp = new Bitmap(18, 18);
                RectangleF rectf = new RectangleF(2, 2, 32, 32);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawString(Batterylife.ToString(), new Font("segoeui", 7), Brushes.White, rectf);
                Forms.PictureBox pictureBox1 = new Forms.PictureBox();
                pictureBox1.Image = bmp;
                pictureBox1.Height = bmp.Height;
                pictureBox1.Width = bmp.Width;
                g.Dispose();
                var thumb = (Bitmap)bmp.GetThumbnailImage(256, 256, null, IntPtr.Zero);
                thumb.MakeTransparent();
                _notifyIcon.Icon = Icon.FromHandle(thumb.GetHicon());

                /*when user has selected to display number instead of icon,
                  generate picturebox using bitmap and graphics, then dispose of graphics
                  before creating a notify icon from the picturebox */
            }
        }

        public void SetText(Forms.PowerStatus _pwr)
        {
            Double BatteryLifeRemaining = Math.Round(_pwr.BatteryLifeRemaining / 60.0);
            Double Batterylife = (int)(_pwr.BatteryLifePercent * 100);
            battForm.SetPercent(Batterylife.ToString() + "%");

            //if user is on AC power, display correct text based on power slider value
            if (_pwr.PowerLineStatus == Forms.PowerLineStatus.Online)
            {
                _notifyIcon.Text = Batterylife.ToString() + "% available (plugged in)";
                battForm.SetTimeRemaining(Batterylife.ToString() + "% available (plugged in)");
                this.Dispatcher.Invoke(() =>
                {
                    if (battForm.PowerSlider.Value == 240)
                    {
                        battForm.SliderDescription.Text = "Power mode (plugged in): Best performance";
                    }
                    else if (battForm.PowerSlider.Value == 160)
                    {
                        battForm.SliderDescription.Text = "Power mode (plugged in): Better performance";
                    }
                    else
                    {
                        battForm.SliderDescription.Text = "Power mode (plugged in): Better battery";
                    }
                    //use dispatcher to get UI thread to update elements
                });
            }
            else
            {
                int hrs = (int)(BatteryLifeRemaining / 60);
                int mins = (int)(BatteryLifeRemaining % 60);

                //if user is on DC power, display correct text based on power slider value
                if (hrs == 0)
                {
                    _notifyIcon.Text = Batterylife.ToString() + "%" + " remaining";
                    battForm.SetTimeRemaining(Batterylife.ToString() + "%" + " remaining");
                    //if user has not synchronized battery time remaining, set default text
                }
                else
                {
                    _notifyIcon.Text = $"{hrs} hr {mins} min " + " " + "(" + Batterylife.ToString() + "%" + ") remaining";
                    battForm.SetTimeRemaining($"{hrs} hr {mins} minutes " + "remaining");
                }

                this.Dispatcher.Invoke(() =>
                {
                    if (battForm.PowerSlider.Value == 240)
                    {
                        battForm.SliderDescription.Text = "Power mode (on battery): Best performance";
                    }
                    else if (battForm.PowerSlider.Value == 160)
                    {
                        battForm.SliderDescription.Text = "Power mode (on battery): Better performance";
                    }
                    else if (battForm.PowerSlider.Value == 80)
                    {
                        battForm.SliderDescription.Text = "Power mode (on battery): Better battery";
                    }
                    else
                    {
                        battForm.SliderDescription.Text = "Power mode (on battery): Battery saver";
                    }
                    //use dispatcher to get UI thread to update elements
                });
            }
        }

        public void notify100(Forms.PowerStatus _pwr)
        {
            Double Batterylife = (int)(_pwr.BatteryLifePercent * 100);
            if (BatteryIcon.Properties.Settings.Default.notifyFull == true && Batterylife == 100)
            {
                new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9814)
                .AddText("Full charge")
                .AddText("Battery charge is full (100%)")
                .Show();
            }
            //if battery level is full and user has selected to notify on full
            //charge, display Windows toast at 100%
        }

        private BatteryReport RequestAggregateBatteryReport()
        {
            // Create aggregate battery object
            var aggBattery = Battery.AggregateBattery;

            // Get report
            var report = aggBattery.GetReport();

            return report;
            // Update UI
            //AddReportUI(BatteryReportPanel, report, aggBattery.DeviceId);
        }

        private void AggregateBattery_ReportUpdated(Battery sender, object args)
        {
            //make async
            notify100(_pwr);
            SetIcon(_pwr);
            SetText(_pwr);
            sendMObj(battForm);
            sendAObj(abForm);
            //first check for 100% notification, then update icons, text,
            //and send window obj to respective classes
        }

        private void battWindow_Opened()
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                                        (Duration)TimeSpan.FromSeconds(0.15));
            battForm.BeginAnimation(UIElement.OpacityProperty, animation);
            //display fade animation for battery window
        }

        private void abWindowOpened()
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                                        (Duration)TimeSpan.FromSeconds(0.10));
            abForm.BeginAnimation(UIElement.OpacityProperty, animation);
            //display fade animation for about window
        }

        private void statWindowOpened()
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                                        (Duration)TimeSpan.FromSeconds(0.10));
            statForm.BeginAnimation(UIElement.OpacityProperty, animation);
            //display fade animation for settings window
        }

        public void sendMObj(MainWindow battForm)
        {
            battForm.receive(battForm);
            //send main window object to mainwindow c# file
        }

        public void sendAObj(About abForm)
        {
            abForm.receive(abForm);
            //send about window object to about c# file
        }

        public void sendSObj(Status statForm)
        {
            statForm.receive(statForm);
            //send status window to status c# file
        }
    }
}