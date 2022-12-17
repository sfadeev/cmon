namespace CMon
{
    public class AppSettings
    {
        public int PollSeconds { get; set; } = 30;
        
        public string BaseUrl { get; set; } = "https://ccu.su";
    }
}