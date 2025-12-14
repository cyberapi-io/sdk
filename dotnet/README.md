# CyberAPI .NET SDK

Official .NET client for [CyberAPI](https://threats.cyberapi.io) Threat Intelligence API.

## Installation

### Via NuGet Package Manager

```bash
dotnet add package CyberAPI
```

### Via Package Manager Console

```powershell
Install-Package CyberAPI
```

### Manual Installation

Clone the repository and reference the project:

```bash
git clone https://github.com/cyberapi-io/sdk.git
cd sdk/dotnet
dotnet add reference CyberAPI/CyberAPI.csproj
```

## Quick Start

```csharp
using CyberAPI;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        using var client = new CyberAPIClient("your_api_key_here");
        
        // Check a domain
        var result = await client.CheckAsync(domain: "example.com");
        
        Console.WriteLine($"Risk Score: {result.RiskScore}");
        Console.WriteLine($"Verdict: {result.Verdict}");
    }
}
```

## Usage Examples

### Basic Domain Check

```csharp
using var client = new CyberAPIClient(apiKey: Environment.GetEnvironmentVariable("CYBERAPI_KEY"));

var result = await client.CheckAsync(domain: "suspicious-site.com");

Console.WriteLine($"Risk Score: {result.RiskScore}/100");
Console.WriteLine($"Verdict: {result.Verdict}");
Console.WriteLine($"Cached: {result.Cached}");

if (result.Signals != null && result.Signals.Count > 0)
{
    Console.WriteLine("\nThreat Signals:");
    foreach (var signal in result.Signals)
    {
        Console.WriteLine($"  ⚠️  {signal}");
    }
}
```

### Quick Malicious Check

```csharp
using var client = new CyberAPIClient("your_api_key");

bool isMalicious = await client.IsMaliciousAsync(domain: "phishing-site.com");

if (isMalicious)
{
    Console.WriteLine("⚠️  Warning: Malicious domain detected!");
}
else
{
    Console.WriteLine("✅ Domain appears safe");
}
```

### Get Threat Signals

```csharp
var signals = await client.GetSignalsAsync(domain: "example.com");

Console.WriteLine($"Detected {signals.Count} threat signals:");
foreach (var signal in signals)
{
    Console.WriteLine($"  - {signal}");
}
```

### IP Geolocation

```csharp
var result = await client.CheckAsync(ip: "8.8.8.8");
var geo = result.GeoLocation;

Console.WriteLine($"IP: {geo.Ip}");
Console.WriteLine($"Country: {geo.Country}");
Console.WriteLine($"City: {geo.City}");
Console.WriteLine($"ISP: {geo.Isp}");
Console.WriteLine($"Risk Score: {result.RiskScore}/100");
```

### Check Both Domain and IP

```csharp
var result = await client.CheckAsync(
    domain: "example.com",
    ip: "93.184.216.34"
);

Console.WriteLine($"Domain: {result.Domain}");
Console.WriteLine($"IP: {result.Ip}");
Console.WriteLine($"Combined Risk Score: {result.RiskScore}");
```

## Error Handling

The SDK provides comprehensive error handling with `CyberAPIException`:

```csharp
try
{
    var result = await client.CheckAsync(domain: "example.com");
    Console.WriteLine($"Risk Score: {result.RiskScore}");
}
catch (CyberAPIException ex) when (ex.StatusCode == 429)
{
    // Rate limit exceeded
    Console.WriteLine("Rate limit exceeded. Please wait before retrying.");
    await Task.Delay(TimeSpan.FromMinutes(1));
}
catch (CyberAPIException ex) when (ex.StatusCode == 403)
{
    // Invalid API key
    Console.WriteLine("Invalid API key. Please check your credentials.");
}
catch (CyberAPIException ex)
{
    // Other API errors
    Console.WriteLine($"API Error: {ex.Message} (HTTP {ex.StatusCode})");
}
catch (Exception ex)
{
    // Network or other errors
    Console.WriteLine($"Error: {ex.Message}");
}
```

### Retry Logic with Polly

```csharp
using Polly;
using Polly.Retry;

var retryPolicy = Policy
    .Handle<CyberAPIException>(ex => ex.StatusCode == 429)
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
    );

var result = await retryPolicy.ExecuteAsync(async () =>
{
    return await client.CheckAsync(domain: "example.com");
});
```

## Configuration

### Custom Base URL

```csharp
var client = new CyberAPIClient(
    apiKey: "your_key",
    baseUrl: "https://custom-endpoint.example.com/api/v1"
);
```

### Custom Timeout

```csharp
var client = new CyberAPIClient(
    apiKey: "your_key",
    timeout: 30  // 30 seconds
);
```

## Response Structure

The `ThreatResponse` class provides strongly-typed access to all response fields:

```csharp
public class ThreatResponse
{
    public int RiskScore { get; set; }              // 0-100 risk score
    public string Verdict { get; set; }             // clean, low, medium, high, malicious
    public List<string> Signals { get; set; }       // Detected threat signals
    public GeoLocation GeoLocation { get; set; }    // IP geolocation data
    public Dictionary<string, object> DnsSecurity { get; set; }
    public Dictionary<string, object> SslInfo { get; set; }
    public Dictionary<string, object> TechStack { get; set; }
    public bool Cached { get; set; }                // Whether result was cached
    public int ProcessTimeMs { get; set; }          // Processing time
}
```

## Rate Limits

Different tiers have different rate limits:

- **Demo**: 10 requests/day
- **Startup**: 10,000 requests/day
- **Business**: 100,000 requests/day

The API returns rate limit information in response headers:
- `X-RateLimit-Limit`: Your daily limit
- `X-RateLimit-Remaining`: Remaining requests today
- `X-Cache`: Whether response was cached (HIT/MISS)

## Async/Await Best Practices

Always use `async`/`await` for API calls:

```csharp
// ✅ Good
var result = await client.CheckAsync(domain: "example.com");

// ❌ Bad - blocks the thread
var result = client.CheckAsync(domain: "example.com").Result;
```

## Dependency Injection

Register the client in your DI container:

```csharp
// Startup.cs or Program.cs
services.AddSingleton<CyberAPIClient>(sp => 
    new CyberAPIClient(Configuration["CyberAPI:ApiKey"])
);

// Usage in a controller or service
public class ThreatCheckService
{
    private readonly CyberAPIClient _client;
    
    public ThreatCheckService(CyberAPIClient client)
    {
        _client = client;
    }
    
    public async Task<bool> IsSafeDomain(string domain)
    {
        var result = await _client.CheckAsync(domain: domain);
        return result.RiskScore < 40;
    }
}
```

## Requirements

- .NET Standard 2.0 or higher
- .NET Core 2.0+ / .NET Framework 4.6.1+ / .NET 5+
- System.Text.Json 6.0.0+

## Support

- 📚 [API Documentation](https://threats.cyberapi.io/docs)
- 📧 [Email Support](mailto:inbox@cyberapi.io)

## License

MIT License - See repository for details
