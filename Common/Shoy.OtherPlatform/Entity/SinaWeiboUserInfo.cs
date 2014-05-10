using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shoy.OtherPlatform.Entity
{
    [Serializable]
     public class SinaWeiboUserInfo : UserInfo
    {
        public string idstr { set; get; }

        public string name { set; get; }

        public string error { set; get; }

        public string gender { set; get;}
    }
}
