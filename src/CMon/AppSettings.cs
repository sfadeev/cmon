namespace CMon
{
    public class AppSettings
    {
        public int PollSeconds { get; set; } = 10;
        
        public string BaseUrl { get; set; } = "https://ccu.su";
    }
}