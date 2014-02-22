namespace Shoy.OnlinePay.Common
{
    public class PartnerInfo
    {
        public int PayType { get; set; }
        public PayType Type { get { return (PayType) PayType; } }
        public string PartnerId { get; set; }
        public string Key { get; set; }
        public string SellerEmail { get; set; }
    }
}