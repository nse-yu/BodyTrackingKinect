using KinectDemo2.Custom.Service;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace KinectDemoTests
{
    public class DBServiceTests
    {
        class Tracking
        {
            private string? userId;
            private float positionX;
            private float positionY;
            private float positionZ;
            private string? model;

            public Tracking(string? userId, float positionX, float positionY, float positionZ, string? model)
            {
                this.userId = userId;
                this.positionX = positionX;
                this.positionY = positionY;
                this.positionZ = positionZ;
                this.model = model;
            }
        }

        [Fact(DisplayName = "HTTP GET: no record")]
        public async Task Get_Empty()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            // target settings
            mockHttpMessageHandler
                // option
                .Protected()
                // target method
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                // returned value
                .ReturnsAsync(() =>
                {
                    var res = new HttpResponseMessage()
                    {
                        Content = new StringContent("[]", Encoding.UTF8, "application/json"),
                        StatusCode = HttpStatusCode.OK  
                    };
                    return res;
                });

            var dbService = new DBService(new HttpClient(mockHttpMessageHandler.Object));
           
            // Act
            var result = await dbService.GetAsync(123);

            // Assert
            Assert.Empty(result);
        }

        [Fact(DisplayName = "HTTP GET: some records")]
        public async Task Get_Any()
        {
            var trackingList = new List<Tracking>()
            {
                new Tracking(userId: "yu", positionX: .5f, positionY: .2f, positionZ: .9f, model: "lightning"),
                new Tracking(userId: "yu", positionX: .545f, positionY: .32f, positionZ: .19f, model: "mediapipe"),
            };
            var jsonString = JsonSerializer.Serialize(trackingList);

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            // target settings
            mockHttpMessageHandler
                // option
                .Protected()
                // target method
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                // returned value
                .ReturnsAsync(() =>
                {
                    var res = new HttpResponseMessage()
                    {
                        Content = new StringContent(jsonString, Encoding.UTF8, "application/json"),
                        StatusCode = HttpStatusCode.OK
                    };
                    return res;
                });

            var dbService = new DBService(new HttpClient(mockHttpMessageHandler.Object));

            // Act
            var result = await dbService.GetAsync(123);

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact(DisplayName = "HTTP POST: save implementation", Skip = "Because of unimplementation")]
        public async Task Post_Any()
        {
           throw new NotImplementedException();
        }
    }
}
