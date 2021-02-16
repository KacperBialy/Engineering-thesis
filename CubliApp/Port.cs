using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;
namespace CubliApp
{
    class Port
    {
        public SerialPort serialPort { get; set; }
        private int BoudRate { get; set; }
        private int DataBits { get; set; }
        private Parity _Parity { get; set; }
        private StopBits _StopBits { get; set; }
        private string PortName { get; set; }

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public bool CreatePort()
        {
            bool isPortOK = false;

            if (PortName != null)
            {
                int readTimeOut = 10000;
                int writeTimeOut = 10000;

                logger.Info($"Port Configuration:\nPortName: {PortName}\nBaudRate: {BoudRate}\nParity{_Parity}\nDataBits: {DataBits}\nStopBits: {_StopBits}\nReadTimeout: {readTimeOut}\nWriteTimeOut: {writeTimeOut}");

                serialPort = new SerialPort();

                serialPort.PortName = PortName;
                serialPort.BaudRate = BoudRate;
                serialPort.Parity = _Parity;
                serialPort.DataBits = DataBits;
                serialPort.StopBits = _StopBits;
                serialPort.ReadTimeout = readTimeOut;
                serialPort.WriteTimeout = writeTimeOut;

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
                _Parity = (Parity)Enum.Parse(typeof(Parity), value, true);
            else if (group == "StopBits")
            {
                if (value == "1")
                    _StopBits = StopBits.One;
                else if (value == "1.5")
                    _StopBits = StopBits.OnePointFive;
                else if (value == "2")
                    _StopBits = StopBits.Two;
            }
            else if (group == "Port")
                PortName = value;
        }
        public void ClearBuffers()
        {
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }
        public bool TestConnection()
        {
            ClearBuffers();
            serialPort.Write("1"); // 1 -> CONNECTION COMMAND
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            TimeSpan ts;
            float time;
            string frame;
            while (true)
            {
                ts = stopWatch.Elapsed;
                ts.
                time = ts.Seconds;
                frame = serialPort.ReadTo("#");
                logger.Debug($"Connection frame: {frame}");
                if (frame.Contains("OK"))
                {
                    return true;
                }
                if (time > 0)
                    break;
            }
            return false;
        }
        public bool TestDisconnect()
        {
            ClearBuffers();
            serialPort.Write("0"); // 0 -> DISCONNECTION COMMAND
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true)
            {
                TimeSpan ts = stopWatch.Elapsed;
                float time = ts.Seconds;
                string frame = serialPort.ReadTo("#");
                logger.Debug($"Disconnection frame: {frame}");
                if (frame.Contains("END"))
                {
                    return true;
                }
                if (time > 0)
                    break;
            }
            return false;
        }
    }

}
