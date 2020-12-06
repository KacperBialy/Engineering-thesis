using System;
using System.IO.Ports;
using System.Windows;

namespace CubliApp
{
    class Ports
    {
        public SerialPort serialPort;

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
                serialPort = new SerialPort();

                serialPort.PortName = PortName;
                serialPort.BaudRate = BoudRate;
                serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), Parity, true);
                serialPort.DataBits = DataBits;
                serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), StopBits, true);
                serialPort.ReadTimeout = 500;
                serialPort.WriteTimeout = 500;

                isPortOK = true;
                return isPortOK;
            }
            else
            {
                MessageBox.Show("Select a port", "Port problem", MessageBoxButton.OK, MessageBoxImage.Warning);
                return isPortOK;
            }
        }
        public void  Update(string group,string value)
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
        public string TestDisconnect()
        {
            ClearBuffers();
            serialPort.Write("0"); // 0->DISCONNECTION COMMAND
            return serialPort.ReadTo("#");
        }
    }

}
