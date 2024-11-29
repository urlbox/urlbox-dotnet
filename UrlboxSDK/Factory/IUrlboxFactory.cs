using UrlboxSDK.Resource;

namespace UrlboxSDK.Factory;

public interface IUrlboxFactory
{
    IUrlbox Create(string key, string secret, string? webhookSecret = null);
}

public class UrlboxFactory : IUrlboxFactory
{
    public IUrlbox Create(string key, string secret, string? webhookSecret = null)
    {
        return new Urlbox(key, secret, webhookSecret);
    }
}
