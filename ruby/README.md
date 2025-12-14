# CyberAPI Ruby SDK

Official Ruby client for [CyberAPI](https://threats.cyberapi.io) Threat Intelligence API.

## Installation

### Using Bundler

Add to your `Gemfile`:

```ruby
gem 'cyberapi', git: 'https://github.com/cyberapi-io/sdk', glob: 'ruby/*.gemspec'
```

### Manual Installation

Copy `cyberapi.rb` to your project:

```bash
curl -O https://raw.githubusercontent.com/cyberapi-io/sdk/main/ruby/cyberapi.rb
```

## Quick Start

```ruby
require_relative 'cyberapi'

# Initialize client
client = CyberAPI::Client.new(api_key: "your_api_key_here")

# Check a domain
result = client.check(domain: "example.com")
puts "Risk Score: #{result['risk_score']}"
puts "Verdict: #{result['verdict']}"

# Check an IP
result = client.check(ip: "1.2.3.4")
puts "Location: #{result['geo_location']['country']}"
```

## Usage Examples

### Basic Domain Check

```ruby
client = CyberAPI::Client.new(api_key: ENV['CYBERAPI_KEY'])

result = client.check(domain: "suspicious-site.com")

puts "Risk Score: #{result['risk_score']}"
puts "Verdict: #{result['verdict']}"
puts "Signals: #{result['signals'].join(', ')}"
```

### Quick Malicious Check

```ruby
if client.malicious?(domain: "phishing-site.com")
  puts "⚠️  Warning: Malicious domain detected!"
else
  puts "✅ Domain appears safe"
end
```

### Get Threat Signals

```ruby
signals = client.signals(domain: "example.com")
puts "Detected #{signals.length} threat signals:"
signals.each { |signal| puts "  - #{signal}" }
```

### IP Geolocation

```ruby
result = client.check(ip: "8.8.8.8")
geo = result['geo_location']

puts "IP: #{geo['ip']}"
puts "Country: #{geo['country']}"
puts "City: #{geo['city']}"
puts "ISP: #{geo['isp']}"
```

## Error Handling

The SDK provides specific error classes for different failure scenarios:

```ruby
begin
  result = client.check(domain: "example.com")
rescue CyberAPI::ClientError => e
  # 4xx errors (invalid request, auth failure, rate limit)
  puts "Client error: #{e.message} (HTTP #{e.status_code})"
rescue CyberAPI::ServerError => e
  # 5xx errors (server issues)
  puts "Server error: #{e.message} (HTTP #{e.status_code})"
rescue CyberAPI::Error => e
  # Other errors (network, parsing, etc.)
  puts "Error: #{e.message}"
end
```

### Common Error Scenarios

**Rate Limit Exceeded (429)**
```ruby
begin
  result = client.check(domain: "example.com")
rescue CyberAPI::ClientError => e
  if e.status_code == 429
    puts "Rate limit exceeded. Please wait before retrying."
    sleep 60
    retry
  end
end
```

**Invalid API Key (403)**
```ruby
begin
  client = CyberAPI::Client.new(api_key: "invalid_key")
  result = client.check(domain: "example.com")
rescue CyberAPI::ClientError => e
  if e.status_code == 403
    puts "Invalid API key. Please check your credentials."
  end
end
```

## Configuration

### Custom Base URL

```ruby
client = CyberAPI::Client.new(
  api_key: "your_key",
  base_url: "https://custom-endpoint.example.com/api/v1"
)
```

### Custom Timeout

```ruby
client = CyberAPI::Client.new(
  api_key: "your_key",
  timeout: 30  # 30 seconds
)
```

## Response Structure

```ruby
{
  "risk_score" => 45,           # 0-100 risk score
  "verdict" => "medium",        # clean, low, medium, high, malicious
  "signals" => [                # Array of detected threat signals
    "missing_mx_records",
    "ip_is_hosting"
  ],
  "geo_location" => {           # IP geolocation data
    "country" => "US",
    "city" => "Mountain View",
    "isp" => "Google LLC"
  },
  "dns_security" => {...},      # DNS analysis
  "ssl_info" => {...},          # SSL certificate info
  "tech_stack" => {...},        # Detected technologies
  "cached" => false,            # Whether result was cached
  "process_time_ms" => 234      # Processing time
}
```

## Rate Limits

Different tiers have different rate limits:

- **Demo**: 10 requests/day
- **Startup**: 10,000 requests/day
- **Business**: 100,000 requests/day

Monitor rate limit headers in responses:
- `X-RateLimit-Limit`: Your daily limit
- `X-RateLimit-Remaining`: Remaining requests today
- `X-Cache`: Whether response was cached (HIT/MISS)

## Requirements

- Ruby 2.5 or higher
- Standard library only (no external dependencies)

## Support

- 📚 [API Documentation](https://threats.cyberapi.io/docs)
- 📧 [Email Support](mailto:inbox@cyberapi.io)

## License

MIT License - See repository for details
