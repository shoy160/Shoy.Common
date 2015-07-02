using Shoy.Core.Context;
using Shoy.Core.Data;
using Shoy.Utility;
using System;
using System.Collections.Generic;
using Shoy.Core.Domain.Entities;
using Shoy.Utility.Helper;

namespace Shoy.Core.Logging
{
    /// <summary>
    /// 实体数据操作日志信息类
    /// </summary>
    public class OperatingLog : Entity<Guid>
    {
        /// <summary>
        /// 初始化一个<see cref="OperatingLog"/>类型的新实例
        /// </summary>
        public OperatingLog()
        {
            Id = CombHelper.NewComb();
            Operator = ShoyContext.Current.Operator;
            OperateDate = DateTime.Now;
            LogItems = new List<OperatingLogItem>();
        }

        /// <summary>
        /// 获取或设置 实体名称
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 获取 操作人
        /// </summary>
        public Operator Operator { get; set; }

        /// <summary>
        /// 获取或设置 操作类型
        /// </summary>
        public OperatingType OperateType { get; set; }

        /// <summary>
        /// 获取或设置 操作时间
        /// </summary>
        public DateTime OperateDate { get; set; }

        /// <summary>
        /// 获取或设置 操作明细
        /// </summary>
        public List<OperatingLogItem> LogItems { get; set; }

    }


    /// <summary>
    /// 实体数据日志操作类型
    /// </summary>
    public enum OperatingType
    {
        /// <summary>
        /// 查询
        /// </summary>
        Query = 0,

        /// <summary>
        /// 插入
        /// </summary>
        Insert = 10,

        /// <summary>
        /// 更新
        /// </summary>
        Update = 20,

        /// <summary>
        /// 删除
        /// </summary>
        Delete = 30
    }


    /// <summary>
    /// 实体操作日志明细
    /// </summary>
    public class OperatingLogItem
    {
        /// <summary>
        /// 获取或设置 字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 获取或设置 旧值
        /// </summary>
        public string OriginalValue { get; set; }

        /// <summary>
        /// 获取或设置 新值
        /// </summary>
        public string NewValue { get; set; }
    }
}
