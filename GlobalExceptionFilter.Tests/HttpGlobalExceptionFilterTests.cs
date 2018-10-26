using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;

using Newtonsoft.Json;

using NUnit.Framework;

namespace GlobalExceptionFilter.Tests
{
    [TestFixture]
    public class HttpGlobalExceptionFilterTests
    {
        private HttpClient _client { get; }

        public HttpGlobalExceptionFilterTests()
        {
            IWebHostBuilder builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>();

            var testServer = new TestServer(builder);
            _client = testServer.CreateClient();
        }

        [Test]
        public async Task Endpoint_ReturnSameStatusCode_ProblemDetailsWithStatusCode()
        {
            HttpResponseMessage apiResponse = await _client.GetAsync("/api/values/v1");

            string content = await apiResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ProblemDetails>(content);

            apiResponse.StatusCode.Should().Be(response.Status);
        }

        [Test]
        public async Task Endpoint_ReturnBadRequest_ProblemDetailsWithEmptyStatusCode()
        {
            HttpResponseMessage apiResponse = await _client.GetAsync("/api/values/v2");

            string content = await apiResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ProblemDetails>(content);

            apiResponse.StatusCode.Should().Be(response.Status);
        }

        [Test]
        public async Task Endpoint_ReturnErrorRequestSameData_ProblemDetailsNotEmpty()
        {
            HttpResponseMessage apiResponse = await _client.GetAsync("/api/values/v3");

            string content = await apiResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ProblemDetails>(content);

            apiResponse.StatusCode.Should().Be(response.Status);
            response.Status.Should().Be(response.Status);
            response.Detail.Should().Be(response.Detail);
            response.Title.Should().Be(response.Title);
            response.Instance.Should().Be("/api/values/v3");
        }

        [Test]
        public async Task Endpoint_ReturnInternalServerErrorResponse_UnhandledExceptionThrown()
        {
            HttpResponseMessage apiResponse = await _client.GetAsync("/api/values/v4");

            await apiResponse.Content.ReadAsStringAsync();

            apiResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task Endpoint_ReturnInternalServerErrorResponse_CustomApiExceptionWasThrownsWithInternalServerError()
        {
            HttpResponseMessage apiResponse = await _client.GetAsync("/api/values/v7");

            await apiResponse.Content.ReadAsStringAsync();

            apiResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
