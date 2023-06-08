using Microsoft.Extensions.Options;
using PR_TelegramBot_14;
using PR_TelegramBot_14.Services;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfigurationSection configurationSection =  builder.Configuration.GetSection(BotConfiguration.ConfigurationName);
builder.Services.Configure<BotConfiguration>(configurationSection);
BotConfiguration botConfiguration = configurationSection.Get<BotConfiguration>();

builder.Services.AddHttpClient("telegram_bot_client").AddTypedClient<ITelegramBotClient>((httpClient, botClient) =>
{
    TelegramBotClientOptions telegramBotClientOptions = new(botConfiguration.BotToken);
    return new TelegramBotClient(telegramBotClientOptions, httpClient);
});

string apikey = builder.Configuration.GetValue<string>("AzureTranslator:ApiKey");
string endpoint = builder.Configuration.GetValue<string>("AzureTranslator:Endpoint");
string region = builder.Configuration.GetValue<string>("AzureTranslator:Region");

builder.Services.AddScoped<AzureTranslatorService>(sp =>
{
    return new AzureTranslatorService(endpoint, apikey, region);
});

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddScoped<UpdateHandlerService>();
builder.Services.AddHostedService<ConfigeWebHookService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
