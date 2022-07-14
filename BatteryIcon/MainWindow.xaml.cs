using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace BatteryIcon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static int brightness;
        private MainWindow battForm;

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

        public void Power_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            //change windows power mode here
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
                battForm.BrightnessSlider.Value = brightness;
                battForm.Bright_Percent.Text = brightness.ToString() + "%";
            });
        }

        private void Window_LostFocus(Object sender, EventArgs e)
        {
            battForm.Hide();
        }

    }
}
