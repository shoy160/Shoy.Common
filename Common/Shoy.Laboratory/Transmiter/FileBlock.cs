using System;
using System.IO;

namespace Shoy.Laboratory.Transmiter
{
    /// <summary>
    /// 文件区块类
    /// </summary>
    public class FileBlock : IComparable<FileBlock>
    {
        /// <summary>
        /// 与该区块关联的FileStream
        /// </summary>
        private readonly FileStream _fileStream;
        /// <summary>
        /// 文件数据
        /// </summary>
        private byte[] _data;
        /// <summary>
        /// 数据长度
        /// </summary>
        private int _dataLength;
        /// <summary>
        /// 数据的Hash值
        /// </summary>
        private byte[] _dataHash;

        /// <summary>
        /// 获取或设置该区块的序号(该区块在文件中的位置)
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 获取该区块的数据长度
        /// </summary>
        public int DataLength { get { return _dataLength; } }
        /// <summary>
        /// 获取该数据块的校验值
        /// </summary>
        public byte[] DataHash { get { return _dataHash; } }

        /// <summary>
        /// 构造函数
        /// 用于从文件读入区块
        /// </summary>
        /// <param name="transmissionTask"></param>
        /// <param name="blockIndex">分块位置</param>
        /// <param name="readOnCreated">是否立即从文件读取数据</param>
        public FileBlock(FileTransmission transmissionTask, int blockIndex, bool readOnCreated)
        {
            _fileStream = transmissionTask.FileStream;
            Index = blockIndex;
            if (readOnCreated)
                Read(true);
        }

        /// <summary>
        /// 构造函数
        /// 用于从二进制数据读入区块
        /// </summary>
        /// <param name="transmissionTask"></param>
        /// <param name="receivedData">输入的二进制数据</param>
        public FileBlock(FileTransmission transmissionTask, byte[] receivedData)
        {
            _fileStream = transmissionTask.FileStream;
            if (receivedData[0] != Consts.FileBlockHeader)
                throw new FileBlockException("Bad Header!", FileBlockException.ErrorCode.BadHeader);
            Index = BitConverter.ToInt32(receivedData, 1);
            _dataLength = receivedData.Length - 9;
            if (_dataLength > Consts.BlockSize)
                throw new FileBlockException("Illegal FileBlock Size!", FileBlockException.ErrorCode.IllegalFileBlockSize);
            _data = new byte[_dataLength];
            _dataHash = new byte[4];
            Array.Copy(receivedData, 5, _dataHash, 0, 4);
            Array.Copy(receivedData, 9, _data, 0, _dataLength);
            if (!_dataHash.BytesEqual(_data.GetHash()))
                throw new FileBlockException("Error Hash!", FileBlockException.ErrorCode.ChecksumError);
        }
        /// <summary>
        /// 从文件读入
        /// </summary>
        /// <param name="calcHashAfterRead">是否在读取后立即计算校验值</param>
        /// <returns>读取块的大小</returns>
        public int Read(bool calcHashAfterRead)
        {
            _data = new byte[Consts.BlockSize];
            lock (_fileStream)
            {
                _fileStream.Position = Index * (long)Consts.BlockSize;
                _dataLength = _fileStream.Read(_data, 0, Consts.BlockSize);
            }
            if (_data.Length != _dataLength)
            {
                byte[] old = _data;
                _data = new byte[_dataLength];
                Array.Copy(old, _data, _dataLength);
            }
            if (calcHashAfterRead)
                CalcHash();
            return _dataLength;
        }
        /// <summary>
        /// 计算校验值
        /// </summary>
        /// <returns>校验值</returns>
        public byte[] CalcHash()
        {
            return _dataHash = _data.GetHash();
        }
        /// <summary>
        /// 将该区块写入文件
        /// </summary>
        public void Write()
        {
            lock (_fileStream)
            {
                _fileStream.Position = Index * (long)Consts.BlockSize;
                _fileStream.Write(_data, 0, _dataLength);
            }
        }
        /// <summary>
        /// 转化为二进制数据以传输
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            var mStream = new MemoryStream(1 + 4 + 4 + _dataLength);
            var header = new[] {Consts.FileBlockHeader};
            mStream.Write(header, 0, 1);
            mStream.Write(BitConverter.GetBytes(Index), 0, 4);
            mStream.Write(_dataHash, 0, 4);
            mStream.Write(_data, 0, _dataLength);
            return mStream.ToArray();
        }
        int IComparable<FileBlock>.CompareTo(FileBlock obj)
        {
            return (Index as IComparable<int>).CompareTo(obj.Index);
        }
    }
}
