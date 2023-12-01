using ChatGPT_APP.Services.Models.RequestModel;
using ChatGPT_APP.Services.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot;
using File = System.IO.File;
namespace ChatGPT_APP.Test_Conversion
{

    [TestFixture]
    public class FileCollabServiceTests
    {
        private FileCollabSerivice _fileCollabService;
        private ITelegramBotClient _telegramBotClient;
        private Mock<ILogger<FileCollabSerivice>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            // Инициализация зависимостей (например, Mock для ITelegramBotClient)
            _loggerMock = new Mock<ILogger<FileCollabSerivice>>();
            _telegramBotClient = new Mock<ITelegramBotClient>().Object;
            _fileCollabService = new FileCollabSerivice(_loggerMock.Object, _telegramBotClient);
        }

        [Test]
        public async Task GetSplitAudioFilePathByUnitOfTimeList_ShouldReturnNonEmptyList()
        {
            // Arrange
            string longVoiseID = "AwACAgIAAxkBAAIRy2VjkCbbf1s89y0jdkYUrNKR58otAAItNwACBK2QSiUKxWf7k2HhMwQ";
            int durations = 986;

            var voiceStream = await _fileCollabService.GetTelegramVoisStreamAsync(longVoiseID);

            var request = new ConversionRequest (voiceStream, durations);
            var timeInterval = 3; // set your time interval

            // Act
            var result = await _fileCollabService.GetSplitAudioFilePathByUnitOfTimeList(request, timeInterval);
           

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<Stream>>(result);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task GetTelegramVoisFilePathAsync_ShouldCreateFile()
        {
            // Arrange
            string inputFilePath = "E:\\C#_WORK\\ChatGPT_APP\\ChatGPT_APP\\SistemFiles\\InputFiles\\audioTest.ogg";
            var voiceStream = await Task.Run(() => System.IO.File.OpenRead(inputFilePath));
            int durations = 986;
            var request = new ConversionRequest (voiceStream, durations);
            

            // Act
            var filePath = await _fileCollabService.GetTelegramVoisFilePathAsync(request.VoiceFileSream, request.id);

            // Assert
            Assert.IsTrue(File.Exists(filePath));
        }

        [Test]
        public async Task GetTelegramVoisStreamAsync_ShouldReturnStream()
        {
            // Arrange
            string longVoiseID = "AwACAgIAAxkBAAIRy2VjkCbbf1s89y0jdkYUrNKR58otAAItNwACBK2QSiUKxWf7k2HhMwQ";
          //  var message = new Message { /* initialize message properties */ };

            // Act
            var result = await _fileCollabService.GetTelegramVoisStreamAsync(longVoiseID);

            // Assert
            Assert.IsNotNull(result);
           // Assert.IsInstanceOf<Stream>(result);
        }
    }
}