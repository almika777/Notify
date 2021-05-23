namespace Common
{
    public class Configuration
    {
        public string TelegramToken { get; set; } = null!;
        public string DbPath { get; set; } = null!;
        public string LogPath { get; set; } = null!;
        public string? CacheFolder { get; set; }
    }
}
