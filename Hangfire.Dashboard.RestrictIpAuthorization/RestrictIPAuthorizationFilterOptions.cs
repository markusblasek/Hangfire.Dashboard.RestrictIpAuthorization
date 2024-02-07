using System.Diagnostics.CodeAnalysis;

namespace Hangfire.Dashboard.RestrictIPAuthorization;

[ExcludeFromCodeCoverage]
public class RestrictIpAuthorizationFilterOptions
{
    public string[] AllowedIps { get; set; } = [];
}
