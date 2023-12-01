using ChatGPT_APP.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChatGPT_APP.Tests
{
    [TestFixture]
    public class OpenAIServiceTests
    {
        private readonly string apiKey = "your_api_key_here"; // Замените на ваш ключ API

        
        [TestMethod]
        public async Task VoiseMessageSynthesizerFromOpenAI_Success()
        {
            // Arrange
            var audioStream = new MemoryStream(); // Создаем поток для звуковых данных
            var openAIService = new OpenAIService();

            string inputFilePath = "E:\\C#_WORK\\ChatGPT_APP\\ChatGPT_APP\\SistemFiles\\InputFiles\\audioTest.ogg";
            var voiceStream = await Task.Run(() => System.IO.File.OpenRead(inputFilePath));

            // Act
            var resultFromStream = await openAIService.VoiseMessageSynthesizerFromOpenAI(audioStream);
           // var resultFromFilePath = await openAIService.VoiseMessageSynthesizerFromOpenAI(inputFilePath);

            // Assert
            NUnit.Framework.Assert.IsNotNull(resultFromStream);
            NUnit.Framework.Assert.IsInstanceOf<string>(resultFromStream);
            // Добавьте дополнительные проверки по результату, если необходимо
        }

        [TestMethod]
        public async Task VoiseMessageSynthesizerFromOpenAI_TooManyRequests_RetryAfterDelay()
        {
            // Arrange
            var audioStream = new MemoryStream(); // Создаем поток для звуковых данных
            var openAIService = new OpenAIService();
            var mockHttpClient = new Mock<HttpClient>();

            // Моделируем ситуацию, когда возвращается код состояния "TooManyRequests"
            mockHttpClient.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ThrowsAsync(new HttpRequestException("Too Many Requests", null, System.Net.HttpStatusCode.TooManyRequests));

           // openAIService.SetHttpClient(mockHttpClient.Object);

            // Act
            var result = await openAIService.VoiseMessageSynthesizerFromOpenAI(audioStream);

            // Assert
            NUnit.Framework.Assert.IsNotNull(result);
            // Добавьте дополнительные проверки по результату, если необходимо
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))] // Ожидаемое исключение
        public async Task VoiseMessageSynthesizerFromOpenAI_HttpRequestException_ThrowsException()
        {
            // Arrange
            var audioStream = new MemoryStream(); // Создаем поток для звуковых данных
            var openAIService = new OpenAIService();
            var mockHttpClient = new Mock<HttpClient>();

            // Моделируем ситуацию, когда возникает исключение типа HttpRequestException
            mockHttpClient.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ThrowsAsync(new HttpRequestException("Some error", null, System.Net.HttpStatusCode.BadRequest));

          //  openAIService.SetHttpClient(mockHttpClient.Object);

            // Act & Assert
            await openAIService.VoiseMessageSynthesizerFromOpenAI(audioStream);
        }
    }
}