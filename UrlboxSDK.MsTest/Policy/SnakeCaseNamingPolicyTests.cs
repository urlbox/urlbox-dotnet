using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK.Policy;

namespace UrlboxSDK.MsTest.Policy;

[TestClass]
public class SnakeCaseNamingPolicyTests
{
    [TestMethod]
    public void ConvertName_ShouldConvertPascalCaseToSnakeCase()
    {
        var namingPolicy = new SnakeCaseNamingPolicy();

        Assert.AreEqual("fail_on_4xx", namingPolicy.ConvertName("FailOn4xx"));
        Assert.AreEqual("fail_on_400", namingPolicy.ConvertName("FailOn400"));
        Assert.AreEqual("fail_on_5xx", namingPolicy.ConvertName("FailOn5xx"));
        Assert.AreEqual("fail_on_500", namingPolicy.ConvertName("FailOn500"));
        Assert.AreEqual("error_500_x", namingPolicy.ConvertName("Error500X"));
        Assert.AreEqual("test_4xx_code", namingPolicy.ConvertName("Test4xxCode"));
        Assert.AreEqual("full_page", namingPolicy.ConvertName("FullPage"));
    }

    [TestMethod]
    public void ConvertName_ShouldHandleSingleWordInputs()
    {
        var namingPolicy = new SnakeCaseNamingPolicy();

        Assert.AreEqual("example", namingPolicy.ConvertName("Example"));
        Assert.AreEqual("test", namingPolicy.ConvertName("Test"));
    }

    [TestMethod]
    public void ConvertName_ShouldPreserveAlreadySnakeCaseNames()
    {
        var namingPolicy = new SnakeCaseNamingPolicy();

        Assert.AreEqual("already_snake_case", namingPolicy.ConvertName("already_snake_case"));
        Assert.AreEqual("test_4xx", namingPolicy.ConvertName("test_4xx"));
    }
}