using ChatGPT_APP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ChatGPT_APP.Services.Adapter
{
    public interface IVoiceConversionTelegramAdapter
    {
        IAsyncEnumerable<ConversionResponce> TelegramAudioToTextConverter(Message request);
    }
}
