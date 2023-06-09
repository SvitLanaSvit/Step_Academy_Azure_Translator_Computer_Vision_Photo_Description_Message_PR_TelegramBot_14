using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace PR_TelegramBot_14.Services
{
    public class AzurePhotoService
    {
        private readonly string endpoint;
        private readonly string apiKey;

        public AzurePhotoService(string endpoint, string apiKey)
        {
            this.endpoint = endpoint;
            this.apiKey = apiKey;
        }

        //Use without Stream and changing in UpdateHandlerService in method (BotOnMessageReceived())

        public async Task<string> GetImageDescriptionAsync(string imageUrl)
        {
            var client = CreateVisionClient(apiKey, endpoint);
            var features = new List<VisualFeatureTypes?>
            {
                VisualFeatureTypes.Description
            };

            var result = await client.AnalyzeImageAsync(imageUrl, features);
            string? description = result.Description.Captions.FirstOrDefault()?.Text;

            return description ?? "Unable to generate description.";
        }

        //Use with Stream and changing in UpdateHandlerService in method (BotOnMessageReceived())

        //public async Task<string> GetImageDescriptionAsync(Stream stream)
        //{
        //    var client = CreateVisionClient(apiKey, endpoint);
        //    var features = new List<VisualFeatureTypes?>
        //    {
        //        VisualFeatureTypes.Description
        //    };

        //    //var result = await client.AnalyzeImageAsync(imageUrl, features);
        //    var result = await client.AnalyzeImageInStreamAsync(stream, features);
        //    string? description = result.Description.Captions.FirstOrDefault()?.Text;

        //    return description ?? "Unable to generate description.";
        //}

        private ComputerVisionClient CreateVisionClient(string apiKey, string endpoint)
        {
            var credentials = new ApiKeyServiceClientCredentials(apiKey);
            var client = new ComputerVisionClient(credentials)
            {
                Endpoint = endpoint
            };

            return client;
        }
    }
}
