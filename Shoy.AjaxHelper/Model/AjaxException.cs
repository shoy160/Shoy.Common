using System;
using Shoy.Utility.Extend;

namespace Shoy.AjaxHelper.Model
{
    public class AjaxException:Exception
    {
        private readonly AjaxResult _reulst;

        public AjaxException(string errMsg)
        {
            _reulst = new AjaxResult {state = 0, msg = errMsg};
        }

        public AjaxException(AjaxResult result)
        {
            _reulst = result;
        }

        public AjaxResult GetResult()
        {
            return _reulst;
        }

        public override string Message
        {
            get
            {
                //if (_errMsg.IsNotNullOrEmpty())
                //    return _errMsg;
                if (_reulst != null)
                    return _reulst.ToJson();
                return base.Message;
            }
        }
    }
}
