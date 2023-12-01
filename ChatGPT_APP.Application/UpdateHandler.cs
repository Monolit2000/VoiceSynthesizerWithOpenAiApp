using ChatGPT_APP.Models;
using ChatGPT_APP.Services.Adapter;
using ChatGPT_APP.Services.Contract.FileCollab;
using ChatGPT_APP.Services.Contract.VoiceConversion;
using ChatGPT_APP.Services.Models.RequestModel;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq.Expressions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace ChatGPT_APP
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly ILogger<UpdateHandler> _logger;
        private readonly IVoiceConversionService _voiceConversionService;
        private readonly IFileCollabSerivice _fileCollabSerivice;
        private readonly IVoiceConversionTelegramAdapter _voiceConversionTelegramAdapter;

        public UpdateHandler(
            ITelegramBotClient botClient, 
            ILogger<UpdateHandler> logger,
            IFileCollabSerivice fileCollabSerivice,
            IVoiceConversionService voiceConversionService,
            IVoiceConversionTelegramAdapter voiceConversionTelegramAdapter)
        {
            _voiceConversionTelegramAdapter = voiceConversionTelegramAdapter;
            _voiceConversionService = voiceConversionService;
            _fileCollabSerivice = fileCollabSerivice;
            _telegramBotClient = botClient;
            _logger = logger;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
        {
            if (update.Message != null && update.Message.Type is MessageType.Voice)
            {
                var handler = update switch
                {
                    { Message: { } message } => BotOnVoiseMessageReceived(message)
                };

                await Console.Out.WriteLineAsync($"ChatID ----- {update.Message.Chat.Id}");

                await handler;

                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Voice message completely converted!");
            }
        }

        private async Task BotOnVoiseMessageReceived(Message message)
        {
            var messageGifId = await SendGif(Configuration.spiderMan_GIF, message.Chat.Id);


            var responce = _voiceConversionTelegramAdapter.TelegramAudioToTextConverter(message);
            await foreach (var result in responce)
            {
                await Console.Out.WriteLineAsync(result.Text);

                await SendAudioWithText(message.Chat.Id, result);
            }

            
            await _telegramBotClient.DeleteMessageAsync(message.Chat.Id, messageGifId);
            await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Voice message completely converted!");
        }


        #region Send Audio With Text
        private async Task SendAudioWithText(long chatId, ConversionResponce VoisWithText)
        {              
          //  if (VoisWithText.VoiceFileSream != null)
            //    await _telegramBotClient.SendVoiceAsync(chatId: chatId, voice: InputFile.FromStream(VoisWithText.VoiceFileSream));

            await _telegramBotClient.SendTextMessageAsync(chatId, $" Your voice message part: {VoisWithText.PartNumber}: \r {VoisWithText.Text}");

            await Console.Out.WriteLineAsync($"voice message part: {VoisWithText.PartNumber} sended ");  
        }
        #endregion

        #region Send Gif
        async Task<int> SendGif(string GIFPath, long ChatId)
        {
            using (var gifStream = System.IO.File.Open(GIFPath, FileMode.Open))
            {
                // InputFile gifFile = new InputFile(gifStream, "my_gif.gif");
                var gifFile = InputFile.FromStream(gifStream, "monkey_gif.gif");
                var messageT = await _telegramBotClient.SendAnimationAsync(ChatId, gifFile, caption: "Processing...");

                int messageId = messageT.MessageId;

                return messageId;
                // Console.WriteLine($"Sent message ID: {messageId}");
            }
        }
        #endregion

        #region Handle Polling Error
        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

            await Console.Out.WriteLineAsync(ErrorMessage);

            // Cooldown in case of network connection error
            if (exception is RequestException)
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
        #endregion

        #region work with ChatGPT API
        private async Task SendResponceFromChatGPT()
        {
            //Sending a GIF message from a selected directory 
            //  int messageGifId = await SendGif(GIFPath, message.Chat.Id);

            //var procesChat =  await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"Chat processing...");

            // var responceFromCHATgptAPI = await chatFunkAsync(result);

            // await _telegramBotClient.DeleteMessageAsync(message.Chat.Id, procesChat.MessageId);

            // await _telegramBotClient.DeleteMessageAsync(message.Chat.Id, messageGifId);

            // await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $" Reply from  chatGPT:\r {responceFromCHATgptAPI}");
        }
        #endregion

    }
}
