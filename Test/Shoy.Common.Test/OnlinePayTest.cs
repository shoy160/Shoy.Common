using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.OnlinePay.App;
using Shoy.OnlinePay.App.Utils;
using Shoy.Utility.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shoy.Common.Test
{
    [TestClass]
    public class OnlinePayTest : TestBase
    {
        [TestMethod]
        public void VerifyTest()
        {
            var paramDict = new Dictionary<string, string>{
                    {"total_amount", "0.01"},
                    {"buyer_id", "2088702417642577"},
                    {"trade_no", "2016122321001004570233068174"},
                    {"notify_time", "2016-12-24 03,54,54"},
                    {"subject", "余额充值"},
                    {"sign_type", "RSA"},
                    {"buyer_logon_id", "101***@qq.com"},
                    {"auth_app_id", "2016122304551127"},
                    {"charset", "utf-8"},
                    {"notify_type", "trade_status_sync"},
                    {"invoice_amount", "0.01"},
                    {"out_trade_no", "7466e891855e472ba7bdcfec684f63ca"},
                    {"trade_status", "TRADE_SUCCESS"},
                    {"gmt_payment", "2016-12-23 18,30,31"},
                    {"version", "1.0"},
                    {"point_amount", "0.00"},
                    {"sign", "Uv5vYvkwI8GxLrMNhVZIBszraf9FPlakbRXRANuMixspUmbu/JZrlCx8Hxh0ihC5WwYlkBCQYgNeTQjk pUbhgwm87Bb9yaJ1Al/PI e22C2C93uiWa4KLxgKaqcOp1ESsZHJxBW8r1DVcLHEEDyuvM0clIh5q4XnN5L6ZZeHpU="},
                    {"gmt_create", "2016-12-23 18,30,30"},
                    {"buyer_pay_amount", "0.01"},
                    {"receipt_amount", "0.01"},
                    {"fund_bill_list", "[{\"amount\",\"0.01\",\"fundChannel\",\"ALIPAYACCOUNT\"}]"},
                    {"app_id", "2016122304551127"},
                    {"seller_id", "2088521455102434"},
                    {"notify_id", "6a4cfbc8ea1dba6293314f2c763815fkee"},
                    {"seller_email", "badou_service@163.com"}
                };
            var config = ConfigUtils<OnlinePayConfig>.Config.Platforms.First(t => t.Type == PaidType.Alipay);
            var signVerified = AlipaySignature.RsaCheck(paramDict, config.PublicKey, config.Charset);
            Console.WriteLine(signVerified);
        }
    }
}
