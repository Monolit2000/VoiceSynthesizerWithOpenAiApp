using ChatGPT_APP.Services.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT_APP.Services.Contract.OpenAI
{
    public interface IOpenAIService
    {
        Task<string> VoiseMessageSynthesizerFromOpenAI(Stream audioStream);
    }
}
