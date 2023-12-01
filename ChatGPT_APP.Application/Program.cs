using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Polling;
using Microsoft.Bot.Configuration;
using Telegram.Bot;
using ChatGPT_APP;
using Microsoft.Extensions.Logging;
using ChatGPT_APP.Services.Contract.FileCollab;
using ChatGPT_APP.Services.Contract.OpenAI;
using ChatGPT_APP.Services.Contract.VoiceConversion;
using ChatGPT_APP.Services.Services;
using ChatGPT_APP.Services.Adapter;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.SetMinimumLevel(LogLevel.Error);    
    })
    .ConfigureServices((context, services) =>
    {
        // Register Bot configuration
        services.Configure<BotConfiguration>(
            context.Configuration.GetSection(BotConfiguration.Configuration));

        // Register named HttpClient to benefits from IHttpClientFactory
        // and consume it with ITelegramBotClient typed client.
        // More read:
        //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
        //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                   // BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                    TelegramBotClientOptions options = new(BotConfiguration.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

        services.AddScoped <IVoiceConversionTelegramAdapter, VoiceConversionTelegramAdapter>();
        services.AddScoped<IVoiceConversionService, VoiceConversionService>();
        services.AddScoped<IUpdateHandler, UpdateHandler>();
        services.AddScoped<IOpenAIService, OpenAIService>();
        services.AddScoped<IFileCollabSerivice, FileCollabSerivice>();
        services.AddHostedService<ReceiverService>();
    }).Build();

await host.RunAsync();


public class BotConfiguration

{
    public static readonly string Configuration = "BotConfiguration";

    public static string BotToken { get; set; } = "6121902784:AAF2klnJ0RYe84AH-h0-i4UoDzfAS66vf_w"; 
}
