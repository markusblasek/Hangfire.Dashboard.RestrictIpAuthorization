using System.Net;

namespace Custom.Hangfire.Dashboard.RestrictIpAuthorization.Tests;

[TestClass]
public class RestrictIpAuthorizationFilterTests
{
    private const string AllowedIpAddress = "10.45.83.100";
    private const string AllowedIpAddressIpv4MappedToIpv6 = $"::ffff:{AllowedIpAddress}";

    [TestMethod]
    [DataRow("blafasel")]
    public void Constructor_InvalidIPAddressInOptions_ThrowException(string ipAddress)
    {
        var options = CreateValidOptions();
        options.AllowedIps = [ipAddress];
        try
        {
            CreateSut(options);
            Assert.Fail("An exception should be thrown");
        }
        catch (ArgumentException ae)
        {
            Assert.AreEqual($"'{ipAddress}' is not a valid ip address", ae.Message);
        }
    }

    [TestMethod]
    [DataRow("127.0.0.1")]
    [DataRow("::1")]
    [DataRow(AllowedIpAddress)]
    [DataRow(AllowedIpAddressIpv4MappedToIpv6)]
    public void IsIpAllowed_RemoteIpIsAllowed_true(string remoteIp)
    {
        var options = CreateValidOptions();
        var sut = CreateSut(options);
        var remoteIpAddress = IPAddress.Parse(remoteIp);
        var result = sut.IsIpAllowed(remoteIpAddress);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsIpAllowed_RemoteIpIsNull_false()
    {
        var options = CreateValidOptions();
        var sut = CreateSut(options);
        var result = sut.IsIpAllowed(null);
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow("194.144.1.47")]
    public void IsIpAllowed_RemoteIpIsNotAllowed_false(string remoteIp)
    {
        var options = CreateValidOptions();
        var sut = CreateSut(options);
        var remoteIpAddress = IPAddress.Parse(remoteIp);
        var result = sut.IsIpAllowed(remoteIpAddress);
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow("194.144.1.47")]
    public void IsIpAllowed_NoAllowedIpsValue_AlwaysTrue(string remoteIp)
    {
        var options = CreateValidOptions();
        options.AllowedIps = [];
        var sut = CreateSut(options);
        var remoteIpAddress = IPAddress.Parse(remoteIp);
        var result = sut.IsIpAllowed(remoteIpAddress);
        Assert.IsTrue(result);
    }

    private static RestrictIpAuthorizationFilter CreateSut(RestrictIpAuthorizationFilterOptions options)
    {
        return new RestrictIpAuthorizationFilter(options);
    }

    private static RestrictIpAuthorizationFilterOptions CreateValidOptions()
    {
        return new RestrictIpAuthorizationFilterOptions
        {
            AllowedIps = [
                RestrictIpAuthorizationFilter.HostnameLocalhost,
                AllowedIpAddress
            ]
        };
    }
}