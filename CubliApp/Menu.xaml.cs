using System.Windows;

namespace CubliApp
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        int maximized = 0;
        Window window = new PortConfiguration();
        Plots plot;
        public Menu()
        {
            window.Visibility = Visibility.Hidden;
            InitializeComponent();
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
            window.Visibility = Visibility.Visible;
        }

        private void btn_Plot_Click(object sender, RoutedEventArgs e)
        {
            Plots plot = new Plots(plt_main);
            plot.CreatePlot();
        }
    }
}
