using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shoy.OnlinePay.MwAlipay
{
    public class MwAlipayResult : BaseResult
    {
        //支付宝交易号
        public string trade_no { get; set; }
    }
}
