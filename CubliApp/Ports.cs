using System;
using System.IO.Ports;
using System.Windows;
using System.Diagnostics;
namespace CubliApp
{
    class Ports
    {
        public SerialPort serialPort;
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private int BoudRate { get; set; }
        private int DataBits { get; set; }
        private string Parity { get; set; }
        private string StopBits { get; set; }
        private string PortName { get; set; }

        public bool CreatePort()
        {
            bool isPortOK = false;


            if (PortName != null)
            {
                Parity Parity_ = (Parity)Enum.Parse(typeof(Parity), Parity, true);
                StopBits StopBits_ = (StopBits)Enum.Parse(typeof(StopBits), StopBits, true);
                int readTimeOut = 10000;
                int writeTimeOut = 10000;
                logger.Info($"Port Configuration:\nPortName: {PortName}\nBaudRate: {BoudRate}\nParity{Parity_.ToString()}\nDataBits: {DataBits}\nStopBits: {StopBits_.ToString()}\nReadTimeout: {readTimeOut}\nWriteTimeOut: {writeTimeOut}");

                serialPort = new SerialPort();

                serialPort.PortName = PortName;
                serialPort.BaudRate = BoudRate;
                serialPort.Parity = Parity_;
                serialPort.DataBits = DataBits;
                serialPort.StopBits = StopBits_;
                serialPort.ReadTimeout = readTimeOut;
                serialPort.WriteTimeout = writeTimeOut;
                serialPort.Handshake = Handshake.None;


                isPortOK = true;
                return isPortOK;
            }
            else
            {
                MessageBox.Show("Select a port", "Port problem", MessageBoxButton.OK, MessageBoxImage.Warning);
                return isPortOK;
            }
        }
        public void Update(string group, string value)
        {
            if (group == "BaudRate")
                BoudRate = int.Parse(value);
            else if (group == "DataBits")
                DataBits = int.Parse(value);
            else if (group == "Parity")
                Parity = value;
            else if (group == "StopBits")
            {
                if (value == "1,5")
                {
                    StopBits = "3"; // 1 -> ONE 2 -> TWO 3 -> ONE POINT FIVE (ENUM StopBits)
                }
                else
                    StopBits = value;
            }
            else if (group == "Port")
                PortName = value;
        }
        public void ClearBuffers()
        {
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }
        public string TestConnection()
        {
            ClearBuffers();
            serialPort.Write("1"); // 1->CONNECTION COMMAND
            return serialPort.ReadTo("#");
        }
        public bool TestDisconnect()
        {
            ClearBuffers();
            serialPort.Write("0"); // 0->DISCONNECTION COMMAND
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            TimeSpan ts = stopWatch.Elapsed;
            while (true)
            {
                float time = ts.Milliseconds;
                string frame = serialPort.ReadTo("#");
                logger.Debug(frame);
                if(frame.Contains("END"))
                {
                    return true;
                }
                if (time > 1000)
                    break;
            }
            return false;
        }
    }

}
