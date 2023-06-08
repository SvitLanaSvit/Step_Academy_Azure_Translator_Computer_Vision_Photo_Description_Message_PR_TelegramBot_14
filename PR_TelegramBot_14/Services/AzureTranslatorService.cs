using System.Text;
using System.Text.Json;

namespace PR_TelegramBot_14.Services
{
    public class AzureTranslatorService
    {
        private readonly string endpoint;
        private readonly string apikey;
        private readonly string region;

        public AzureTranslatorService(string endpoint, string apikey, string region)
        {
            this.endpoint = endpoint;
            this.apikey = apikey;
            this.region = region;
        }

        public async Task<string> TranslateTextAsync(string text, string fromLanguage, string toLanguage)
        {
            
            string url = $"translate?api-version=3.0&to={toLanguage}&from={fromLanguage}";
            string requestUrl = $"{endpoint}{url}";

            using(HttpClient client = new HttpClient())
            {
                using(HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl)) 
                {
                    requestMessage.Headers.Add("Ocp-Apim-Subscription-Key", apikey);
                    requestMessage.Headers.Add("Ocp-Apim-Subscription-Region", region);
                    //requestMessage.Headers.Add("Content-Type", "application/json");

                    object textForTranslate = new object[] { new {Text = text}};
                    string body = JsonSerializer.Serialize(textForTranslate);
                    requestMessage.Content = new StringContent(body, encoding: Encoding.UTF8, "application/json");

                    HttpResponseMessage message = await client.SendAsync(requestMessage);
                    string translation = await message.Content.ReadAsStringAsync();

                    List<TranslationResult>? translationResults = JsonSerializer.Deserialize<List<TranslationResult>>(translation);

                    string translatedText = translationResults![0].Translations[0].Text;
                    return translatedText;
                }
            }
        }
    }
}
