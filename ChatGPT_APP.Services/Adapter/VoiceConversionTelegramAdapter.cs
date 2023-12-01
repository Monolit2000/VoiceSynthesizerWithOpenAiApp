using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatGPT_APP.Models;
using ChatGPT_APP.Services.Contract.FileCollab;
using ChatGPT_APP.Services.Contract.VoiceConversion;
using ChatGPT_APP.Services.Models.RequestModel;
using OpenAI.Chat;
using Telegram.Bot.Types;

namespace ChatGPT_APP.Services.Adapter
{
    public class VoiceConversionTelegramAdapter : IVoiceConversionTelegramAdapter
    {

        private readonly IVoiceConversionService _voiceConversionService;
        private readonly IFileCollabSerivice _fileCollabSerivice;
    
        public VoiceConversionTelegramAdapter(IVoiceConversionService voiceConversionService, IFileCollabSerivice fileCollabSerivice)
        {
            _fileCollabSerivice = fileCollabSerivice;
            _voiceConversionService = voiceConversionService;
        }

        public async IAsyncEnumerable<ConversionResponce> TelegramAudioToTextConverter(Telegram.Bot.Types.Message massege)
        {
            var duration = massege.Voice.Duration;
            var VoiseId = massege.Voice.FileId;

            var stream = await _fileCollabSerivice.GetTelegramVoisStreamAsync(VoiseId);
            var messageId = massege.MessageId;

            //var voiceAndTextRequest = new ConversionRequest(stream, duration)
            //{
            //    id = massege.MessageId
            //};

            var filePath = await _fileCollabSerivice.GetTelegramVoisFilePathAsync(stream, messageId);

            var voiceStream = await Task.Run(() =>
            System.IO.File.OpenRead(filePath))
                .ConfigureAwait(false);

            var request = new ConversionRequest(voiceStream, duration, filePath);

            var result = _voiceConversionService.TelegramAudioToTextConverter(request);
            await foreach (var responce in result)
            {
                yield return responce;
            }

        }


    }
}
