[![NuGet Version](https://img.shields.io/nuget/v/Custom.Hangfire.Dashboard.RestrictIpAuthorization.svg)](https://www.nuget.org/packages/Custom.Hangfire.Dashboard.RestrictIpAuthorization/)
[![Github Action Status](https://img.shields.io/github/actions/workflow/status/markusblasek/Hangfire.Dashboard.RestrictIpAuthorization/dotnet.yml)](https://github.com/markusblasek/Hangfire.Dashboard.RestrictIpAuthorization/actions/workflows/dotnet.yml)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Custom.Hangfire.Dashboard.RestrictIpAuthorization.svg)](https://www.nuget.org/packages/Custom.Hangfire.Dashboard.RestrictIpAuthorization/)
[![Coverage Status](https://coveralls.io/repos/github/markusblasek/Hangfire.Dashboard.RestrictIpAuthorization/badge.svg?branch=main)](https://coveralls.io/github/markusblasek/Hangfire.Dashboard.RestrictIpAuthorization?branch=main)

# Custom.Hangfire.Dashboard.RestrictIpAuthorization

Restrict IP addresses to access hangfire (e.g. dashboard)

## Installation

```
dotnet add package Custom.Hangfire.Dashboard.RestrictIpAuthorization
```

## Usage

```cs
var restrictIpAuthorizationFilter = new RestrictIpAuthorizationFilter(
    new RestrictIpAuthorizationFilterOptions
    {
        AllowedIps = ["::1", "localhost", "10.27.147.54"]
    });
app.UseHangfireDashboard(options: new DashboardOptions
{
    Authorization = [restrictIpAuthorizationFilter]
});
```

## License

The MIT License. See the [license](https://github.com/markusblasek/Hangfire.Dashboard.RestrictIpAuthorization/blob/main/LICENSE) file for details.

