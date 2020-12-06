using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows;

namespace CubliApp
{
    class Bluetooth
    {
        Ports port  = new Ports();

        public StringBuilder dataReceived { get; set; }
        public StringBuilder dataSend { get; set; }
        private Thread readThread { get; set; }
        private bool IsPortOpen { get; set; }
        public Bluetooth()
        {
            dataReceived = new StringBuilder();
            dataSend = new StringBuilder();
        }
        private bool Configuration()
        {
            
                bool isConfigurationOK = port.CreatePort();
                return isConfigurationOK;

        }
    public void UpdateConfiguration(string group,string value)
        {
            port.Update(group, value);
        }
        public bool Connect( MainWindow window, Bluetooth bluetooth)
        {
            bool isConfigurationOK = Configuration();

            if (isConfigurationOK == true)
            {
                try
                {
                    port.serialPort.Open();
                    IsPortOpen = ConnectionTest(window);
                    if (IsPortOpen  == true)
                    {
                        IsPortOpen = true;
                        readThread = new Thread(() => bluetooth.Read(window));
                        readThread.Start();
                    }
                }
                catch
                {
                    IsPortOpen = false;
                    MessageBox.Show("You have chosen the wrong port", "Port problem", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            return IsPortOpen;
        }
        public bool Disconnect(MainWindow window)
        {
            bool isDisconnectOK = false;

            IsPortOpen = false;
            readThread.Join();

            if (port.TestDisconnect() == "END")
            {

                isDisconnectOK = true;
                dataReceived.Append("End of transmission\n");
                port.serialPort.Close();
                window.Dispatcher.BeginInvoke(new Action(() =>
                {
                    window.txtBox_receivedMessages.Text = dataReceived.ToString();
                }));
                return isDisconnectOK;
            }
            else
            {
                IsPortOpen = true;
                return isDisconnectOK;
            }
        }
        bool ConnectionTest(MainWindow window)
        {

            if (port.TestConnection() == "OK")
            {
                dataReceived.Append("Start of transmission\n");
                window.Dispatcher.BeginInvoke(new Action(() =>
                {
                    window.txtBox_receivedMessages.Text = dataReceived.ToString();
                }));
                return true;
            }
            else
                return false;
        }
        public void SendData(string data, MainWindow window)
        {
            if (IsPortOpen)
            {
                port.serialPort.Write(data);
                dataSend.Append($"{data}\n");
                window.Dispatcher.BeginInvoke(new Action(() =>
                {
                    window.txtBox_sendMessages.Text = dataSend.ToString();
                }));
            }
        }
        public void Read(MainWindow window)
        {
            while (IsPortOpen)
            {
                try
                {
                    dataReceived.Append($"{port.serialPort.ReadTo("#")}\n");
                    window.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        window.txtBox_receivedMessages.Text = dataReceived.ToString();
                    }));
                }
                catch (TimeoutException) { }
            }
        }
    }
}
