namespace SatraWebApplication.Model
{
    public class SatraUser
    {
        public int ID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; }
        public  bool IsValid { get; set; }
        public SatraUser()
        {
            
        }

    }
}
