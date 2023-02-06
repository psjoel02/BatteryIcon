using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BatteryIcon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private static int brightness;
        private static PowerStatus _pwr = SystemInformation.PowerStatus;
        private MainWindow battForm;
        System.Windows.Threading.DispatcherTimer _timer;

        public static Guid BestPerformance = new Guid("ded574b5-45a0-4f42-8737-46345c09c238");
        public static Guid BetterPerformance = new Guid("00000000-0000-0000-0000-000000000000");
        public static Guid BetterBattery = new Guid("961cc777-2547-4f9d-8174-7d86181b8a7a");
        public static Guid BatterySaver = new Guid("3af9B8d9-7c97-431d-ad78-34a8bfea439f");

        [DllImportAttribute("powrprof.dll", EntryPoint = "PowerSetActiveOverlayScheme")]
        private static extern uint PowerSetActiveOverlayScheme(Guid OverlaySchemeGuid);

        [DllImportAttribute("powrprof.dll", EntryPoint = "PowerGetEffectiveOverlayScheme")]
        public static extern uint PowerGetEffectiveOverlayScheme(out Guid EffectiveOverlayGuid);
        //set GUIDs for power modes and import powrprof.dll methods using Pinvoke

        public MainWindow()
        {
            InitializeComponent();
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Tick += timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 5); 
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("ms-settings:powersleep");
            //if settings is clicked in the main window, open Window Settings for Power
        }

        public void SetIcon(string s)
        {
            BatteryIcon.Dispatcher.Invoke(() =>
            {
                ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                new Icon($"{s}").Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
                BatteryIcon.Source = imageSource;
                //create icon for main menu
            });
        }
        public void SetPercent(string s)
        {
            Percent.Dispatcher.Invoke(() =>
            {
                Percent.Text = s;
                //set text for percent in main menu
            });
        }

        public void SetTimeRemaining(string s)
        {
            TimeRemaining.Dispatcher.Invoke(() =>
            { 
                TimeRemaining.Text = s;
                //set text for time remaining in main menu
            });
        }
        public void SetSliderDescription(string s)
        {
            SliderDescription.Dispatcher.Invoke(() =>
            {
                SliderDescription.Text = s;
                //set slider description to match Windows 10's text
            });
        }

        public void PowerSlider_ValueChanged(Object sender, EventArgs e)
        {

            if (_pwr.PowerLineStatus == System.Windows.Forms.PowerLineStatus.Online)
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (battForm.PowerSlider.Value == 240)
                        {
                            battForm.SliderDescription.Text = "Power mode (plugged in): Best performance";
                            PowerSetActiveOverlayScheme(BestPerformance);
                        }
                        else if (battForm.PowerSlider.Value == 160)
                        {
                            battForm.SliderDescription.Text = "Power mode (plugged in): Better performance";
                            PowerSetActiveOverlayScheme(BetterPerformance);
                        }
                        else
                        {
                            battForm.SliderDescription.Text = "Power mode (plugged in): Better battery";
                            PowerSetActiveOverlayScheme(BetterBattery);
                        }
                        //update UI when value is changed and apply new power mode on AC power
                    });
                }
                catch (AccessViolationException)
                {
                    Environment.Exit(0);
                    throw;
                    //catch if used on non x64 systems and exit application
                }
            }
            else
            {
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (battForm.PowerSlider.Value == 240)
                        {
                            battForm.SliderDescription.Text = "Power mode (on battery): Best performance";
                            PowerSetActiveOverlayScheme(BestPerformance);
                        }
                        else if (battForm.PowerSlider.Value == 160)
                        {
                            battForm.SliderDescription.Text = "Power mode (on battery): Better performance";
                            PowerSetActiveOverlayScheme(BetterPerformance);
                        }
                        else if (battForm.PowerSlider.Value == 80)
                        {
                            battForm.SliderDescription.Text = "Power mode (on battery): Better battery";
                            PowerSetActiveOverlayScheme(BetterBattery);
                        }
                        else
                        {
                            battForm.SliderDescription.Text = "Power mode (on battery): Battery saver";
                            PowerSetActiveOverlayScheme(BatterySaver);
                        }
                        //update UI when value is changed and apply new power mode on DC power
                    });
                }
                catch (AccessViolationException)
                {
                    Environment.Exit(0);
                    throw;
                    //catch if used on non x64 systems and exit application
                }
            }
        }

        private void Brightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            if (brightness != battForm.BrightnessSlider.Value)
            {
                BrightnessSlider.Dispatcher.Invoke(() =>
                {
                    Brightness.SetBrightness((int)battForm.BrightnessSlider.Value);
                    brightness = (int)battForm.BrightnessSlider.Value;
                    battForm.Bright_Percent.Text = brightness.ToString() + "%";
                    //if main window's field brightness is not equal to the window's brightness,
                    //change it and update the UI
                });
            }
        }

        public void receive(MainWindow form)
        {
            battForm = form;
            this.Dispatcher.Invoke(() =>
            {
                brightness = Brightness.GetBrightness();
                if (PowerGetEffectiveOverlayScheme(out Guid activeScheme) == 0)
                {
                    
                    if(activeScheme == BestPerformance)
                    {
                        battForm.PowerSlider.Value = 240;
                    }
                    else if(activeScheme == BetterPerformance)
                    {
                        battForm.PowerSlider.Value = 160;
                    }
                    else if(activeScheme == BetterBattery)
                    {
                        battForm.PowerSlider.Value = 80;
                    }
                    else
                    {
                        battForm.PowerSlider.Value = 0;
                    }
                }
                battForm.BrightnessSlider.Value = brightness;
                battForm.Bright_Percent.Text = brightness.ToString() + "%";
                //receive MainWindow Object and set initial slider positions and text fields
            });
        }


        private void Window_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            this.Hide();
        }
    }
}
