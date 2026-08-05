namespace SatraWebApplication.Model
{
    public class UserLoginLog
    {
        public long Id { get; set; }

        public string? UserId { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        public bool IsSuccessful { get; set; }
    }
}
