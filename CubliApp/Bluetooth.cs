using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows;

namespace CubliApp
{
    class Bluetooth
    {
        private SerialPort _serialPort;

        public int BoudRate { get; set; }
        public int DataBits { get; set; }
        public string Parity { get; set; }
        public string StopBits { get; set; }
        public string PortName { get; set; }
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
            bool isConfigurationOK = false;
            if (PortName != null)
            {
                _serialPort = new SerialPort();

                _serialPort.PortName = PortName;
                _serialPort.BaudRate = BoudRate;
                _serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), Parity, true);
                _serialPort.DataBits = DataBits;
                _serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), StopBits, true);
                _serialPort.ReadTimeout = 5000;
                _serialPort.WriteTimeout = 5000;

                isConfigurationOK = true;

                return isConfigurationOK;
            }
            else
            {
                MessageBox.Show("Select a port","Port problem",MessageBoxButton.OK,MessageBoxImage.Warning);
                return isConfigurationOK;
            }
        }
        public bool Connect( MainWindow window, Bluetooth bluetooth)
        {
            bool isConfigurationOK = Configuration();

            if (isConfigurationOK == true)
            {
                try
                {
                    _serialPort.Open();
                    IsPortOpen = ConnectionTest(window);
                    if (IsPortOpen == true)
                    {
                        readThread = new Thread(() => bluetooth.Read(window));
                        readThread.Start();
                    }
                }
                catch
                {
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

            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
            _serialPort.Write("0"); // 0->DISCONNECTION COMMAND
            string message = _serialPort.ReadTo("#");
            if (message == "END")
            {

                isDisconnectOK = true;
                dataReceived.Append("End of transmission\n");
                _serialPort.Close();
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
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
            _serialPort.Write("1"); // 1->CONNECTION COMMAND
            string message = _serialPort.ReadTo("#");
            if (message == "OK")
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
                _serialPort.Write(data);
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
                    dataReceived.Append($"{_serialPort.ReadTo("#")}\n");
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
