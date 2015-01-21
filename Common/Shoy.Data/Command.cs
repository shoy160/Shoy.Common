using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace Shoy.Data
{
    public class Command
    {
        private readonly StringBuilder _sql;

        private CommandType _type = CommandType.Text;

        private readonly IList<Parameter> _parameters;

        public CommandType CommandType { get { return _type; } set { _type = value; } }

        public StringBuilder SqlText { get { return _sql; } }

        public Command(string sql)
        {
            _sql = new StringBuilder(512);
            _sql.Append(sql);
            _parameters = new List<Parameter>();
        }

        public IDbCommand DbCommand { get; set; }

        public IList<Parameter> Parameters { get { return _parameters; } }

        public Command AddParameter(string name,object value,ParameterDirection direction)
        {
            var para = new Parameter {Name = name, Value = value, Direction = direction};
            if (_parameters.Count(t => t.Name == name) == 0)
                _parameters.Add(para);
            return this;
        }

        public Command AddParameter(string name,object value)
        {
            return AddParameter(name, value, ParameterDirection.Input);
        }

        public Command AddParameter(Parameter para)
        {
            _parameters.Add(para);
            return this;
        }

        public Command AddSqlText(string sql)
        {
            SqlText.Append(sql);
            return this;
        }

        public void ClearParameters()
        {
            Parameters.Clear();
        }

        public void Clean()
        {
            ClearParameters();
            SqlText.Remove(0, SqlText.Length);
        }

        public IDbCommand CreateCommand(IDriver driver)
        {
            IDbCommand cmd = driver.Command;
            cmd.CommandText = driver.ReplaceSql(SqlText.ToString());
            cmd.CommandType = CommandType;
            DbCommand = cmd;
            foreach (Parameter p in Parameters)
            {
                cmd.Parameters.Add(driver.CreateParameter(p.Name, p.Value, p.Direction));
            }
            return cmd;
        }

        internal bool IsChange()
        {
            return Regex.IsMatch(SqlText.ToString(), "((insert)|(update)|(delete))\\s+", RegexOptions.IgnoreCase);
        }

        internal bool IsSelect()
        {
            return Regex.IsMatch(SqlText.ToString(), "select\\s+", RegexOptions.IgnoreCase);
        }

        internal bool IsValue()
        {
            return Regex.IsMatch(SqlText.ToString(), "^([a-z0-9]+)$", RegexOptions.IgnoreCase);
        }

        [ThreadStatic]
        private static Command _threadCommand;

        public static Command GetThreadCommand()
        {
            if (_threadCommand == null)
                _threadCommand = new Command(string.Empty);
            _threadCommand.Clean();
            return _threadCommand;
        }
    }
}
