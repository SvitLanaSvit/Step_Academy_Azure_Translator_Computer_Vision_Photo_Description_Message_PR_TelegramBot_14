using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace PR_TelegramBot_14.Services
{
    public class UpdateHandlerService
    {
        private readonly ILogger<UpdateHandlerService> logger;
        private readonly ITelegramBotClient botClient;
        private readonly AzureTranslatorService translatorService;

        public UpdateHandlerService(ILogger<UpdateHandlerService> logger, ITelegramBotClient botClient, AzureTranslatorService translatorService)
        {
            this.logger = logger;
            this.botClient = botClient;
            this.translatorService = translatorService;
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
            string? text = message.Text;
            string translatedText = await translatorService.TranslateTextAsync(text!, "en", "uk");

            //Code for future reworks
            await botClient.SendTextMessageAsync(message.From!.Id, text: $"You send: {text}\nTranslated: {translatedText}", cancellationToken: cancellationToken);
        }

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
