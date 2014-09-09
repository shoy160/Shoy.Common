using System;
using System.IO;
using System.Net.Sockets;

namespace Shoy.Laboratory.Transmiter
{
    /// <summary>
    /// 发送端
    /// 传输前发送端创建该类实例
    /// 设置必要属性后
    /// 调用Start()方法开始传输
    /// </summary>
    public class FileSender : FileTransmission
    {
        /// <summary>
        /// 接收到命令时发生
        /// </summary>
        public event CommandReceivedEventHandler CommandReceived;
        /// <summary>
        /// 开始异步接收
        /// </summary>
        internal override IAsyncResult BeginReceive()
        {
            InitializeReceiveBuf();
            try
            {
                return Socket.BeginReceive(ReceiveBuf, 0, ReceiveBuf.Length, SocketFlags.None, ReceiveCallback, null);
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
        /// <summary>
        /// 开始传输
        /// </summary>
        public override void Start()
        {
            base.Start();
            try
            {
                BeginReceive();
                FileStream = new FileStream(FullFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                TotalBlock = (int)(FileStream.Length / Consts.BlockSize) + 1;
                LastBlockSize = (int)(FileStream.Length - ((long)TotalBlock - 1) * Consts.BlockSize);
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }
        /// <summary>
        /// 获取估计剩余时间
        /// </summary>
        public override TimeSpan TimeRemaining
        {
            get
            {
                int blockRemaining = TotalBlock - FinishedBlock.Count;
                return TimeSpan.FromSeconds(blockRemaining / BlockAverSpeed);
            }
        }
        /// <summary>
        /// 获取已完成的数据长度
        /// </summary>
        public override long FinishedSize
        {
            get
            {
                return FinishedBlock.Count * (long)Consts.BlockSize;
            }
        }
        /// <summary>
        /// 同步发送区块
        /// </summary>
        /// <param name="blockIndex">区块序号</param>
        /// <returns>发送的数据长度</returns>
        public int SendBlock(int blockIndex)
        {
            try
            {
                int ret = Socket.EndSend(BeginSendBlock(blockIndex, null, null));
                OnBlockFinished(blockIndex);
                return ret;
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
        /// 异步发送区块并使用默认的回调方法
        /// </summary>
        /// <param name="blockIndex">区块序号</param>
        public void SendBlockAsync(int blockIndex)
        {
            BeginSendBlock(blockIndex, SendCallback, blockIndex);
        }

        /// <summary>
        /// 异步发送区块并使用指定的回调方法和参数
        /// </summary>
        /// <param name="blockIndex">区块序号</param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        public IAsyncResult BeginSendBlock(int blockIndex, AsyncCallback callback, object state)
        {
            try
            {
                if (!IsAlive)
                    throw new InvalidOperationException("Is Not Alive");
                if (blockIndex >= TotalBlock)
                    throw new ArgumentOutOfRangeException("blockIndex");
                byte[] toSend = Blocks[blockIndex].GetBytes();
                return Socket.BeginSend(toSend, 0, toSend.Length, SocketFlags.None, callback, state);
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
        internal void ReceiveCallback(IAsyncResult ar)
        {
            bool continueReceive = true;
            int count = 0;
            try
            {
                count = Socket.EndReceive(ar);
            }
            catch (SocketException)
            {
                OnConnectLost();
                return;
            }
            catch (Exception ex)
            {
                try
                {
                    OnErrorOccurred(ex);
                }
                catch { return; }
            }
            try
            {
                if (count == 0)
                    return;
                switch (ReceiveBuf[0])
                {
                    case Consts.StringHeader:
                        continueReceive = OnCommandReceived(ReceiveBuf.ToFtString());
                        break;
                    default:
                        throw new FormatException("Bad Header!");
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
            if (continueReceive)
            {
                BeginReceive();
            }
        }
        /// <summary>
        /// 命令处理
        /// </summary>
        /// <param name="str">收到的命令</param>
        /// <returns>是否继续接收</returns>
        internal bool OnCommandReceived(string str)
        {
            if (CommandReceived != null)
                CommandReceived(this, new CommandReceivedEventArgs(str));
            bool continueReceive = true;
            string[] msg = str.Split(' ');
            if (msg[0] == "Exit")
            {
                OnAllFinished();
                continueReceive = false;
                Stop();
            }
            else if (msg[0] == "GET")
            {
                if (msg[1] == "FileBlock")
                {
                    int blockIndex;
                    if (!int.TryParse(msg[2], out blockIndex))
                        throw new FormatException("Bad BlockIndex " + msg[2]);
                    SendBlock(blockIndex);
                }
                else if (msg[1] == "BlockHash")
                {
                    int blockIndex;
                    if (!int.TryParse(msg[2], out blockIndex))
                        throw new FormatException("Bad BlockIndex " + msg[2]);
                    byte[] hash = Blocks[blockIndex].DataHash;
                    SendStringAsync(string.Format("BlockHash {0} {1}", blockIndex, BitConverter.ToInt32(hash, 0)));
                }
                else if (msg[1] == "FileName")
                {
                    SendStringAsync(string.Format("SET FileName {0}", FileName.DoReplace()));
                }
                else if (msg[1] == "TotalBlock")
                {
                    SendStringAsync(string.Format("SET TotalBlock {0}", TotalBlock));
                }
                else if (msg[1] == "LastBlockSize")
                {
                    SendStringAsync(string.Format("SET LastBlockSize {0}", LastBlockSize));
                }
                else
                    throw new FormatException("Bad Command " + msg[1]);
            }
            else
                throw new FormatException("Bad Command " + msg[0]);

            return continueReceive;
        }
    }
}
