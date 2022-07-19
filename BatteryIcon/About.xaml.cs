using System;
using System.Windows;

namespace BatteryIcon
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class About : Window
    {
        private About abForm;

        public About()
        {
            InitializeComponent();
        }

        private void Github_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/psjoel02/BatteryIcon");
            //if Github link is clicked on, visit project repository
        }

        private void License_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/psjoel02/BatteryIcon/blob/master/LICENSE");
            //if License link is clicked on, visit license inside of BatteryIcon repository
        }

        private void Release_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/psjoel02/BatteryIcon/releases/");
            //if Release link is clicked on, visit releases page inside of BatteryIcon repository
        }

        public void receive(About form)
        {
            abForm = form;
            //receive about form object
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
            //instead of closing window, hide it from user.
        }
    }
}
