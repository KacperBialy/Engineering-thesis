using System;
using System.Text;
using System.Threading;
using System.Windows;

namespace CubliApp
{
    class Bluetooth
    {
        Ports port = new Ports();
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
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
        public void UpdateConfiguration(string group, string value)
        {
            port.Update(group, value);
        }
        public bool Connect(MainWindow window, Bluetooth bluetooth)
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
                        IsPortOpen = true;
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
        public bool Disconnect(MainWindow window)
        {
            bool isDisconnectOK = false;

            IsPortOpen = false;
            readThread.Join();

            string endFrame = port.TestDisconnect();
            logger.Info($"End frame received: {endFrame}");

            if (endFrame.Contains("END"))
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
        bool ConnectionTest(MainWindow window)
        {
            string startFrame = port.TestConnection();
            logger.Info($"Initial frame received: {startFrame}");
            if (port.TestConnection().Contains("OK"))
            {
                dataReceived.Append("Start of transmission\n");

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

                    if (dataReceived.Length > 5000)
                    {
                        string[] data_splited = dataReceived.ToString().Split('\n');

                        int range = 0;
                        for (int i = 0; i < 50; i++)
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
    }
}
