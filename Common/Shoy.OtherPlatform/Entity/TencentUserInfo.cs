using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shoy.OtherPlatform.Entity
{
    [Serializable]
    public class TencentUserInfo : UserInfo 
    {
        //腾讯有木有想过我们码农的感受  - -!
        public string ret { get; set; }
        public string msg { get; set; }
        public string uid { get; set; }
        public string nickname { get; set; }
        public string gender { get; set; }
        public string figureurl { get; set; }
        public string figureurl_1 { get; set; }
        public string figureurl_2 { get; set; }
        public string vip { get; set; }
        public string level { get; set; }
        public string is_yellow_year_vip { get; set; }

    }
}
