using System;

namespace Shoy.OnlinePay.YeePay
{
    public class YeepayResult:BaseResult
    {
        public string Code { get; set; }
        public DateTime PayDate { get; set; }
        public bool NeedResponse { get; set; }
        public string TrxId { get; set; }
    }
}
