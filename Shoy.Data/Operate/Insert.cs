using System;
using System.Collections.Generic;
using System.Text;

namespace Shoy.Data
{
    /// <summary>
    /// 新增操作类
    /// </summary>
    public class Insert:ICommandExecute
    {
        private string _mTable;
        private IList<Field> _mInserFields = new List<Field>();

        public Insert(string mTable)
        {
            _mTable = mTable;
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="value">字段值</param>
        /// <returns></returns>
        public Insert AddField(string name, object value)
        {
            AddField(name, value, true);
            return this;
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="value">字段值</param>
        /// <param name="isparameter">是否是参数</param>
        /// <returns></returns>
        public Insert AddField(string name, object value, bool isparameter)
        {
            var f = new Field {IsParameter = isparameter, Name = name, Value = value};
            _mInserFields.Add(f);
            return this;
        }

        /// <summary>
        /// 执行Command
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public int Execute(IConnectionContext cc)
        {
            Command cmd = Command.GetThreadCommand().AddSqlText("Insert into ").AddSqlText(_mTable);
            StringBuilder names = new StringBuilder(),
                          values = new StringBuilder();
            for (int i = 0; i < _mInserFields.Count; i++)
            {
                if (i > 0)
                {
                    names.Append(",");
                    values.Append(",");
                }
                Field field = _mInserFields[i];
                names.Append(field.Name);
                if (field.IsParameter)
                {
                    values.Append("@").Append(field.ParameterName);
                    cmd.AddParameter(field.ParameterName, field.Value ?? DBNull.Value);
                }
                else
                    values.Append(field.Value);

            }
            cmd.SqlText.Append("(").Append(names).Append(")").Append(" Values(").Append(values).Append(")");
            return cc.ExecuteNonQuery(cmd);
        }
    }
}
