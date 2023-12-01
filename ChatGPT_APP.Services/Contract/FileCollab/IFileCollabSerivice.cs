using ChatGPT_APP.Services.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ChatGPT_APP.Services.Contract.FileCollab
{
    public interface IFileCollabSerivice
    {
        Task<List<Stream>> GetSplitAudioFilePathByUnitOfTimeList(ConversionRequest request, int TimeInterval);
        Task<string> GetTelegramVoisFilePathAsync(Stream fileStream, long id, string name = "T");
        Task<Stream> GetTelegramVoisStreamAsync(string FileId);






    }
}
