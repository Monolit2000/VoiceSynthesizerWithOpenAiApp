using ChatGPT_APP.Models;
using ChatGPT_APP.Services.Models.RequestModel;
//using ChatGPT_APP.Services.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT_APP.Services.Contract.VoiceConversion
{
    public interface IVoiceConversionService
    {
        IAsyncEnumerable<ConversionResponce> TelegramAudioToTextConverter(ConversionRequest request);
    }
}
