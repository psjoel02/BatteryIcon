using System.Windows;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

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

        public static Guid BestPerformance = new Guid("ded574b5-45a0-4f42-8737-46345c09c238");
        public static Guid BetterPerformance = new Guid("00000000-0000-0000-0000-000000000000");
        public static Guid BetterBattery = new Guid("961cc777-2547-4f9d-8174-7d86181b8a7a");
        public static Guid BatterySaver = new Guid("3af9B8d9-7c97-431d-ad78-34a8bfea439f");

        [DllImportAttribute("powrprof.dll", EntryPoint = "PowerSetActiveOverlayScheme")]
        private static extern uint PowerSetActiveOverlayScheme(Guid OverlaySchemeGuid);

        [DllImportAttribute("powrprof.dll", EntryPoint = "PowerGetEffectiveOverlayScheme")]
        public static extern uint PowerGetEffectiveOverlayScheme(out Guid EffectiveOverlayGuid);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("ms-settings:powersleep");
        }

        public void SetIcon(string s)
        {
            BatteryIcon.Dispatcher.Invoke(() =>
            {
                BatteryIcon.Source = new BitmapImage(new Uri(@s, UriKind.Relative));
            });
        }
        public void SetPercent(string s)
        {
            Percent.Dispatcher.Invoke(() =>
            {
                Percent.Text = s;
            });
        }

        public void SetTimeRemaining(string s)
        {
            TimeRemaining.Dispatcher.Invoke(() =>
            { 
                TimeRemaining.Text = s;
            });
        }
        public void SetSliderDescription(string s)
        {
            SliderDescription.Dispatcher.Invoke(() =>
            {
                SliderDescription.Text = s;
            });
        }

        public void PowerSlider_ValueChanged(Object sender, EventArgs e)
        {

                if (_pwr.PowerLineStatus == System.Windows.Forms.PowerLineStatus.Online)
                {
                    this.Dispatcher.Invoke(() =>
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
                    });
                }
                else
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
                        else if(battForm.PowerSlider.Value == 80)
                        {
                            battForm.SliderDescription.Text = "Power mode (on battery): Better battery";
                            PowerSetActiveOverlayScheme(BetterBattery);
                        }
                        else
                        {
                            battForm.SliderDescription.Text = "Power mode (on battery): Battery saver";
                            PowerSetActiveOverlayScheme(BatterySaver);
                        }
                    });
                }
        }

        private void Brightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (brightness != battForm.BrightnessSlider.Value)
            {
                this.Dispatcher.Invoke(() =>
                {
                    Brightness.SetBrightness((int)battForm.BrightnessSlider.Value);
                    brightness = (int)battForm.BrightnessSlider.Value;
                    battForm.Bright_Percent.Text = brightness.ToString() + "%";
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
            });
        }

        //LostFocus="Window_LostFocus"

    }
}
