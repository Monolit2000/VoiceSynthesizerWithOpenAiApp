using OpenAI.Audio;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChatGPT_APP.Models;
using ChatGPT_APP.Services.Contract.OpenAI;
using Telegram.Bot.Types;
using ChatGPT_APP.Services.Models.RequestModel;

namespace ChatGPT_APP.Services.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly string apiKey = Configuration.OpenAI_Api_Key;

        public async Task<string> VoiseMessageSynthesizerFromOpenAI(Stream audioStream)
        {

            var api = new OpenAIClient(apiKey);
            var transcriptionRequest = new AudioTranscriptionRequest(
                audio: audioStream, 
                audioName: "");
            try
            {
                string result = await api.AudioEndpoint.CreateTranscriptionAsync(transcriptionRequest);

                return result;
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    Console.WriteLine("Програма на паузе 5 сек");

                    await Task.Delay(TimeSpan.FromSeconds(5));

                    return await VoiseMessageSynthesizerFromOpenAI(audioStream);
                }
                else
                {
                    Console.WriteLine($"Erorr: {ex.Message}");
                    throw;
                }
            }       
        }

        #region Work with API ChatGPT 
            //code (≧∇≦)ﾉ
        #endregion

    }
}
