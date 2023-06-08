using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PR_TelegramBot_14.Services;
using Telegram.Bot.Types;

namespace PR_TelegramBot_14.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly ILogger<BotController> logger;
        //private readonly UpdateHandlerService updateHandler;

        public BotController(
            ILogger<BotController> logger 
            //UpdateHandlerService updateHandler
            )
        {
            this.logger = logger;
            //this.updateHandler = updateHandler;
        }

        [HttpPost]
        public async Task Post([FromBody]Update update, [FromServices]UpdateHandlerService updateHandler, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Handling update: {update.Id}");
            await updateHandler.HandleUpdateAsync(update, cancellationToken);
        }
    }
}
