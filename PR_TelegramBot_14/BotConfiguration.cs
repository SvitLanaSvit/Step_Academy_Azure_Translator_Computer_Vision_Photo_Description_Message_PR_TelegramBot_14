namespace PR_TelegramBot_14
{
    public class BotConfiguration
    {
        public static string ConfigurationName = "BotConfiguration";
        public string BotToken { get; set; } = default!;
        public string Route { get; set; } = default!;
        public string HostAddress { get; set; } = default!;
        public string SecretToken { get; set; } = default!;
    }
}
