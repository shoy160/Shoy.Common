using System;
using System.Collections.Generic;
using System.Text;

namespace Shoy.Data
{
    /// <summary>
    /// 条件表达式
    /// </summary>
    [Serializable]
    public class Expression
    {
        public Expression()
        {

        }
        private readonly List<Parameter> _mParameters = new List<Parameter>();
        public List<Parameter> Parameters
        {
            get
            {
                return _mParameters;
            }
        }
        private readonly StringBuilder _mSqlText = new StringBuilder();
        public StringBuilder SqlText
        {
            get
            {
                return _mSqlText;
            }

        }

        internal void Parse(Command cmd)
        {
            if (SqlText.Length > 0)
            {
                cmd.SqlText.Append(" where ").Append(SqlText);
                foreach (Parameter p in Parameters)
                {
                    cmd.AddParameter(p);
                }
            }
        }

        public static Expression operator &(Expression exp1, Expression exp2)
        {
            if (exp1 == null || exp1.SqlText.Length == 0)
                return exp2;
            if (exp2 == null || exp2.SqlText.Length == 0)
                return exp1;

            var exp = new Expression();
            // exp.ParameterNameIndex = exp1.ParameterNameIndex>exp2.ParameterNameIndex?exp1.ParameterNameIndex:exp2.ParameterNameIndex;
            exp.SqlText.Append("(");
            exp.SqlText.Append(exp1);

            exp.SqlText.Append(")");
            exp.Parameters.AddRange(exp1.Parameters);
            exp.SqlText.Append(" and (");
            exp.SqlText.Append(exp2.SqlText);
            exp.SqlText.Append(")");
            exp.Parameters.AddRange(exp2.Parameters);
            return exp;
        }
        public static Expression operator |(Expression exp1, Expression exp2)
        {
            if (exp1 == null || exp1.SqlText.Length == 0)
                return exp2;
            if (exp2 == null || exp2.SqlText.Length == 0)
                return exp1;
            var exp = new Expression();
            // exp.ParameterNameIndex = exp1.ParameterNameIndex > exp2.ParameterNameIndex ? exp1.ParameterNameIndex : exp2.ParameterNameIndex;
            exp.SqlText.Append("(");
            exp.SqlText.Append(exp1);

            exp.SqlText.Append(")");
            exp.Parameters.AddRange(exp1.Parameters);
            exp.SqlText.Append(" or (");
            exp.SqlText.Append(exp2.SqlText);
            exp.SqlText.Append(")");
            exp.Parameters.AddRange(exp2.Parameters);
            return exp;

        }

        internal static string GetParamName()
        {
            ParamNameSeed pns = NameSeed;
            //if (pns.Value > 200)
            //    pns.Value = 0;
            //else
            pns.Value++;
            return "tmp_p" + pns.Value;
        }

        [ThreadStatic]
        static ParamNameSeed _mNameSeed = new ParamNameSeed();

        internal static ParamNameSeed NameSeed
        {
            get { return _mNameSeed ?? (_mNameSeed = new ParamNameSeed()); }
        }

        internal class ParamNameSeed
        {
            public int Value
            {
                get;
                set;
            }
        }
        public override string ToString()
        {

            return SqlText.ToString();
        }

        public Expression AddSql(string sql)
        {
            _mSqlText.Append(sql);
            return this;
        }

        public Expression Add(string name, object value)
        {
            _mParameters.Add(new Parameter { Name = name, Value = value });
            return this;
        }
    }
}
