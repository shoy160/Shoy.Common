namespace Shoy.ThirdPlatform.Entity
{
    public abstract class UserBase
    {
        public bool Status { get; set; }
        public string Message { get; set; }

        public UserBase()
        {
            Status = true;
            Message = string.Empty;
        }
    }
}