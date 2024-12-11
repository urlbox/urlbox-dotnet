using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

#nullable enable

namespace UrlboxSDK.MsTest.Utils;

public class MockHttpClientFixture
{
    public Mock<HttpMessageHandler> MockHandler { get; }
    public HttpClient HttpClient { get; }

    public MockHttpClientFixture()
    {
        // Create a mock of HttpMessageHandler
        MockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        // Create a real HttpClient using the mock
        HttpClient = new HttpClient(MockHandler.Object);
    }

    /// <summary>
    /// Sets up a mocked HTTP request using the specified method, URL, status code, and response content.
    /// This method configures the mock HTTP client to return a predefined response when a matching request is sent.
    /// </summary>
    /// <param name="method">The HTTP method to match (e.g., GET, POST).</param>
    /// <param name="url">The exact request URL to match.</param>
    /// <param name="status">The HTTP status code to return (e.g., 200, 404).</param>
    /// <param name="responseContent">The response content to return as the HTTP body.</param>
    /// <param name="headers">
    /// Optional dictionary of headers to include in the HTTP response. 
    /// Keys are header names, and values are header values.
    /// </param>
    public void StubRequest(HttpMethod method, string url, HttpStatusCode status, string responseContent, Dictionary<string, string>? headers = null)
    {
        var response = new HttpResponseMessage(status)
        {
            Content = new StringContent(responseContent)
        };

        // Add headers if provided
        if (headers != null)
        {
            foreach (var header in headers)
            {
                response.Headers.Add(header.Key, header.Value);
            }
        }

        MockHandler.Protected()
            // Setup the protected SendAsync method of HttpMessageHandler to simulate an HTTP request
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", // The protected method being mocked
                ItExpr.Is<HttpRequestMessage>(req =>
                    // Match the request based on HTTP method and exact request URL
                    req.Method == method && req.RequestUri != null && req.RequestUri.ToString() == url),
                ItExpr.IsAny<CancellationToken>() // Accept any cancellation token
            )
            // Return a pre-defined HttpResponseMessage when the request matches the conditions
            .ReturnsAsync(response);
    }
}