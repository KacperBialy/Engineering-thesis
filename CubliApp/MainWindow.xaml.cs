using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.IO.Ports;
using System.Windows.Media;
using System.Text;
using System;

namespace CubliApp
{
    public partial class MainWindow : Window
    {

        Bluetooth bluetooth = new Bluetooth();
        public MainWindow()
        {
            InitializeComponent();
            ReScanPorts();
        }

        private void ReScanPorts()
        {
            comboBox_COMS.Items.Clear();
            foreach (string s in SerialPort.GetPortNames())
            {
                if (comboBox_COMS.Items.Contains(s) == false)
                {
                    comboBox_COMS.Items.Add(s);
                }
            }
        }
        private void btn_ReScan_Click(object sender, RoutedEventArgs e)
        {
            ReScanPorts();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.GroupName == "BaudRate")
                bluetooth.BoudRate = int.Parse(radioButton.Content.ToString());
            else if (radioButton.GroupName == "DataBits")
                bluetooth.DataBits = int.Parse(radioButton.Content.ToString());
            else if (radioButton.GroupName == "Parity")
                bluetooth.Parity = radioButton.Content.ToString();
            else if (radioButton.GroupName == "StopBits")
                bluetooth.StopBits = radioButton.Content.ToString();
        }

        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            btn_Disconnect.IsEnabled = true;
            bool isConnection = bluetooth.Connect(this,bluetooth);
            if (isConnection == true)
            {
                lbl_disconnect_connect.Foreground = Brushes.Green;
                lbl_disconnect_connect.Content = "Connected";
                btn_Connect.IsEnabled = false;
            }
        }


        private void btn_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            btn_Connect.IsEnabled = true;
            bool isDisconnectOK = bluetooth.Disconnect(this);
            if (isDisconnectOK == true)
            {
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
            bluetooth.SendData(data,this);
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
            if(comboBox_COMS.SelectedValue!=null)
                bluetooth.PortName = comboBox_COMS.SelectedValue.ToString();
        }
    }
}
