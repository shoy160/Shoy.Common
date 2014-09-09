using System;
using System.Collections.Generic;
using System.IO;

namespace Shoy.Laboratory.Transmiter
{
    /// <summary>
    /// 文件区块的抽象集合
    /// 之所以说抽象是因为该集合并不存储实际的区块(缓存区除外)
    /// 而是通过一个索引器来读写文件
    /// 并提供磁盘缓存
    /// </summary>
    public class FileBlockCollection
    {
        /// <summary>
        /// 与该区块关联的传输对象
        /// </summary>
        internal FileTransmission Task;

        /// <summary>
        /// 与该区块关联的FileStream
        /// </summary>
        internal FileStream FileStream;

        private bool _enabledIoBuffer;

        /// <summary>
        /// 磁盘缓存区
        /// </summary>
        internal Dictionary<int, FileBlock> IoBuffer;

        public FileBlockCollection(FileTransmission transmissionTask)
        {
            Task = transmissionTask;
            FileStream = Task.FileStream;
            IoBufferSize = Consts.DefaultIoBufferSize;
        }

        /// <summary>
        /// 获取或设置一个值,该值指示是否启用磁盘缓存
        /// </summary>
        internal bool EnabledIoBuffer
        {
            get { return _enabledIoBuffer; }
            set
            {
                _enabledIoBuffer = value;
                if (value)
                    IoBuffer = new Dictionary<int, FileBlock>();
                else
                {
                    if (Task is FileReceiver)
                        WriteAllBlock();
                    IoBuffer = null;
                }
            }
        }

        internal int IoBufferSize;

        /// <summary>
        /// 获取已接收或已发送的区块序号列表
        /// </summary>
        public List<int> Finished
        {
            get { return Task.FinishedBlock; }
        }

        /// <summary>
        /// 获取已存在(Hash成功)的区块序号列表
        /// </summary>
        public List<int> Exist
        {
            get
            {
                var receiver = Task as FileReceiver;
                return (receiver == null ? null : receiver.ExistBlock);
            }
        }

        /// <summary>
        /// 获取被丢弃的区块序号列表
        /// </summary>
        public List<int> Cast
        {
            get
            {
                var receiver = Task as FileReceiver;
                return (receiver == null ? null : receiver.CastBlock);
            }
        }

        /// <summary>
        /// 获取总区块数
        /// </summary>
        public int Count
        {
            get { return Task.TotalBlock; }
        }

        /// <summary>
        /// 获取有效区块数(已存在+已接收)
        /// </summary>
        public int CountValid
        {
            get
            {
                var receiver = Task as FileReceiver;
                return Task.FinishedBlock.Count + (receiver == null ? 0 : receiver.ExistBlock.Count);
            }
        }

        /// <summary>
        /// 将缓存中的区块全部写入磁盘
        /// </summary>
        /// <returns>写入的区块数量</returns>
        public int WriteAllBlock()
        {
            if (!EnabledIoBuffer)
                return -1;
            int count = 0;
            lock (IoBuffer)
            {
                foreach (var b in IoBuffer)
                {
                    b.Value.Write();
                    count++;
                }
                if (count != IoBuffer.Count)
                    throw new IOException("Can not Write All FileBlocks!");
                IoBuffer.Clear();
            }
            return count;
        }

        /// <summary>
        /// 读取数据以填充缓存
        /// </summary>
        /// <param name="startIndex">起始区块</param>
        /// <returns>读取的区块数量</returns>
        public int FillIoBuffer(int startIndex)
        {
            int index;
            lock (IoBuffer)
            {
                IoBuffer.Clear();
                for (index = startIndex; IoBuffer.Count < IoBufferSize && index < Task.Blocks.Count; index++)
                {
                    IoBuffer.Add(index, new FileBlock(Task, index, true));
                }
            }
            return index - startIndex;
        }

        /// <summary>
        /// 异步填充缓存
        /// </summary>
        /// <param name="startIndex">起始区块</param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        public IAsyncResult BeginFillIoBuffer(int startIndex, AsyncCallback callback, object state)
        {
            return new DelegateIntInt(FillIoBuffer).BeginInvoke(startIndex, callback, state);
        }

        /// <summary>
        /// 写入区块
        /// </summary>
        /// <param name="value">区块对象</param>
        public void Write(FileBlock value)
        {
            if (EnabledIoBuffer)
            {
                if (IoBuffer.Count >= IoBufferSize)
                    WriteAllBlock();
                lock (IoBuffer)
                    IoBuffer.Add(value.Index, value);
            }
            else
                value.Write();
        }

        /// <summary>
        /// 读取或写入区块
        /// </summary>
        /// <param name="blockIndex">区块序号</param>
        public FileBlock this[int blockIndex]
        {
            get
            {
                FileBlock output;
                if (EnabledIoBuffer)
                {

                    bool isInBuf;
                    lock (IoBuffer)
                        isInBuf = IoBuffer.TryGetValue(blockIndex, out output);
                    if (isInBuf)
                        return output;
                    output = new FileBlock(Task, blockIndex, true);
                    BeginFillIoBuffer(blockIndex + 1, null, null);
                }
                else
                    output = new FileBlock(Task, blockIndex, true);
                return output;
            }
            set
            {
                if (blockIndex != value.Index)
                    throw new FileBlockException("Bad Index!", FileBlockException.ErrorCode.BadIndex);
                Write(value);
            }
        }
    }
}
