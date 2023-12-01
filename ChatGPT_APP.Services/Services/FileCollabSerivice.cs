using ChatGPT_APP.Models;
using ChatGPT_APP.Services.Contract.FileCollab;
using ChatGPT_APP.Services.Models.RequestModel;
using Microsoft.Extensions.Logging;
using OpenAI.Files;
using System.Diagnostics;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace ChatGPT_APP.Services.Services
{
    public class FileCollabSerivice : IFileCollabSerivice
    {
        private readonly string ffmpegPath = Configuration.ffmpegPath;
        private readonly string SaveFilePath = Configuration.SaveFilePath;
        

        private readonly ITelegramBotClient _telegramBotClient;
        private readonly ILogger<FileCollabSerivice> _logger; 

        public FileCollabSerivice(ILogger<FileCollabSerivice> logger,ITelegramBotClient telegramBotClient)
        {
            _logger = logger;
            _telegramBotClient = telegramBotClient;
        }

        public async Task<List<Stream>> GetSplitAudioFilePathByUnitOfTimeList(ConversionRequest request, int TimeInterval)
        {
            await Console.Out.WriteLineAsync("Long vosie request");

            var outputFiles = new List<Stream>();

            var inputFilePath = request.AudioPath;

            await Console.Out.WriteLineAsync(inputFilePath);

            await Console.Out.WriteLineAsync($" file: << {inputFilePath} >> was received");
            _logger.LogInformation($" file: << {inputFilePath} >> was received");
      

            int minutesCount = request.Duration / (TimeInterval * 60);
            int remainder = request.Duration % (TimeInterval * 60);

            await Console.Out.WriteLineAsync($"{remainder}");

            if (remainder > 0)
                minutesCount++;

            Console.WriteLine(minutesCount);

            for (int i = 0; i < minutesCount; i++)
            {
                int start = i * 60;
                int end = (i + TimeInterval) * 60;

                if (i == minutesCount - 1 && remainder > 0)
                {
                    end = start + remainder;
                }

                string convertedAudioFilePath = inputFilePath + i + ".mp3";

                await SplitAudio(inputFilePath, convertedAudioFilePath, start, end);
                await Console.Out.WriteLineAsync($"{convertedAudioFilePath}");

                outputFiles.Add(await GetVoiceFileStream(convertedAudioFilePath));

             
            }

            return outputFiles;
        }


        public async Task<string> GetTelegramVoisFilePathAsync(Stream fileStream, long id, string name = "T")
        {
            try
            {
                var VoiseFilePath = Path.Combine(SaveFilePath, $"{id}AudiofileMP3{name}.mp3");

                Stream stream = File.Create(VoiseFilePath);
                await fileStream.CopyToAsync(stream).ConfigureAwait(false);
                stream.Close();
                return VoiseFilePath;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<Stream> GetTelegramVoisStreamAsync(string FileId)
        {
            try
            {
                var voiceStream = new MemoryStream();
                var voiceFile = await _telegramBotClient.GetFileAsync(FileId);
                await _telegramBotClient.DownloadFileAsync(voiceFile.FilePath, voiceStream);
                voiceStream.Seek(0, SeekOrigin.Begin); 
                return voiceStream;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting voice stream: {ex}");
                throw new Exception (ex.Message);  
            }
        }


        private async Task SplitAudio(string inputFile, string outputFile, int start, int end)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = ffmpegPath;
                startInfo.Arguments = $"-i \"{inputFile}\" -ss {start} -to {end} \"{outputFile}\"";
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    await process.WaitForExitAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting voice stream: {ex}");
                throw new Exception(ex.Message);
            }
        }


        private async Task<Stream> GetVoiceFileStream(string voiceFilePath)
        {

            var voiceStream = await Task.Run(() => System.IO.File.OpenRead(voiceFilePath));
            return voiceStream;
        }

        private bool IsFileReadable(string filePath)
        {
            try
            {
                using (var fileStream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Console.WriteLine("Файл открыт только для чтения");
                    return true;
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Файл заблокирован или не доступен для чтения");
                return false;
            }
        }

        static async Task DeleteFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                await Console.Out.WriteLineAsync($"the file at this path: {filePath} has been deleted");
                await Task.Run(() => File.Delete(filePath));
            }
            else
            {
                throw new FileNotFoundException($"Файл {filePath} не существует.");
            }
        }


        


    }
}
//process.OutputDataReceived += (sender, e) => Console.WriteLine($"Output: {e.Data}");
//process.ErrorDataReceived += (sender, e) => Console.WriteLine($"Error: {e.Data}");

//process.Start();
//process.BeginOutputReadLine();
//process.BeginErrorReadLine();

//await process.WaitForExitAsync();

//if (process.ExitCode != 0)
//{
//    // Если процесс завершается с кодом отличным от 0, что-то пошло не так.
//    throw new Exception($"FFmpeg process exited with code {process.ExitCode}");
//}