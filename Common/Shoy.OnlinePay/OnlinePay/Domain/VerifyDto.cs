using DayEasy.Core.Domain.Entities;

namespace DayEasy.God.Services.OnlinePay.Domain
{
    public class VerifyDto : DDto
    {
        public string TradeNo { get; set; }
        public decimal Amount { get; set; }
    }
}
