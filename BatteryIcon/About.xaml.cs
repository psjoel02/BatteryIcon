using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        }

        private void License_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/psjoel02/BatteryIcon/blob/master/LICENSE");
        }

        private void Release_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/psjoel02/BatteryIcon/releases/");
        }

        public void receive(About form)
        {
            abForm = form;
            //create exe file for application

        }

        private void Window_LostFocus(Object sender, EventArgs e)
        {
            abForm.Hide();
        }
        //LostFocus="Window_LostFocus"

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
    }
}
