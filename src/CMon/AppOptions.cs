namespace CMon
{
    public class AppOptions
    {
        public int CacheMinutes { get; set; } = 60;
        
        public int PollSeconds { get; set; } = 15;
        
        public string BaseUrl { get; set; } = "https://ccu.su/data.cgx";
        
        public string Imei { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}