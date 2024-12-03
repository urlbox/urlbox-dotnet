using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK.Response.Resource;
using UrlboxSDK.Webhook.Resource;

[TestClass]
public class UrlboxWebhookResponseTests
{
    [TestMethod]
    public void WebhookMeta_creates()
    {
        Meta meta = new(startTime: "START", endTime: "END");
        Assert.IsInstanceOfType(meta, typeof(Meta));
        Assert.AreEqual("START", meta.StartTime);
        Assert.AreEqual("END", meta.EndTime);
    }

    [TestMethod]
    public void UrlboxWebhookResponse_CreatesMinGetters()
    {
        SyncUrlboxResponse response = new(
            renderUrl: "https://urlbox.com",
            size: 12345
        );

        Meta meta = new(startTime: "START", endTime: "END");

        UrlboxWebhookResponse webhookResponse = new(
            @event: "render.succeeded",
            renderId: "renderId",
            result: response,
            meta: meta
        );

        Assert.IsInstanceOfType(webhookResponse, typeof(UrlboxWebhookResponse));
        Assert.IsInstanceOfType(webhookResponse.Result, typeof(SyncUrlboxResponse));
        Assert.AreEqual("render.succeeded", webhookResponse.Event);
        Assert.AreSame(response, webhookResponse.Result);
        Assert.AreSame(meta, webhookResponse.Meta);
    }

    [TestMethod]
    public void UrlboxWebhookResponse_CreatesMinGettersWithError()
    {
        ErrorUrlboxResponse.UrlboxError error = new(message: "message", code: null, errors: null);
        Meta meta = new(startTime: "START", endTime: "END");

        UrlboxWebhookResponse webhookResponse = new(
            @event: "render.succeeded",
            renderId: "renderId",
            error: error,
            meta: meta
        );

        Assert.IsInstanceOfType(webhookResponse, typeof(UrlboxWebhookResponse));
        Assert.AreEqual("render.succeeded", webhookResponse.Event);
        Assert.AreSame(error, webhookResponse.Error);
        Assert.AreSame(meta, webhookResponse.Meta);
    }
}
