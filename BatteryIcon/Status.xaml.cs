using System;
using System.Windows;

namespace BatteryIcon
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Status : Window
    {
        private Status statForm;

        public Status()
        {
            InitializeComponent();
        }

        public void receive(Status form)
        {
            statForm = form;
            //receive settings form object
        }

        public void setPowerState(string s)
        {
            PowerState.Dispatcher.Invoke(() =>
            {
                PowerState.Text = s;
            });
        }

        public void setWearLevel(string s)
        {
            WearLevel.Dispatcher.Invoke(() =>
            {
                WearLevel.Text = s;
            });
        }

        public void setRate(string s)
        {
            Rate.Dispatcher.Invoke(() =>
            {
                Rate.Text = s;
            });
        }

        public void setDesignCapacity(string s)
        {
            DesignCapacity.Dispatcher.Invoke(() =>
            {
                DesignCapacity.Text = s;
            });
        }

        public void setFullyChargedCapacity(string s)
        {
            FullyChargedCapacity.Dispatcher.Invoke(() =>
            {
                FullyChargedCapacity.Text = s;
            });
        }

        public void setCurrentCapacity(string s)
        {
            CurrentCapacity.Dispatcher.Invoke(() =>
            {
                CurrentCapacity.Text = s;
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Collapsed;
            //receive MainWindow Object and set initial slider positions and text fields
        }
    }
}
