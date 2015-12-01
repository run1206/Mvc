using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class SimpleTests : IClassFixture<MvcTestFixture<SimpleWebSite.Startup>>
    {
        public SimpleTests(MvcTestFixture<SimpleWebSite.Startup> fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task JsonSerializeFormated()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "http://localhost/Home/Dict");

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var expected = "{" + Environment.NewLine +
                "  \"first\": \"wall\"," + Environment.NewLine +
                "  \"second\": \"floor\"" + Environment.NewLine +
                "}";

            Assert.Equal(expected, content);
        }
    }
}
