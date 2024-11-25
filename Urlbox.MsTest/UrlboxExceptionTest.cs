using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK;

[TestClass]
public class UrlboxExceptionTests
{
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    [TestMethod]
    public void FromResponse_ValidResponse_ParsesSuccessfully()
    {
        var jsonResponse = @"
        {
            ""error"": {
                ""message"": ""Invalid options, please check errors - {\""url\"":[\""error resolving URL - ENOTFOUND ffffffffffftest-site.urlbox.com\""]}"",
                ""code"": ""InvalidOptions"",
                ""errors"": ""{\""url\"":[\""error resolving URL - ENOTFOUND ffffffffffftest-site.urlbox.com\""]}""
            },
            ""requestId"": ""5490b293-29b7-43e6-b9f0-7ea23c6a1259""
        }";

        var exception = Assert.ThrowsException<UrlboxException>(() => UrlboxException.FromResponse(jsonResponse, _serializerOptions));

        Assert.AreEqual("Invalid options, please check errors - {\"url\":[\"error resolving URL - ENOTFOUND ffffffffffftest-site.urlbox.com\"]}", exception.Message);
        Assert.AreEqual("InvalidOptions", exception.Code);
        Assert.AreEqual("{\"url\":[\"error resolving URL - ENOTFOUND ffffffffffftest-site.urlbox.com\"]}", exception.Errors);
        Assert.AreEqual("5490b293-29b7-43e6-b9f0-7ea23c6a1259", exception.RequestId);
    }

    [TestMethod]
    public void FromResponse_ResponseWithMissingCodeAndErrors_ParsesSuccessfully()
    {
        var jsonResponse = @"
        {
            ""error"": {
                ""message"": ""Invalid options, please check errors"",
                ""code"": """",
                ""errors"": """"
            },
            ""requestId"": ""5490b293-29b7-43e6-b9f0-7ea23c6a1259""
        }";

        var exception = Assert.ThrowsException<UrlboxException>(() => UrlboxException.FromResponse(jsonResponse, _serializerOptions));

        Assert.AreEqual("Invalid options, please check errors", exception.Message);
        Assert.IsNull(exception.Code);
        Assert.IsNull(exception.Errors);
        Assert.AreEqual("5490b293-29b7-43e6-b9f0-7ea23c6a1259", exception.RequestId);
    }

    [TestMethod]
    public void FromResponse_InvalidJson_ThrowsException()
    {
        var invalidJson = @"{ ""invalid"": ""json"" }";

        var exception = Assert.ThrowsException<JsonException>(() => UrlboxException.FromResponse(invalidJson, _serializerOptions));

        Assert.AreEqual("Invalid JSON response structure", exception.Message);
        Assert.IsInstanceOfType(exception, typeof(JsonException));
    }

    [TestMethod]
    public void FromResponse_NullOrEmptyResponse_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => UrlboxException.FromResponse(null, _serializerOptions));
        Assert.ThrowsException<ArgumentException>(() => UrlboxException.FromResponse(string.Empty, _serializerOptions));
    }

    [TestMethod]
    public void FromResponse_ResponseWithMissingRequestId_ThrowsJsonException()
    {
        var jsonResponse = @"
        {
            ""error"": {
                ""message"": ""Invalid options, please check errors"",
                ""code"": ""InvalidOptions"",
                ""errors"": ""{\""url\"":[\""error resolving URL - ENOTFOUND ffffffffffftest-site.urlbox.com\""]}""
            }
        }";

        var exception = Assert.ThrowsException<JsonException>(() => UrlboxException.FromResponse(jsonResponse, _serializerOptions));

        Assert.AreEqual("Invalid JSON response structure", exception.Message);
    }

    [TestMethod]
    public void FromResponse_ResponseWithMissingError_ThrowsJsonException()
    {
        var jsonResponse = @"
        {
            ""requestId"": ""5490b293-29b7-43e6-b9f0-7ea23c6a1259""
        }";

        var exception = Assert.ThrowsException<JsonException>(() => UrlboxException.FromResponse(jsonResponse, _serializerOptions));

        Assert.AreEqual("Invalid JSON response structure", exception.Message);
    }
}