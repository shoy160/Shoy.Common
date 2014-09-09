using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Shoy.Laboratory.Transmiter
{
    public delegate void BlockFinishedEventHandler(object sender, BlockFinishedEventArgs e);

    public delegate void CommandReceivedEventHandler(object sender, CommandReceivedEventArgs e);

    public delegate void FileTransmissionErrorOccurEventHandler(object sender, FileTransmissionErrorOccurEventArgs e);

    public delegate void DelegateSendBlocks(int start, int end);

    public delegate void DelegateVoidBool(bool logic);

    public delegate int DelegateIntInt(int value);

    /// <summary>
    /// 文件传输基类
    /// </summary>
    public abstract class FileTransmission : IDisposable
    {
        private FileStream _fileStream;

        //internal readonly TransmissionMode _Mode;
        /// <summary>
        /// 总区块数
        /// </summary>
        private int _totalBlock;
        /// <summary>
        /// 最后一个区块的大小
        /// </summary>
        internal int LastBlockSize;
        internal List<int> FinishedBlock;
        internal byte[] ReceiveBuf;
        private Socket _socket;
        internal EventWaitHandle WaitHandle;
        private bool _isAlive;
        private readonly FileBlockCollection _blocks;
        private DateTime _startTime;

        /// <summary>
        /// 上一个区块完成的时间
        /// </summary>
        private DateTime _priorBlockTime;
        private double _byteSpeed;

        /// <summary>
        /// 获取或设置一个值,该值指示是否启用磁盘缓存
        /// </summary>
        public bool EnabledIoBuffer
        {
            get { return _blocks.EnabledIoBuffer; }
            set { _blocks.EnabledIoBuffer = value; }
        }

        /// <summary>
        /// 获取或设置磁盘缓存的大小(单位:区块数)
        /// </summary>
        public int IoBufferSize
        {
            get { return _blocks.IoBufferSize; }
            set
            {
                if (!_blocks.EnabledIoBuffer)
                    throw new InvalidOperationException("IOBuffer is not enabled!");
                _blocks.IoBufferSize = value;
            }
        }
        /// <summary>
        /// 获取当前磁盘缓存中的区块数
        /// </summary>
        public int CurrentIoBufferSize
        {
            get
            {
                return !_blocks.EnabledIoBuffer ? 0 : _blocks.IoBuffer.Count;
            }
        }
        /// <summary>
        /// 获取或设置该传输的目标连接
        /// </summary>
        public Socket Socket
        {
            get { return _socket; }
            set
            {
                try
                {
                    if (value.ProtocolType != ProtocolType.Tcp)
                        throw new ArgumentException("Socket Protocol must be TCP", "Socket");
                    _socket = value;
                    _socket.ReceiveBufferSize = _socket.SendBufferSize = Consts.NetBlockMaxSize;
                }
                catch (Exception ex)
                {
                    OnErrorOccurred(ex);
                }
            }
        }

        /// <summary>
        /// 获取与此传输关联的文件流
        /// </summary>
        public FileStream FileStream
        {
            get { return _fileStream; }
            internal set { _fileStream = value; }
        }

        /// <summary>
        /// 获取或设置文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 获取或设置文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 获取或设置文件名(包括路径)
        /// </summary>
        public string FullFileName
        {
            get
            {
                try
                {
                    return FilePath.TrimEnd('\\') + "\\" + FileName;
                }
                catch (Exception ex)
                {
                    OnErrorOccurred(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    int i = value.LastIndexOf('\\');
                    FilePath = i > 0 ? value.Substring(0, i) : Environment.CurrentDirectory;
                    FileName = value.Substring(i + 1);
                }
                catch (Exception ex)
                {
                    OnErrorOccurred(ex);
                }
            }
        }
        /// <summary>
        /// 一个区块完成时发生
        /// </summary>
        public event BlockFinishedEventHandler BlockFinished;
        /// <summary>
        /// 全部完成时发生
        /// </summary>
        public event EventHandler AllFinished;
        /// <summary>
        /// 连接中断时发生
        /// </summary>
        public event EventHandler ConnectLost;
        /// <summary>
        /// 出现错误时发生
        /// </summary>
        public event FileTransmissionErrorOccurEventHandler ErrorOccurred;
        /// <summary>
        /// 获取一个值,该值指示传输是否正在进行
        /// </summary>
        public bool IsAlive { get { return _isAlive; } }

        /// <summary>
        /// 获取传输开始的时间
        /// </summary>
        public DateTime StartTime
        {
            get { return _startTime; }
            internal set { _startTime = value; }
        }

        /// <summary>
        /// 获取已用时
        /// </summary>
        public TimeSpan TimePast { get { return DateTime.Now - _startTime; } }
        /// <summary>
        /// 获取估计剩余时间
        /// </summary>
        public abstract TimeSpan TimeRemaining { get; }
        /// <summary>
        /// 获取平均速率(区块/秒)
        /// </summary>
        public double BlockAverSpeed
        {
            get
            {
                return FinishedBlock.Count / TimePast.TotalSeconds;
            }
        }
        /// <summary>
        /// 获取平均速率(字节/秒)
        /// </summary>
        public double ByteAverSpeed
        {
            get
            {
                return BlockAverSpeed * Consts.BlockSize;
            }
        }
        /// <summary>
        /// 获取平均速率(千字节/秒)
        /// </summary>
        public double KByteAverSpeed
        {
            get
            {
                return ByteAverSpeed / 1024;
            }
        }
        /// <summary>
        /// 获取瞬时速率(字节/秒)
        /// </summary>
        public double ByteSpeed
        {
            get
            {
                return _byteSpeed;
            }
        }
        /// <summary>
        /// 获取瞬时速率(千字节/秒)
        /// </summary>
        public double KByteSpeed
        {
            get
            {
                return _byteSpeed / 1024;
            }
        }

        internal int TotalBlock
        {
            get { return _totalBlock; }
            set { _totalBlock = value; }
        }

        /// <summary>
        /// 获取文件总长度
        /// </summary>
        public long TotalSize
        {
            get
            {
                return (_totalBlock - 1) * (long)Consts.BlockSize + LastBlockSize;
            }
        }
        /// <summary>
        /// 获取已完成的数据长度
        /// </summary>
        public abstract long FinishedSize { get; }
        /// <summary>
        /// 获取进度值(%)
        /// </summary>
        public double Progress
        {
            get
            {
                return (FinishedSize / (double)TotalSize) * 100;
            }
        }
        /// <summary>
        /// 获取该传输的区块集合
        /// </summary>
        public FileBlockCollection Blocks { get { return _blocks; } }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public FileTransmission()
        {
            FinishedBlock = new List<int>();
            WaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            _blocks = new FileBlockCollection(this);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名</param>
        public FileTransmission(string filePath, string fileName)
        {
            FinishedBlock = new List<int>();
            WaitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
            _blocks = new FileBlockCollection(this);

            FilePath = filePath;
            FileName = fileName;
        }
        /// <summary>
        /// 初始化接收缓存
        /// </summary>
        internal void InitializeReceiveBuf()
        {
            try
            {
                ReceiveBuf = new byte[_socket.ReceiveBufferSize];
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }
        /// <summary>
        /// 开始异步接收
        /// </summary>
        internal abstract IAsyncResult BeginReceive();
        /// <summary>
        /// 开始传输
        /// </summary>
        public virtual void Start()
        {
            try
            {
                _isAlive = true;
                _startTime = DateTime.Now;
                WaitHandle.Reset();
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }
        /// <summary>
        /// 中止传输
        /// </summary>
        /// <param name="shutDownSocket">是否关闭Socket</param>
        public virtual void Stop(bool shutDownSocket)
        {
            try
            {
                _isAlive = false;
                _fileStream.Close();
                _fileStream = null;
                WaitHandle.Set();
                if (shutDownSocket)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        /// <summary>
        /// 异步中止传输,不关闭Socket
        /// </summary>
        internal void Stop()
        {
            new DelegateVoidBool(Stop).BeginInvoke(false, null, null);
        }

        /// <summary>
        /// 等待传输完成
        /// </summary>
        public bool WaitForExit()
        {
            return WaitHandle.WaitOne();
        }
        /// <summary>
        /// 等待传输完成
        /// </summary>
        public bool WaitForExit(int millisecondsTimeout, bool exitContext)
        {
            return WaitHandle.WaitOne(millisecondsTimeout, exitContext);
        }
        /// <summary>
        /// 等待传输完成
        /// </summary>
        public bool WaitForExit(TimeSpan timeout, bool exitContext)
        {
            return WaitHandle.WaitOne(timeout, exitContext);
        }
        internal virtual void OnBlockFinished(int blockIndex)
        {
            if (!FinishedBlock.Exists(a => a == blockIndex))
                FinishedBlock.Add(blockIndex);
            if (blockIndex == _totalBlock - 1)
                _byteSpeed = LastBlockSize / (DateTime.Now - _priorBlockTime).TotalSeconds;
            else
                _byteSpeed = Consts.BlockSize / (DateTime.Now - _priorBlockTime).TotalSeconds;
            _priorBlockTime = DateTime.Now;
            if (BlockFinished != null)
                BlockFinished(this, new BlockFinishedEventArgs(blockIndex));
        }
        internal virtual void OnConnectLost()
        {
            if (!_isAlive)
                return;
            Stop();
            if (ConnectLost != null)
                ConnectLost(this, new EventArgs());
        }
        /// <summary>
        /// 同步发送字符串
        /// </summary>
        public int SendString(string str)
        {
            try
            {
                return _socket.EndSend(BeginSendString(str, null, null));
            }
            catch (SocketException)
            {
                OnConnectLost();
                return 0;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return 0;
            }
        }
        /// <summary>
        /// 异步发送字符串并使用默认的回调方法
        /// </summary>
        public void SendStringAsync(string str)
        {
            BeginSendString(str, SendCallback, null);
        }
        /// <summary>
        /// 异步发送字符串并使用指定的的回调方法和参数
        /// </summary>
        public IAsyncResult BeginSendString(string str, AsyncCallback callback, object state)
        {
            try
            {
                if (!_isAlive)
                    throw new InvalidOperationException("Is Not Alive");
                byte[] toSend = str.ToBytes();
                return _socket.BeginSend(toSend, 0, toSend.Length, SocketFlags.None, callback, state);
            }
            catch (SocketException)
            {
                OnConnectLost();
                return null;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return null;
            }
        }
        internal void SendCallback(IAsyncResult ar)
        {
            try
            {
                _socket.EndSend(ar);
            }
            catch (SocketException)
            {
                OnConnectLost();
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
            if (ar.AsyncState != null)
            {
                if (ar.AsyncState is int)
                {
                    OnBlockFinished((int)ar.AsyncState);
                }
            }
        }
        internal virtual void OnAllFinished()
        {
            if (AllFinished != null)
                AllFinished(this, new EventArgs());
        }
        internal virtual void OnErrorOccurred(Exception innerException)
        {
            var eventargs = new FileTransmissionErrorOccurEventArgs(innerException);
            if (ErrorOccurred != null)
                ErrorOccurred(this, eventargs);
            if (!eventargs.Continue)
                throw innerException;
        }
        void IDisposable.Dispose()
        {
            _fileStream.Close();
            _socket.Close();
        }
    }
}
