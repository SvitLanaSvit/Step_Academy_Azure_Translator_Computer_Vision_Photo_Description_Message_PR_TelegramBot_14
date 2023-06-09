using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace PR_TelegramBot_14.Services
{
    public class UpdateHandlerService
    {
        private readonly ILogger<UpdateHandlerService> logger;
        private readonly ITelegramBotClient botClient;
        private readonly AzureTranslatorService translatorService;
        private readonly BotConfiguration botConfiguration;
        private readonly AzurePhotoService photoService;

        public UpdateHandlerService(ILogger<UpdateHandlerService> logger, ITelegramBotClient botClient, AzureTranslatorService translatorService, AzurePhotoService photoService, IOptions<BotConfiguration> botConfigurationOptions)
        {
            this.logger = logger;
            this.botClient = botClient;
            this.translatorService = translatorService;
            this.botConfiguration = botConfigurationOptions.Value;
            this.photoService = photoService;
        }

        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            Task handler = update switch
            {
                { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
                { EditedMessage: { } message } => BotOnMessageReceived(message, cancellationToken),
                _ => UnknownUpdateTypeHandleAsync(update, cancellationToken)
            };
            await handler;
        }

        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            if (message.Type == MessageType.Photo)
            {
                var photo = message.Photo!.Last(); // Get the last photo from the array (the highest resolution)
                var fieldId = photo.FileId;

                var file = await botClient.GetFileAsync(fieldId, cancellationToken);
                string fileUrl = $"https://api.telegram.org/file/bot{botConfiguration.BotToken}/{file.FilePath}";

                // Pass the file URL to the AzurePhotoService to get the description
                var description = await photoService.GetImageDescriptionAsync(fileUrl);//
                await botClient.SendTextMessageAsync(message.Chat.Id, text: description, cancellationToken: cancellationToken);
            }
            else if (message.Type == MessageType.Text)
            {
                string? text = message.Text;
                string translatedText = await translatorService.TranslateTextAsync(text!, "en", "uk");

                //Code for future reworks
                await botClient.SendTextMessageAsync(message.From!.Id, text: $"You send: {text}\nTranslated: {translatedText}", cancellationToken: cancellationToken);
            }
        }

        //private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        //{
        //    if (message.Type == MessageType.Photo)
        //    {
        //        var photo = message.Photo!.Last(); // Get the last photo from the array (the highest resolution)
        //        var fieldId = photo.FileId;
        //        try
        //        {
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                await botClient.GetInfoAndDownloadFileAsync(fieldId, ms);
        //                ms.Seek(0, SeekOrigin.Begin);
        //                var description = await photoService.GetImageDescriptionAsync(ms);//

        //                await botClient.SendTextMessageAsync(message.Chat.Id, text: description, cancellationToken: cancellationToken);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.LogWarning(ex.Message);
        //        }
        //    }
        //    else if (message.Type == MessageType.Text)
        //    {
        //        string? text = message.Text;
        //        string translatedText = await translatorService.TranslateTextAsync(text!, "en", "uk");

        //        //Code for future reworks
        //        await botClient.SendTextMessageAsync(message.From!.Id, text: $"You send: {text}\nTranslated: {translatedText}", cancellationToken: cancellationToken);
        //    }
        //}

        private Task UnknownUpdateTypeHandleAsync(Update upddate, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Uknown update type: {upddate.Type}");
            return Task.CompletedTask;
        }

        public Task HandlErrorAsync(Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            logger.LogWarning(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
