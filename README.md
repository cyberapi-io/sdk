# CyberAPI SDKs

Official client libraries for CyberAPI Threat Intelligence API.

Get your API key at [https://www.cyberapi.io](https://www.cyberapi.io)

## Available SDKs

### Python
- **Location**: [/sdk/python/](https://github.com/cyberapi-io/sdk/tree/main/python)
- **Features**: Simple, async-ready, type hints
- **Installation**: Copy `cyberapi.py` to your project
- **Requirements**: Python 3.6+, requests library

### JavaScript/Node.js
- **Location**: [/sdk/javascript/](https://github.com/cyberapi-io/sdk/tree/main/javascript)
- **Features**: Works in Node.js and browsers, Promise-based
- **Installation**: Copy `cyberapi.js` to your project
- **Requirements**: Node.js 12+ or modern browser

### PHP
- **Location**: [/sdk/php/](https://github.com/cyberapi-io/sdk/tree/main/php)
- **Features**: PSR-4 compatible, cURL-based, error handling
- **Installation**: Composer or manual include
- **Requirements**: PHP 7.4+, cURL extension

### Go
- **Location**: [/sdk/go/](https://github.com/cyberapi-io/sdk/tree/main/go)
- **Features**: Idiomatic Go, context support, strongly-typed
- **Installation**: `go get github.com/cyberapi-io/sdk/go`
- **Requirements**: Go 1.16+

### Ruby
- **Location**: [/sdk/ruby/](https://github.com/cyberapi-io/sdk/tree/main/ruby)
- **Features**: Idiomatic Ruby, comprehensive error handling
- **Installation**: Bundler or manual include
- **Requirements**: Ruby 2.5+

### .NET
- **Location**: [/sdk/dotnet/](https://github.com/cyberapi-io/sdk/tree/main/dotnet)
- **Features**: Async/await, strongly-typed, .NET Standard 2.0
- **Installation**: NuGet package or project reference
- **Requirements**: .NET Standard 2.0+ / .NET Core 2.0+ / .NET 5+

## Feature Comparison

| Feature | Python | JavaScript | PHP | Go | Ruby | .NET |
|---------|--------|------------|-----|----|----- |------|
| Domain Check | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| IP Check | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Error Handling | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Type Safety | ⚠️ | ❌ | ❌ | ✅ | ❌ | ✅ |
| Async Support | ✅ | ✅ | ❌ | ✅ | ❌ | ✅ |
| Examples | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |

## Quick Start Example

All SDKs follow a similar pattern:

```python
# Python
from cyberapi import CyberAPI
client = CyberAPI(api_key="your_key")
result = client.check(domain="example.com")
```

```javascript
// JavaScript
const client = new CyberAPI("your_key");
const result = await client.check({ domain: "example.com" });
```

```php
// PHP
use CyberAPI\CyberAPI;
$client = new CyberAPI('your_key');
$result = $client->check('example.com');
```

```go
// Go
client := cyberapi.NewClient("your_key")
result, err := client.Check("example.com", "")
```

```ruby
# Ruby
client = CyberAPI::Client.new(api_key: "your_key")
result = client.check(domain: "example.com")
```

```csharp
// C# (.NET)
using var client = new CyberAPIClient("your_key");
var result = await client.CheckAsync(domain: "example.com");
```

## Contributing

Want to contribute an SDK improvement or add support for another language? 

- 📧 Email us at [inbox@cyberapi.io](mailto:inbox@cyberapi.io)
- 🔧 Submit a PR on [GitHub](https://github.com/cyberapi-io/sdk)

## Documentation

- 📚 Full API documentation: [www.cyberapi.io/docs](https://docs.cyberapi.io)
- 🌐 Main website: [cyberapi.io](https://www.cyberapi.io)

## License

MIT License - See individual SDK directories for details

