namespace TelegramBot.Options
{
    public class TelegramOptions
    {
        public const string Telegram = nameof(Telegram);
        public string Token{ get; set; }
        public string AdminId { get; set; }
    }
}
