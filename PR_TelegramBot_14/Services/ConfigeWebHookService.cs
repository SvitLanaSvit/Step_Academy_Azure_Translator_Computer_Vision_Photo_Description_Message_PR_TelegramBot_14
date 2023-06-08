using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace PR_TelegramBot_14.Services
{
    public class ConfigeWebHookService : IHostedService
    {
        private readonly ILogger<ConfigeWebHookService> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly BotConfiguration botConfiguration;


        public ConfigeWebHookService(ILogger<ConfigeWebHookService> logger, IServiceProvider serviceProvider, IOptions<BotConfiguration> botOptions)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            botConfiguration = botOptions.Value;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = serviceProvider.CreateScope();
            string botUrl = $"{botConfiguration.HostAddress}{botConfiguration.Route}";
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            await botClient.SetWebhookAsync(
                url: botUrl,
                allowedUpdates: Array.Empty<UpdateType>(),
                //secretToken: botConfiguration.SecretToken,
                cancellationToken: cancellationToken);
            logger.LogInformation("Setting webhook");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var scope = serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            logger.LogInformation("Deleting webhook");
            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
    }
}
