namespace CMon
{
    public class AppSettings
    {
        public int PollSeconds { get; set; } = 15;
        
        public string BaseUrl { get; set; } = "https://ccu.su";
    }
}