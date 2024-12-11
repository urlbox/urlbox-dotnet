using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK.Webhook.Resource;

namespace UrlboxSDK.MsTest.Webhook.Validator;

[TestClass]
public class UrlboxWebhookValidatorTests
{
    private Urlbox urlbox;

    [TestInitialize]
    public void TestInitialize()
    {
        urlbox = new Urlbox("key", "secret", "webhook_secret");
    }

    [TestMethod]
    public void VerifyWebhookSignature_Succeeds()
    {
        string urlboxSignature = "t=123456,sha256=41f85178517e8e031be5771ee4951bc3f6fbd871f41b4866546803576b1c3843";
        var content = "{\"event\":\"render.succeeded\",\"renderId\":\"e9617143-2a95-4962-9cc9-d72f3c413b9c\",\"result\":{\"renderUrl\":\"https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png\",\"size\":359081},\"meta\":{\"startTime\": \"2024-01-11T23:32:11.908Z\",\"endTime\":\"2024-01-11T23:33:32.500Z\"}}";
        UrlboxWebhookResponse result = urlbox.VerifyWebhookSignature(urlboxSignature, content);

        Assert.AreEqual(result.Event, "render.succeeded");
        Assert.AreEqual(result.RenderId, "e9617143-2a95-4962-9cc9-d72f3c413b9c");

        Assert.AreEqual("https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png", result.Result.RenderUrl);
        Assert.AreEqual(359081, result.Result.Size);

        Assert.AreEqual(result.Meta.StartTime, "2024-01-11T23:32:11.908Z");
        Assert.AreEqual(result.Meta.EndTime, "2024-01-11T23:33:32.500Z");
    }

    [TestMethod]
    public void VerifyWebhookSignature_FailsNoTimestamp()
    {
        string urlboxSignature = ",sha256=41f85178517e8e031be5771ee4951bc3f6fbd871f41b4866546803576b1c3843";
        var content = "{\"event\":\"render.succeeded\",\"renderId\":\"e9617143-2a95-4962-9cc9-d72f3c413b9c\",\"result\":{\"renderUrl\":\"https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png\",\"size\":359081},\"meta\":{\"startTime\": \"2024-01-11T23:32:11.908Z\",\"endTime\":\"2024-01-11T23:33:32.500Z\"}}";
        var result = Assert.ThrowsException<ArgumentException>(() => urlbox.VerifyWebhookSignature(urlboxSignature, content));
        Assert.AreEqual(result.Message, "Unable to verify signature as header is empty or malformed. Please ensure you pass the `x-urlbox-signature` from the header of the webhook response.");
    }

    [TestMethod]
    public void VerifyWebhookSignature_FailsNoSha()
    {
        string urlboxSignature = "t=123456,";
        var content = "{\"event\":\"render.succeeded\",\"renderId\":\"e9617143-2a95-4962-9cc9-d72f3c413b9c\",\"result\":{\"renderUrl\":\"https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png\",\"size\":359081},\"meta\":{\"startTime\": \"2024-01-11T23:32:11.908Z\",\"endTime\":\"2024-01-11T23:33:32.500Z\"}}";
        var result = Assert.ThrowsException<ArgumentException>(() => urlbox.VerifyWebhookSignature(urlboxSignature, content));
        Assert.AreEqual(result.Message, "Unable to verify signature as header is empty or malformed. Please ensure you pass the `x-urlbox-signature` from the header of the webhook response.");
    }

    [TestMethod]
    public void Urlbox_createsWithWebhookValidator()
    {
        Urlbox urlbox = new("key", "secret", "webhook");
        // Shar of 'content' should not match 321, but method should run if 'webhook' passed.
        var result = Assert.ThrowsException<System.Exception>(() => urlbox.VerifyWebhookSignature("t=123,sha256=321", "content"));

        Assert.AreEqual(
            "Cannot verify that this response came from Urlbox. Double check that you're webhook secret is correct.",
            result.Message
        );
    }

    [TestMethod]
    public void Urlbox_throwsWhenWithoutWebhookValidator()
    {
        Urlbox urlbox = new("key", "secret");
        // Should throw bc no webhook set so no validator instance
        var result = Assert.ThrowsException<ArgumentException>(() => urlbox.VerifyWebhookSignature("t=123,sha256=321", "content"));
        Assert.AreEqual(result.Message, "Please set your webhook secret in the Urlbox instance before calling this method.");
    }

    [TestMethod]
    public void VerifyWebhookSignature_FailsShaEmpty()
    {
        string urlboxSignature = "t=123456,sha256=";
        var content = "{\"event\":\"render.succeeded\",\"renderId\":\"e9617143-2a95-4962-9cc9-d72f3c413b9c\",\"result\":{\"renderUrl\":\"https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png\",\"size\":359081},\"meta\":{\"startTime\": \"2024-01-11T23:32:11.908Z\",\"endTime\":\"2024-01-11T23:33:32.500Z\"}}";
        var result = Assert.ThrowsException<ArgumentException>(() => urlbox.VerifyWebhookSignature(urlboxSignature, content));
        Assert.AreEqual("The signature could not be found, please ensure you are passing the x-urlbox-signature header.", result.Message);
    }

    [TestMethod]
    public void VerifyWebhookSignature_FailsTimestampEmpty()
    {
        string urlboxSignature = "t=,sha256=41f85178517e8e031be5771ee4951bc3f6fbd871f41b4866546803576b1c3843";
        var content = "{\"event\":\"render.succeeded\",\"renderId\":\"e9617143-2a95-4962-9cc9-d72f3c413b9c\",\"result\":{\"renderUrl\":\"https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png\",\"size\":359081},\"meta\":{\"startTime\": \"2024-01-11T23:32:11.908Z\",\"endTime\":\"2024-01-11T23:33:32.500Z\"}}";
        var result = Assert.ThrowsException<ArgumentException>(() => urlbox.VerifyWebhookSignature(urlboxSignature, content));
        Assert.AreEqual("The timestamp could not be found, please ensure you are passing the x-urlbox-signature header.", result.Message);
    }

    [TestMethod]
    public void VerifyWebhookSignature_FailsNoComma()
    {
        string urlboxSignature = "t=12345sha256=41f85178517e8e031be5771ee4951bc3f6fbd871f41b4866546803576b1c3843";
        var content = "{\"event\":\"render.succeeded\",\"renderId\":\"e9617143-2a95-4962-9cc9-d72f3c413b9c\",\"result\":{\"renderUrl\":\"https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png\",\"size\":359081},\"meta\":{\"startTime\": \"2024-01-11T23:32:11.908Z\",\"endTime\":\"2024-01-11T23:33:32.500Z\"}}";
        var result = Assert.ThrowsException<ArgumentException>(() => urlbox.VerifyWebhookSignature(urlboxSignature, content));
        Assert.AreEqual("Unable to verify signature as header is empty or malformed. Please ensure you pass the `x-urlbox-signature` from the header of the webhook response.", result.Message);
    }
}