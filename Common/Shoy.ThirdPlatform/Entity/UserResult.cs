
namespace Shoy.ThirdPlatform.Entity
{
    /// <summary> 统一返回数据 </summary>
    public class UserResult
    {
        /// <summary> OpenId/Uid/user_id </summary>
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public string Nick { get; set; }
        public string Profile { get; set; }
        public string Gender { get; set; }
    }
}
