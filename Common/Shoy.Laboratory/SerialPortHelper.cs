using System;
using System.IO.Ports;
using System.Threading;

namespace Shoy.Laboratory
{
    /// <summary> 串口通讯 </summary>
    public class SerialPortHelper
    {
        private readonly SerialPort _serialPort;
        /// <summary> 数据接收事件 </summary>
        public event ReceivedEventHandle Received;
        /// <summary> 错误事件 </summary>
        public event SerialErrorReceivedEventHandler Error;

        public static string[] Ports => SerialPort.GetPortNames();

        /// <summary> 串口通讯 </summary>
        /// <param name="portName">串口名称</param>
        /// <param name="baudrate">波特率:9600</param>
        /// <param name="parity">奇偶校验:n</param>
        /// <param name="dataBits">数据位:8</param>
        /// <param name="stopBits">停止位:1</param>
        public SerialPortHelper(string portName, int baudrate = 9600, Parity parity = Parity.None, int dataBits = 8,
            StopBits stopBits = StopBits.One)
        {
            _serialPort = new SerialPort(portName, baudrate, parity, dataBits, stopBits)
            {
                RtsEnable = true,
                ReadTimeout = 3000
            };
            _serialPort.DataReceived += DataReceivedEvent;
            _serialPort.ErrorReceived += ErrorReceivedEvent;
        }

        ~SerialPortHelper()
        {
            Close();
        }

        /// <summary> 开启串口 </summary>
        public void Open()
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }

        /// <summary> 关闭串口 </summary>
        public void Close()
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();
        }

        /// <summary> 发送数据 </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            Send(data, 0, data.Length);
        }

        /// <summary> 发送数据 </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Send(byte[] data, int offset, int count)
        {
            if (!_serialPort.IsOpen) return;
            //清空接收缓冲区
            _serialPort.DiscardInBuffer();
            _serialPort.Write(data, offset, count);
        }

        /// <summary> 发送命令 </summary>
        /// <param name="data"></param>
        /// <param name="receiveData"></param>
        /// <returns></returns>
        public int SendCommand(byte[] data, ref byte[] receiveData)
        {
            if (!_serialPort.IsOpen)
                return -1;
            //清空接收缓冲区
            _serialPort.DiscardInBuffer();
            _serialPort.Write(data, 0, data.Length);
            while (_serialPort.BytesToRead < receiveData.Length)
            {
                Thread.Sleep(1);
            }
            //_serialPort.byte
            return _serialPort.Read(receiveData, 0, receiveData.Length);
        }

        private void ErrorReceivedEvent(object sender, SerialErrorReceivedEventArgs e)
        {
            Error?.Invoke(sender, e);
        }

        private void DataReceivedEvent(object sender, SerialDataReceivedEventArgs e)
        {
            if (Received == null) return;
            var data = new byte[_serialPort.BytesToRead];
            _serialPort.Read(data, 0, data.Length);
            Received?.Invoke(sender, new RecivedEventArgs(data));
        }
        public delegate void ReceivedEventHandle(object sender, RecivedEventArgs e);
    }
    public class RecivedEventArgs : EventArgs
    {
        public RecivedEventArgs(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; set; }
    }
}
