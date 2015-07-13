using System;

namespace Shoy.ThirdPlatform.Entity
{
    [Serializable]
    public class WeiboUser : UserBase
    {
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string Nick { get; set; }
        public string Profile { get; set; }
        public string Gender { get; set; }
    }
}
