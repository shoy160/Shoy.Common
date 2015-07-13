
namespace Shoy.ThirdPlatform.Entity
{
    public class UserResult
    {
        /// <summary>
        /// Id：OpenId/Uid/user_id
        /// </summary>
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public string Nick { get; set; }
        public string Profile { get; set; }
        public string Gender { get; set; }
    }
}
