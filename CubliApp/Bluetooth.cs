using System;
using System.Text;
using System.Threading;
using System.Windows;

namespace CubliApp
{
    class Bluetooth
    {
        Ports port;
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public StringBuilder dataReceived { get; set; }
        public StringBuilder dataSend { get; set; }
        public StringBuilder dataToPlot { get; set; }
        private Thread readThread { get; set; }
        private bool IsPortOpen { get; set; }
        private static bool statusOfPlot { get; set; }
        public Bluetooth()
        {
            dataReceived = new StringBuilder();
            dataToPlot = new StringBuilder();
            dataSend = new StringBuilder();
            port = new Ports();
        }
        private bool Configuration()
        {

            bool isConfigurationOK = port.CreatePort();
            return isConfigurationOK;

        }
        public void UpdateConfiguration(string group, string value)
        {
            port.Update(group, value);
        }
        public bool Connect(PortConfiguration window, Bluetooth bluetooth)
        {
            bool isConfigurationOK = Configuration();

            if (isConfigurationOK == true)
            {
                try
                {
                    port.serialPort.Open();
                    logger.Info("Port Open");

                    IsPortOpen = ConnectionTest(window);
                    if (IsPortOpen == true)
                    {
                        logger.Info("Connected");
                        readThread = new Thread(() => bluetooth.Read(window));
                        readThread.Start();
                    }
                }
                catch
                {
                    IsPortOpen = false;
                    logger.Error("Problem with connection!");
                    MessageBox.Show("You have chosen the wrong port", "Port problem", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            return IsPortOpen;
        }
        public bool Disconnect(PortConfiguration window)
        {
            bool isDisconnectOK = false;

            IsPortOpen = false;
            readThread.Join();

            if (port.TestDisconnect())
            {
                isDisconnectOK = true;

                dataReceived.Append("End of transmission\n");

                port.serialPort.Close();
                logger.Info("Port closed");

                window.Dispatcher.BeginInvoke(new Action(() =>
                {
                    window.txtBox_receivedMessages.Text = dataReceived.ToString();
                }));

                return isDisconnectOK;
            }
            else
            {
                logger.Error("Problem with disconnection");
                IsPortOpen = true;
                return isDisconnectOK;
            }
        }
        bool ConnectionTest(PortConfiguration window)
        {
            if (port.TestConnection())
                return true;
            else
                return false;
        }
        public void SendData(string data, PortConfiguration window)
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
        public void Read(PortConfiguration window)
        {
            dataReceived = new StringBuilder();
            while (IsPortOpen)
            {
                try
                {
                    string readData = ($"{port.serialPort.ReadTo("#")}\n");
                    dataReceived.Append(readData);
                    sendDataToPlot(readData);
                    window.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        window.txtBox_receivedMessages.Text = dataReceived.ToString();
                    }));

                    if (dataReceived.Length > 5000)
                    {
                        string[] data_splited = dataReceived.ToString().Split('\n');

                        int range = 0;
                        for (int i = 0; i < data_splited.Length - 1; i++)
                        {
                            range += data_splited[i].Length + 1;
                        }
                        dataReceived.Remove(0, range);
                    }
                }
                catch (TimeoutException exception)
                {
                    logger.Error(exception.Message);
                }
            }
        }
        public static void SetStatusOfPlot(bool status)
        {
            statusOfPlot = status;
        }

        public void sendDataToPlot(string readData)
        {
            dataToPlot.Append(readData);
            if (statusOfPlot)
            {
                Plots.setData(dataToPlot);
                dataToPlot = new StringBuilder();
                SetStatusOfPlot(false);
            }

        }
    }
}
