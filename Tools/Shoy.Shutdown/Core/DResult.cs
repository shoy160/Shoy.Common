using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Deyi.Shutdown.Core
{
    /// <summary> 基础数据结果类 </summary>
    [Serializable]
    [DataContract(Name = "result")]
    public class DResult
    {
        [DataMember(Name = "status", Order = 1)]
        public bool Status { get; set; }

        [DataMember(Name = "message", Order = 2)]
        public string Message { get; set; }

        public DResult(bool status, string message)
        {
            Status = status;
            Message = message;
        }

        public DResult(string message)
            : this(false, message)
        {
        }

        public static DResult Success
        {
            get { return new DResult(true, string.Empty); }
        }

        public static DResult Error(string message)
        {
            return new DResult(false, message);
        }

        public static DResult<T> Succ<T>(T data)
        {
            return new DResult<T>(true, data);
        }

        public static DResult<T> Error<T>(string message)
        {
            return new DResult<T>(message);
        }

        public static DResults<T> Succ<T>(IEnumerable<T> data, int count = -1)
        {
            return count < 0 ? new DResults<T>(data) : new DResults<T>(data, count);
        }

        public static DResults<T> Errors<T>(string message)
        {
            return new DResults<T>(message);
        }
    }

    [Serializable]
    [DataContract(Name = "result")]
    public class DResult<T> : DResult
    {
        [DataMember(Name = "data", Order = 3)]
        public T Data { get; set; }

        public DResult(bool status, T data)
            : base(status, string.Empty)
        {
            Data = data;
        }

        public DResult(string message)
            : base(false, message)
        {
        }
    }

    [Serializable]
    [DataContract(Name = "result")]
    public class DResults<T> : DResult
    {
        [DataMember(Name = "data", Order = 3)]
        public IEnumerable<T> Data { get; set; }

        [DataMember(Name = "count", Order = 4)]
        public int TotalCount { get; set; }

        public DResults(string message)
            : base(false, message)
        {
        }

        public DResults(IEnumerable<T> list)
            : base(true, string.Empty)
        {
            var data = list as T[] ?? list.ToArray();
            Data = data;
            TotalCount = data.Count();
        }

        public DResults(IEnumerable<T> list, int totalCount)
            : base(true, string.Empty)
        {
            Data = list;
            TotalCount = totalCount;
        }
    }
}