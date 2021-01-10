using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;

namespace CubliApp
{
    public partial class PortConfiguration : Window
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static Bluetooth bluetooth = new Bluetooth();
        public PortConfiguration()
        {
            Logger.Info($"App start");
            InitializeComponent();
            ReScanPorts(); 
        }
        private void ReScanPorts()
        {
            comboBox_COMS.Items.Clear();
            var coms = SerialPort.GetPortNames().OrderBy(name => name);
            foreach (string name in coms)
            {
                if (comboBox_COMS.Items.Contains(name) == false)
                {
                    comboBox_COMS.Items.Add(name);
                }
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
        private void btn_ReScan_Click(object sender, RoutedEventArgs e)
        {
            ReScanPorts();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            bluetooth.UpdateConfiguration(radioButton.GroupName, radioButton.Content.ToString());
        }

        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            bool isConnection = bluetooth.Connect(this, bluetooth);
            if (isConnection == true)
            {
                btn_Disconnect.IsEnabled = true;
                lbl_disconnect_connect.Foreground = Brushes.Green;
                lbl_disconnect_connect.Content = "Connected";
                btn_Connect.IsEnabled = false;
            }
        }


        private void btn_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            
            bool isDisconnectOK = bluetooth.Disconnect(this);
            if (isDisconnectOK == true)
            {
                btn_Connect.IsEnabled = true;
                lbl_disconnect_connect.Foreground = Brushes.Red;
                lbl_disconnect_connect.Content = "Disconnected";
                btn_Disconnect.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Failed to disconnect", "Port problem", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            string data = txtBox_DataToSend.Text;
            bluetooth.SendData(data, this);
        }

        private void btn_ClearAll_Click(object sender, RoutedEventArgs e)
        {
            bluetooth.dataReceived = new StringBuilder();
            bluetooth.dataSend = new StringBuilder();
            txtBox_receivedMessages.Text = bluetooth.dataReceived.ToString();
            txtBox_sendMessages.Text = bluetooth.dataReceived.ToString();
        }

        private void comboBox_COMS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_COMS.SelectedValue != null)
                bluetooth.UpdateConfiguration("Port",comboBox_COMS.SelectedValue.ToString());
        }

    }
}
