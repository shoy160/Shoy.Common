
namespace Shoy.OnlinePay
{
    public class BaseResult
    {
        public string TradeNum { get; set; }
        public bool State { get; set; }
        public decimal Amount { get; set; }
        public string ErrMsg { get; set; }
    }
}
