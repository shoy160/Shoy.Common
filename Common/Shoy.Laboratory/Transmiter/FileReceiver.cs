using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Shoy.Laboratory.Transmiter
{
    /// <summary>
    /// 接收端
    /// 传输前接收端创建该类实例
    /// 设置必要属性后
    /// 调用Start()方法开始传输
    /// </summary>
    public class FileReceiver : FileTransmission
    {
        internal List<int> ExistBlock;
        internal List<int> CastBlock;

        /// <summary>
        /// 下载线程
        /// </summary>
        internal Thread DownThread;

        public event BlockFinishedEventHandler BlockHashed;

        /// <summary>
        /// 开始异步接收
        /// </summary>
        internal override IAsyncResult BeginReceive()
        {
            InitializeReceiveBuf();
            try
            {
                return Socket.BeginReceive(ReceiveBuf, 0, ReceiveBuf.Length, SocketFlags.None, null, null);
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
        /// 获取估计剩余时间
        /// </summary>
        public override TimeSpan TimeRemaining
        {
            get
            {
                int blockRemaining = TotalBlock - FinishedBlock.Count - ExistBlock.Count;
                return TimeSpan.FromSeconds(blockRemaining/BlockAverSpeed);
            }
        }

        /// <summary>
        /// 获取已完成的数据长度
        /// </summary>
        public override long FinishedSize
        {
            get { return (FinishedBlock.Count + (long) ExistBlock.Count - 1)*Consts.BlockSize + LastBlockSize; }
        }

        /// <summary>
        /// 开始传输
        /// </summary>
        public override void Start()
        {
            base.Start();
            try
            {
                CastBlock = new List<int>();
                ExistBlock = new List<int>();
                DownThread = new Thread(DownLoad) {IsBackground = true, Name = "DownThread"};
                DownThread.Start();
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
        public override void Stop(bool shutDownSocket)
        {
            try
            {
                if (DownThread != null)
                {
                    if ((DownThread.ThreadState & ThreadState.Running) == ThreadState.Running)
                        DownThread.Abort();
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
            base.Stop(shutDownSocket);
        }

        internal string ReceiveString()
        {
            int count;
            try
            {
                count = Socket.EndReceive(BeginReceive());
            }
            catch (Exception)
            {
                OnConnectLost();
                throw;
            }
            if (count == 0)
                return null;
            return ReceiveBuf.ToFtString();
        }

        internal FileBlock ReceiveFileBlock()
        {
            var mStream = new MemoryStream();
            while (true)
            {
                int count;
                try
                {
                    count = Socket.EndReceive(BeginReceive());
                    if (count == 0) throw new Exception();
                }
                catch (Exception)
                {
                    OnConnectLost();
                    throw;
                }
                mStream.Write(ReceiveBuf, 0, count);
                try
                {
//接收到正确的区块则返回
                    return new FileBlock(this, mStream.ToArray());
                }
                catch (FileBlockException)
                {
//接收到不完整或错误的区块,若不完整则继续接收
                    if (mStream.Length >= Consts.NetBlockMaxSize)
                        throw; //区块已达到指定大小但仍然错误,则抛出错误
                }
            }
        }

        /// <summary>
        /// 从发送端获取文件名
        /// </summary>
        public void GetFileName()
        {
            while (true)
            {
                SendString("GET FileName");
                string[] msg = ReceiveString().Split(' ');
                if (msg[0] == "SET" && msg[1] == "FileName")
                {
                    FileName = msg[2];
                    break;
                }
            }
        }

        /// <summary>
        /// 从发送端获取区块总数
        /// </summary>
        public void GetTotalBlock()
        {
            while (true)
            {
                SendString("GET TotalBlock");
                string[] msg = ReceiveString().Split(' ');
                if (msg[0] == "SET" && msg[1] == "TotalBlock")
                {
                    int totalBlock;
                    if (int.TryParse(msg[2], out totalBlock))
                    {
                        TotalBlock = totalBlock;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 从发送端获取最后一个区块的大小
        /// </summary>
        public void GetLastBlockSize()
        {
            while (true)
            {
                SendString("GET LastBlockSize");
                string[] msg = ReceiveString().Split(' ');
                if (msg[0] == "SET" && msg[1] == "LastBlockSize")
                {
                    if (int.TryParse(msg[2], out LastBlockSize))
                        break;
                }
            }
        }

        /// <summary>
        /// 校验文件
        /// </summary>
        /// <returns>损坏或尚未下载的区块序号列表</returns>
        public List<int> HashFile()
        {
            FileStream.Position = 0;
            ExistBlock.Clear();
            for (int count = 0; FileStream.Position < FileStream.Length && count < TotalBlock; count++)
            {
//校验已存在的区块
                var testBlock = new FileBlock(this, count, true);
                SendString(string.Format("GET BlockHash {0}", count));
                string[] msg = ReceiveString().Split(' ');
                if (msg[0] == "BlockHash")
                {
                    if (Convert.ToInt32(msg[1]) == count)
                    {
                        if (BitConverter.ToInt32(testBlock.DataHash, 0) == Convert.ToInt32(msg[2]))
                            ExistBlock.Add(count);
                    }
                }
                if (BlockHashed != null)
                    BlockHashed(this, new BlockFinishedEventArgs(count));
            }
            int maxExistBlockIndex; //已存在的区块最大序号
            try
            {
                maxExistBlockIndex = ExistBlock.Max();
            }
            catch
            {
                maxExistBlockIndex = 0;
            }
            var blockRemaining = new List<int>();
            for (int index = 0; index < TotalBlock;)
            {
//计算仍需传输的区块
                if (index <= maxExistBlockIndex)
                {
                    if (ExistBlock.Exists(a => a == index))
                    {
                        index++;
                        continue;
                    }
                }
                blockRemaining.Add(index++);
            }
            return blockRemaining;
        }

        /// <summary>
        /// 接收整个文件
        /// </summary>
        internal void DownLoad()
        {
            try
            {
                if (string.IsNullOrEmpty(FilePath)) //未指定路径时默认为接收程序所在路径
                    FilePath = Environment.CurrentDirectory;
                if (string.IsNullOrEmpty(FileName)) //未指定文件名时从发送端获取
                {
                    GetFileName();
                }
                FileStream = new FileStream(FullFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                    //temp
                GetTotalBlock();
                GetLastBlockSize();
                List<int> blockRemaining = HashFile();
                if (FileStream.Length > TotalSize) //如果已存在的文件比目标文件长则截断它
                    FileStream.SetLength(TotalSize);
                StartTime = DateTime.Now;
                foreach (int index in blockRemaining)
                {
                    FileBlock block;
                    while (true)
                    {
                        SendString(string.Format("GET FileBlock {0}", index));
                        try
                        {
                            block = ReceiveFileBlock();
                            break;
                        }
                        catch (FileBlockException)
                        {
//接收到错误的区块,抛弃该数据并重新请求
                            CastBlock.Add(index);
                        }
                        catch (Exception ex)
                        {
                            OnErrorOccurred(ex);
                        }
                    }
                    while (true)
                    {
                        try
                        {
                            Blocks[index] = block; //写入区块
                            OnBlockFinished(index);
                            break;
                        }
                        catch (IOException ex)
                        {
//磁盘写入错误时
                            try
                            {
                                OnErrorOccurred(ex);
                                //重试
                            }
                            catch
                            {
//退出
                                Stop();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            OnErrorOccurred(ex);
                        }
                    }
                }
                SendStringAsync("Exit");
                Blocks.WriteAllBlock();
                OnAllFinished();
                Stop();
            }
            catch (SocketException)
            {
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }
    }
}
