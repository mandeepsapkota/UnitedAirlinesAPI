using Moq;
using UnitedAirlinesAPI.Infrastructure;
using UnitedAirlinesAPI.Services;
using Microsoft.Extensions.Options;
using UnitedAirlinesAPI.Models;
using Moq.Protected;
using Newtonsoft.Json;

namespace UnitedAirlinesAPI.UnitTests.Services
{
    public class CargoServiceUnitTest : UnitTestBase
    {
        private CargoService _cargoService;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
        private readonly IOptions<SettingConfig> sampleOption =
            Options.Create(
                new SettingConfig
                {
                    APIKey = "PMAK-62a806dc4e8722210079384a-69529bed3436dcc9746dbc7a37ed647b44",
                    MockUrl = "https://www.testurl.com/baseaddress/"
                });

        private readonly HttpResponseMessage httpResonseMessage = new HttpResponseMessage
        {
            Content = new StringContent(""),
            StatusCode = System.Net.HttpStatusCode.OK,
            RequestMessage = new HttpRequestMessage
            {
                Content = new StringContent(""),
                RequestUri = new Uri(@"https://www.testurl.com/baseaddress/")
            }
        };

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                   ItExpr.IsAny<HttpRequestMessage>(),
                                                   ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResonseMessage);

            var client = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientFactoryMock
                .Setup(_ => _.CreateClient(It.IsAny<string>()))
                .Returns(client).Verifiable();

            _cargoService = new CargoService(sampleOption, _httpClientFactoryMock.Object);
        }

        [Test]
        public void GetCargoManifestByAirportCodeFlightAndDate()
        {
            //Arrange
            CargoRequest request = new()
            {
                AirportCode = "PDX",
                FlightDate = "AS-7095-01Jan2023"
            };

            var objectValues = new Dictionary<string, object>
            {
                { "flight", "UDP001" },
                { "airport", "CHICAGO" },
                { "segments", Array.Empty<string>() }
            };
            var json = JsonConvert.SerializeObject(objectValues, Formatting.Indented);
            httpResonseMessage.Content = new StringContent(json);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                   ItExpr.IsAny<HttpRequestMessage>(),
                                                   ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResonseMessage);

            var client = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientFactoryMock
                .Setup(_ => _.CreateClient(It.IsAny<string>()))
                .Returns(client).Verifiable();

            _cargoService = new CargoService(sampleOption, _httpClientFactoryMock.Object);

            //Act
            var response = _cargoService.GetCargoManifestByAirportCodeFlightAndDate(request);

            //Assert
            Assert.That(response, Is.Not.Null);
        }
    }
}
