using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Threading;
using System.Windows.Threading;

namespace BatteryIcon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
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
        public TextBlock SetSliderDescription()
        {
            return this.FindName("SliderDescription") as TextBlock;
        }

        public Slider SetPowerSlider()
        {
            return this.FindName("PowerSlider") as Slider;
        }

        public Slider SetBrightnessSlider()
        {
            return this.FindName("BrightnessSlider") as Slider;
        }
    }
}
