using System.Diagnostics.CodeAnalysis;

namespace Custom.Hangfire.Dashboard.RestrictIpAuthorization;

[ExcludeFromCodeCoverage]
public class RestrictIpAuthorizationFilterOptions
{
    public string[] AllowedIps { get; set; } = [];
}
