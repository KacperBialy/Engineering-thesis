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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace Navigation_Drawer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         int maximized = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ListViewItem_MouseUp_Contributors(object sender, MouseButtonEventArgs e)
        {
            GridContributors.Visibility = Visibility.Visible;
            GridHome.Visibility = Visibility.Collapsed;
            GridPlot.Visibility = Visibility.Collapsed;
            GridSettings.Visibility = Visibility.Collapsed;
        }

        private void ListViewItem_MouseUp_GitHub(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/");
        }

        private void ListViewItem_MouseUp_Home(object sender, MouseButtonEventArgs e)
        {
            GridContributors.Visibility = Visibility.Collapsed;
            GridHome.Visibility = Visibility.Visible;
            GridPlot.Visibility = Visibility.Collapsed;
            GridSettings.Visibility = Visibility.Collapsed;
        }

        private void ListViewItem_MouseUp_Settings(object sender, MouseButtonEventArgs e)
        {
            GridContributors.Visibility = Visibility.Collapsed;
            GridHome.Visibility = Visibility.Collapsed;
            GridPlot.Visibility = Visibility.Collapsed;
            GridSettings.Visibility = Visibility.Visible;
        }

        private void PackIcon_MouseUp_Plot(object sender, MouseButtonEventArgs e)
        {
            GridContributors.Visibility = Visibility.Collapsed;
            GridHome.Visibility = Visibility.Collapsed;
            GridPlot.Visibility = Visibility.Visible;
            GridSettings.Visibility = Visibility.Collapsed;
        }

        private void button_Minimalize_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }        
        private void button_Maximalize_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();

        }

        private void Button_MouseEnter_effect(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Background = (Brush) new BrushConverter().ConvertFrom("#FF386A7F");
        }

        private void Button_MouseLeave_effect(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Background = Brushes.Transparent;
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

    }
}
