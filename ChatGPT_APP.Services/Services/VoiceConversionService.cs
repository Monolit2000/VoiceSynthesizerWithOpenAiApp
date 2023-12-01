using System.Diagnostics;
using System.Net;
using Telegram.Bot.Types;
using Telegram.Bot;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Audio;
using ChatGPT_APP.Models;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Primitives;
using OpenAI.Chat;
using Message = Telegram.Bot.Types.Message;
using System.Net.WebSockets;
using ChatGPT_APP.Services.Contract.FileCollab;
using ChatGPT_APP.Services.Contract.OpenAI;
using ChatGPT_APP.Services.Contract.VoiceConversion;
using System.IO;
using ChatGPT_APP.Services.Models.RequestModel;

namespace ChatGPT_APP.Services.Services
{
    public class VoiceConversionService : IVoiceConversionService
    {
        private readonly IFileCollabSerivice _fileCollabSerivice;
        private readonly IOpenAIService _openAIService;


        public VoiceConversionService(IFileCollabSerivice fileCollabSerivice, IOpenAIService openAIService)
        {
            _fileCollabSerivice = fileCollabSerivice;
            _openAIService = openAIService;
        }

        public async IAsyncEnumerable<ConversionResponce> TelegramAudioToTextConverter(ConversionRequest request)
        {
            var duration = request.Duration;

            const int fourMinute = 4 * 60;

            switch (duration)
            {
                case >= fourMinute:
                    await foreach (var responce in GetSegmentsAudioWithTextAsync(request))
                    {
                        yield return responce;
                    }
                    break;

                default:

                    var inputFilePath = await _fileCollabSerivice.GetTelegramVoisFilePathAsync(request.VoiceFileSream,request.id);

                    var voiceStream = await Task.Run(() => System.IO.File.OpenRead(inputFilePath));

                    yield return new ConversionResponce(await _openAIService.VoiseMessageSynthesizerFromOpenAI(voiceStream));


                    //await foreach (var responcee in GetSinglAudioWithTextAsync(request))
                    //{
                    //    yield return responcee;
                    //}
                    break;
            }
        }


        private async IAsyncEnumerable<ConversionResponce> GetSegmentsAudioWithTextAsync(ConversionRequest request)
        {
            var splitedAudioList = await _fileCollabSerivice.GetSplitAudioFilePathByUnitOfTimeList(request, 3);

            for (int partNumber = 0; partNumber < splitedAudioList.Count; partNumber++)
            {
                var voiceStream = splitedAudioList[partNumber];
                var synthesTest = await _openAIService.VoiseMessageSynthesizerFromOpenAI(splitedAudioList[partNumber]);

                yield return new ConversionResponce(voiceStream, synthesTest, partNumber);
            }

            await Console.Out.WriteLineAsync($"voice message completely converted ");
        }


        private async IAsyncEnumerable<ConversionResponce> GetSinglAudioWithTextAsync(ConversionRequest request)
        {
            await Console.Out.WriteLineAsync("Short vosie message ");

            if (request.VoiceFileSream is null)
                throw new Exception("VoiceFileSream is null");

            var synthesizedText = await _openAIService.VoiseMessageSynthesizerFromOpenAI(request.VoiceFileSream);

            yield return new ConversionResponce(synthesizedText);
        }

    }
}