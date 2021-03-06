﻿using System;
using System.Text;
using System.Threading;
using System.Windows;

namespace CubliApp
{
    public class Bluetooth
    {
        private readonly Port port;
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public StringBuilder dataReceived { get; set; }
        public StringBuilder dataSend { get; set; }
        public StringBuilder dataToPlot { get; set; }
        private Thread readThread { get; set; }
        public bool IsPortOpen { get; set; }
        private static bool statusOfPlot { get; set; }
        public Bluetooth()
        {
            dataReceived = new StringBuilder();
            dataToPlot = new StringBuilder();
            dataSend = new StringBuilder();
            port = new Port();
        }
        public void UpdateConfiguration(string group, string value)
        {
            port.Update(group, value);
        }
        public bool Connect(PortConfiguration window)
        {
            bool isConfigurationOK = port.CreatePort();

            if (isConfigurationOK == true)
            {
                try
                {
                    port.serialPort.Open();
                    logger.Info("Port Open");

                    if (port.TestConnection() == true)
                    {
                        IsPortOpen = true;
                        dataReceived.Append("Start of transmission\n");

                        window.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            window.txtBox_receivedMessages.Text = dataReceived.ToString();
                        }));

                        logger.Info("Connected");
                        readThread = new Thread(() => Read());
                        readThread.Start();
                    }
                }
                catch(Exception exception)
                {
                    IsPortOpen = false;
                    port.serialPort.Close();
                    logger.Error("Problem with connection!");
                    logger.Error(exception.Message);
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
        public void SendData(string data)
        {
            if (IsPortOpen)
            {
                port.serialPort.Write(data);
                dataSend.Append($"{data}\n");
            }
        }
        public void Read()
        {
            while (IsPortOpen)
            {
                try
                {
                    string readData = ($"{port.serialPort.ReadTo("#")}\n");
                    sendDataToPlot(readData);
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
                SetStatusOfPlot(false);
                Plots.setData(dataToPlot);
                dataToPlot = new StringBuilder();
            }

        }
    }
}
