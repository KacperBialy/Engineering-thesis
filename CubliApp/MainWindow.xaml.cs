using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace CubliApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int boudRate { get; set; }
        public int dataBits { get; set; }
        public string parity { get; set; }
        public string stopBits { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_ReScan_Click(object sender, RoutedEventArgs e)
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                if (comboBox_COMS.Items.Contains(s) == false)
                {
                    comboBox_COMS.Items.Add(s);
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.GroupName == "BaudRate")
                boudRate = int.Parse(radioButton.Content.ToString());
            else if (radioButton.GroupName == "DataBits")
                dataBits = int.Parse(radioButton.Content.ToString());
            else if (radioButton.GroupName == "Parity")
                parity = radioButton.Content.ToString();
            else if (radioButton.GroupName == "StopBits")
                stopBits = radioButton.Content.ToString();

        }

        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            SerialPort _serialPort;

            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();
            // Allow the user to set the appropriate properties.
            _serialPort.PortName = "COM4";
            _serialPort.BaudRate = boudRate;
            _serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), parity, true);
            _serialPort.DataBits = dataBits;
            _serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);

        }
    }
}
