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

namespace CubliApp
{
    /// <summary>
    /// Interaction logic for CubeControll.xaml
    /// </summary>
    public partial class CubeControll : Window
    {
        public CubeControll()
        {
            InitializeComponent();
            ViewModel3d.CreateModel(ref viewPort3d);
        }
        private void btn_Rotate(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            if (button.Name == "btn_up")
                ViewModel3d.Rotate("x", '+', 45);

            else if (button.Name == "btn_upJump")
                ViewModel3d.Rotate("x", '+', 90);

            else if (button.Name == "btn_left")
                ViewModel3d.Rotate("y", '-', 45);

            else if (button.Name == "btn_leftJump")
                ViewModel3d.Rotate("y", '-', 90);
        }

        private void btn_ResetView_Click(object sender, RoutedEventArgs e)
        {
            ViewModel3d.Reset();
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            ViewModel3d.Start();
        }
    }
}
