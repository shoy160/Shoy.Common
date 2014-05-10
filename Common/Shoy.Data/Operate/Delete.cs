namespace Shoy.Data
{
    /// <summary>
    /// 删除操作类
    /// </summary>
    public class Delete:ICommandExecute
    {
        private string _mTable;

        public Delete(string mTable)
        {
            _mTable = mTable;
        }

        /// <summary>
        /// 执行Command
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public int Execute(IConnectionContext cc)
        {
            Command cmd = Command.GetThreadCommand().AddSqlText("Delete from ").AddSqlText(_mTable);

            return 0;
            return cc.ExecuteNonQuery(cmd);
        }
    }
}
