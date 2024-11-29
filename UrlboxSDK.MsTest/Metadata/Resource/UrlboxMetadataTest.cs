using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK.Metadata.Resource;

[TestClass]
public class UrlboxMetadataTests
{
    [TestMethod]
    public void UrlboxMetadata_CreatesGettersMin()
    {
        string url = "url";
        string urlRequested = "urlRequested";
        string urlResolved = "urlResolved";

        UrlboxMetadata urlboxMetadata = new UrlboxMetadata(
            url: url,
            urlRequested: urlRequested,
            urlResolved: urlResolved
        );

        Assert.IsInstanceOfType(urlboxMetadata, typeof(UrlboxMetadata));
        Assert.AreEqual(url, urlboxMetadata.Url);
        Assert.AreEqual(urlRequested, urlboxMetadata.UrlRequested);
        Assert.AreEqual(urlResolved, urlboxMetadata.UrlResolved);

        Assert.IsNull(urlboxMetadata.Author);
        Assert.IsNull(urlboxMetadata.Date);
        Assert.IsNull(urlboxMetadata.Description);
        Assert.IsNull(urlboxMetadata.Image);
        Assert.IsNull(urlboxMetadata.Logo);
        Assert.IsNull(urlboxMetadata.Publisher);
        Assert.IsNull(urlboxMetadata.Title);
        Assert.IsNull(urlboxMetadata.OgTitle);
        Assert.IsNull(urlboxMetadata.OgImage);
        Assert.IsNull(urlboxMetadata.OgDescription);
        Assert.IsNull(urlboxMetadata.OgUrl);
        Assert.IsNull(urlboxMetadata.OgType);
        Assert.IsNull(urlboxMetadata.OgSiteName);
        Assert.IsNull(urlboxMetadata.OgImage);
        Assert.IsNull(urlboxMetadata.OgLocale);
        Assert.IsNull(urlboxMetadata.Charset);
        Assert.IsNull(urlboxMetadata.TwitterCard);
        Assert.IsNull(urlboxMetadata.TwitterSite);
        Assert.IsNull(urlboxMetadata.TwitterCreator);
    }

    [TestMethod]
    public void UrlboxMetadata_CreatesGettersAll()
    {
        OgImage ogImage = new OgImage(
            url: "url",
            type: "type",
            width: "123",
            height: "123"
        );

        string author = "author";
        string date = "date";
        string description = "description";
        string image = "image";
        string logo = "logo";
        string publisher = "publisher";
        string title = "title";
        string url = "url";
        string ogTitle = "ogTitle";
        OgImage[] ogImages = new OgImage[] { ogImage, ogImage };
        string ogDescription = "ogDescription";
        string ogUrl = "ogUrl";
        string ogType = "ogType";
        string ogSiteName = "ogSiteName";
        string ogLocale = "ogLocale";
        string charset = "charset";
        string urlRequested = "urlRequested";
        string urlResolved = "urlResolved";
        string twitterCard = "twitterCard";
        string twitterSite = "twitterSite";
        string twitterCreator = "twitterCreator";

        UrlboxMetadata urlboxMetadata = new UrlboxMetadata(
            author: author,
            date: date,
            description: description,
            image: image,
            logo: logo,
            publisher: publisher,
            title: title,
            url: url,
            ogTitle: ogTitle,
            ogImage: ogImages,
            ogDescription: ogDescription,
            ogUrl: ogUrl,
            ogType: ogType,
            ogSiteName: ogSiteName,
            ogLocale: ogLocale,
            charset: charset,
            urlRequested: urlRequested,
            urlResolved: urlResolved,
            twitterCard: twitterCard,
            twitterSite: twitterSite,
            twitterCreator: twitterCreator
        );

        Assert.IsInstanceOfType(urlboxMetadata, typeof(UrlboxMetadata));
        Assert.AreEqual(author, urlboxMetadata.Author);
        Assert.AreEqual(date, urlboxMetadata.Date);
        Assert.AreEqual(description, urlboxMetadata.Description);
        Assert.AreEqual(image, urlboxMetadata.Image);
        Assert.AreEqual(logo, urlboxMetadata.Logo);
        Assert.AreEqual(publisher, urlboxMetadata.Publisher);
        Assert.AreEqual(title, urlboxMetadata.Title);
        Assert.AreEqual(url, urlboxMetadata.Url);
        Assert.AreEqual(ogTitle, urlboxMetadata.OgTitle);
        Assert.AreEqual(ogImages, urlboxMetadata.OgImage);
        Assert.AreEqual(ogDescription, urlboxMetadata.OgDescription);
        Assert.AreEqual(ogUrl, urlboxMetadata.OgUrl);
        Assert.AreEqual(ogType, urlboxMetadata.OgType);
        Assert.AreEqual(ogSiteName, urlboxMetadata.OgSiteName);
        Assert.AreEqual(ogLocale, urlboxMetadata.OgLocale);
        Assert.AreEqual(charset, urlboxMetadata.Charset);
        Assert.AreEqual(urlRequested, urlboxMetadata.UrlRequested);
        Assert.AreEqual(urlResolved, urlboxMetadata.UrlResolved);
        Assert.AreEqual(twitterCard, urlboxMetadata.TwitterCard);
        Assert.AreEqual(twitterSite, urlboxMetadata.TwitterSite);
        Assert.AreEqual(twitterCreator, urlboxMetadata.TwitterCreator);
    }
}
