namespace TelegramBot.Options
{
    public class TelegramOptions
    {
        public const string Telegram = nameof(Telegram);
        public required string Token{ get; set; }
        public required string AdminId { get; set; }
    }
}
