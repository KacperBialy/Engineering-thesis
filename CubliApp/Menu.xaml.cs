using System;
using System.Windows;
using System.Windows.Controls;

namespace CubliApp
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        int maximized = 0;
        Window windowPort = new PortConfiguration();
        Window windowCubeControll = new CubeControll();
        private const string MODEL_PATH = "cube.obj";
        private Plots plot;
        private bool JustChecked;
        private bool[] AxisEnables = new bool[8];
        static System.Timers.Timer _timer = new System.Timers.Timer();

        public Menu()
        {
            windowPort.Visibility = Visibility.Hidden;
            windowCubeControll.Visibility = Visibility.Hidden;

            InitializeComponent();

            plot = new Plots(plt_sensor_1, plt_sensor_2, plt_sensor_3);
            plot.CreatePlots();

            lbl_Time.Content = DateTime.Now.ToShortTimeString();

            _timer.Interval = 60000;
            _timer.Elapsed += UpdateClock;
            _timer.Enabled = true;
        }
        private void UpdateClock(object sender, EventArgs e)
        {
            windowPort.Dispatcher.BeginInvoke(new Action(() =>
            {
                lbl_Time.Content = DateTime.Now.ToShortTimeString();

            }));
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_Maximized(object sender, RoutedEventArgs e)
        {
            if (maximized == 0)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
            maximized++;
            maximized = maximized % 2;
        }

        private void Button_Click_Minimized(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btn_PortConfig_Click(object sender, RoutedEventArgs e)
        {
            windowPort.Visibility = Visibility.Visible;
        }

        private void btn_Plot_Click(object sender, RoutedEventArgs e)
        {
            grid_plots.Visibility = Visibility.Visible;

        }

        private void btn_controlCube_Click(object sender, RoutedEventArgs e)
        {
            windowCubeControll.Visibility = Visibility.Visible;
        }

        
        private void RadioButton_CheckedPlots(object sender, RoutedEventArgs e)
        {
            RadioButton s = (RadioButton)sender;

            if (s.GroupName == "AccX")
                AxisEnables[0] = true;
            else if (s.GroupName == "AccY")
                AxisEnables[1] = true;
            else if (s.GroupName == "AccZ")
                AxisEnables[2] = true;
            else if (s.GroupName == "GyrX")
                AxisEnables[3] = true;
            else if (s.GroupName == "GyrY")
                AxisEnables[4] = true;
            else if (s.GroupName == "GyrZ")
                AxisEnables[5] = true;
            else if (s.GroupName == "Roll")
                AxisEnables[6] = true;
            else if (s.GroupName == "Pitch")
                AxisEnables[7] = true;
            Plots.setAxisEnables(AxisEnables);

            JustChecked = true;
        }

        private void RadioButton_ClickPlots(object sender, RoutedEventArgs e)
        {
            if (JustChecked)
            {
                JustChecked = false;
                e.Handled = true;
                return;
            }
            RadioButton s = (RadioButton)sender;
            if (s.IsChecked == true)
            {
                if (s.GroupName == "AccX")
                    AxisEnables[0] = false;
                else if (s.GroupName == "AccY")
                    AxisEnables[1] = false;
                else if (s.GroupName == "AccZ")
                    AxisEnables[2] = false;
                else if (s.GroupName == "GyrX")
                    AxisEnables[3] = false;
                else if (s.GroupName == "GyrY")
                    AxisEnables[4] = false;
                else if (s.GroupName == "GyrZ")
                    AxisEnables[5] = false;
                else if (s.GroupName == "Roll")
                    AxisEnables[6] = false;
                else if (s.GroupName == "Pitch")
                    AxisEnables[7] = false;
                Plots.setAxisEnables(AxisEnables);
                s.IsChecked = false;
            }
        }

        private void btn_Github_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/KacperBialy/Engineering-thesis");
        }
    }
}
