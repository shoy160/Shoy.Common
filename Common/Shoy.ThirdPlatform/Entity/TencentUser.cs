using System;

namespace Shoy.ThirdPlatform.Entity
{
    /// <summary> 腾讯接口返回数据 </summary>
    [Serializable]
    public class TencentUser : UserBase
    {
        public string AccessToken { get; set; }
        public string Nick { get; set; }
        public string Profile { get; set; }
        public string Gender { get; set; }
    }
}
