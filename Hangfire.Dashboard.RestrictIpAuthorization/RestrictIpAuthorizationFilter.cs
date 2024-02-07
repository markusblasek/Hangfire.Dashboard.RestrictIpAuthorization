using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hangfire.Dashboard.RestrictIPAuthorization;

public class RestrictIpAuthorizationFilter : IDashboardAuthorizationFilter
{
    internal const string HostnameLocalhost = "localhost";

    private readonly ILogger<RestrictIpAuthorizationFilter> _logger;
    private readonly HashSet<byte[]> _safeList = [];

    public RestrictIpAuthorizationFilter(RestrictIpAuthorizationFilterOptions options) : this(options,
        new NullLogger<RestrictIpAuthorizationFilter>())
    {
    }

    public RestrictIpAuthorizationFilter(RestrictIpAuthorizationFilterOptions options, ILogger<RestrictIpAuthorizationFilter> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
        ValidateOptions(options);
        SetSafeList(options);
    }

    private static void ValidateOptions(RestrictIpAuthorizationFilterOptions options)
    {
        foreach (var ip in options.AllowedIps.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()))
        {
            if (ip.Equals(HostnameLocalhost, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!IPAddress.TryParse(ip, out _))
            {
                throw new ArgumentException($"'{ip}' is not a valid ip address");
            }
        }
    }

    private void SetSafeList(RestrictIpAuthorizationFilterOptions options)
    {
        var validIps = GetValidIps(options.AllowedIps);
        if (validIps.Length == 0)
        {
            return;
        }

        foreach (var ip in validIps)
        {
            if (IPAddress.TryParse(ip, out var parsedAddress))
            {
                _safeList.Add(parsedAddress.GetAddressBytes());
            }
            else if (ip.Equals(HostnameLocalhost, StringComparison.OrdinalIgnoreCase))
            {
                _safeList.Add(IPAddress.Loopback.GetAddressBytes());
                _safeList.Add(IPAddress.IPv6Loopback.GetAddressBytes());
            }
        }
    }

    private static string[] GetValidIps(IEnumerable<string> ips)
    {
        return ips
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToLowerInvariant())
            .Where(x => x.Equals(HostnameLocalhost, StringComparison.OrdinalIgnoreCase) || IPAddress.TryParse(x, out _))
            .Distinct().ToArray();
    }

    [ExcludeFromCodeCoverage]
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var remoteIp = httpContext.Connection.RemoteIpAddress;
        return IsIpAllowed(remoteIp);
    }

    internal bool IsIpAllowed(IPAddress? remoteIp)
    {
        if (_safeList.Count == 0)
        {
            return true;
        }
        if (remoteIp == null)
        {
            _logger.LogTrace("Remote ip could not be detected - access to hangfire is not allowed");
            return false;
        }

        if (remoteIp.IsIPv4MappedToIPv6)
        {
            remoteIp = remoteIp.MapToIPv4();
        }

        var remoteIpBytes = remoteIp.GetAddressBytes();
        var isAllowedIp = _safeList.Any(x => x.SequenceEqual(remoteIpBytes));

        if (isAllowedIp)
        {
            _logger.LogTrace("Remote ip '{remoteIp}' is allowed to access hangfire", remoteIp);
            return true;
        }
        _logger.LogDebug("Remote ip '{remoteIp}' is not allowed to access hangfire", remoteIp);
        return false;
    }
}

