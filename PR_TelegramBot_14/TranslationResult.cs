using System.Text.Json.Serialization;

namespace PR_TelegramBot_14
{
    public class Translation
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("to")]
        public string To { get; set; }
    }

    public class TranslationResult
    {
        [JsonPropertyName("translations")]
        public List<Translation> Translations { get; set; }
    }
}
