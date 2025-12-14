# CyberAPI PHP SDK

Official PHP client for [CyberAPI](https://www.cyberapi.io) Threat Intelligence API.

## Installation

### Via Composer

```bash
composer require cyberapi/cyberapi-php
```

### Manual Installation

Download and include `CyberAPI.php` directly in your project:

```php
require_once 'path/to/CyberAPI.php';
```

## Quick Start

```php
<?php
require_once 'vendor/autoload.php'; // If using Composer

use CyberAPI\CyberAPI;

$client = new CyberAPI('your_api_key_here');

// Check a domain
$result = $client->check('example.com');
echo "Risk Score: " . $result['risk_score'] . "\n";
echo "Verdict: " . $result['verdict'] . "\n";
```

## Usage Examples

### Basic Domain Check

```php
<?php
use CyberAPI\CyberAPI;

$client = new CyberAPI($_ENV['CYBERAPI_KEY']);

try {
    $result = $client->check('suspicious-site.com');
    
    echo "Domain: " . $result['domain'] . "\n";
    echo "Risk Score: " . $result['risk_score'] . "/100\n";
    echo "Verdict: " . $result['verdict'] . "\n";
    
    if (!empty($result['signals'])) {
        echo "\nThreat Signals:\n";
        foreach ($result['signals'] as $signal) {
            echo "  ⚠️  $signal\n";
        }
    }
} catch (Exception $e) {
    echo "Error: " . $e->getMessage() . "\n";
}
```

### IP Address Check

```php
$result = $client->check(null, '8.8.8.8');

$geo = $result['geo_location'];
echo "IP: " . $geo['ip'] . "\n";
echo "Country: " . $geo['country'] . "\n";
echo "City: " . $geo['city'] . "\n";
echo "ISP: " . $geo['isp'] . "\n";
```

### Check Both Domain and IP

```php
$result = $client->check('example.com', '93.184.216.34');

echo "Domain: " . $result['domain'] . "\n";
echo "IP: " . $result['ip'] . "\n";
echo "Combined Risk Score: " . $result['risk_score'] . "\n";
```

### Quick Malicious Check

```php
function isMalicious($client, $domain) {
    $result = $client->check($domain);
    return $result['risk_score'] >= 80;
}

if (isMalicious($client, 'phishing-site.com')) {
    echo "⚠️  Warning: Malicious domain detected!\n";
} else {
    echo "✅ Domain appears safe\n";
}
```

## Error Handling

The SDK provides comprehensive error handling for various scenarios:

### Basic Error Handling

```php
try {
    $result = $client->check('example.com');
    print_r($result);
} catch (InvalidArgumentException $e) {
    // Missing domain/ip parameter
    echo "Invalid input: " . $e->getMessage() . "\n";
} catch (Exception $e) {
    // API errors, network errors, etc.
    echo "Error: " . $e->getMessage() . "\n";
    
    // Check HTTP status code if available
    if ($e->getCode() > 0) {
        echo "HTTP Status: " . $e->getCode() . "\n";
    }
}
```

### Handling Specific HTTP Errors

```php
try {
    $result = $client->check('example.com');
} catch (Exception $e) {
    $statusCode = $e->getCode();
    
    switch ($statusCode) {
        case 403:
            echo "Invalid API key. Please check your credentials.\n";
            break;
        case 429:
            echo "Rate limit exceeded. Please wait before retrying.\n";
            sleep(60);
            // Retry logic here
            break;
        case 500:
        case 503:
            echo "Server error. Please try again later.\n";
            break;
        default:
            echo "Error: " . $e->getMessage() . "\n";
    }
}
```

### Retry Logic with Exponential Backoff

```php
function checkWithRetry($client, $domain, $maxRetries = 3) {
    $attempt = 0;
    
    while ($attempt < $maxRetries) {
        try {
            return $client->check($domain);
        } catch (Exception $e) {
            $attempt++;
            
            if ($e->getCode() == 429 && $attempt < $maxRetries) {
                $waitTime = pow(2, $attempt); // Exponential backoff
                echo "Rate limited. Waiting {$waitTime}s before retry...\n";
                sleep($waitTime);
            } else {
                throw $e;
            }
        }
    }
    
    throw new Exception("Max retries exceeded");
}

try {
    $result = checkWithRetry($client, 'example.com');
    echo "Success: " . $result['verdict'] . "\n";
} catch (Exception $e) {
    echo "Failed after retries: " . $e->getMessage() . "\n";
}
```

## Configuration

### Custom Base URL

```php
$client = new CyberAPI(
    'your_api_key',
    'https://custom-endpoint.example.com/api/v1'
);
```

### Custom Timeout

```php
$client = new CyberAPI(
    'your_api_key',
    'https://www.cyberapi.io/api/v1',
    30  // 30 seconds timeout
);
```

## Response Structure

```php
Array
(
    [risk_score] => 45              // 0-100 risk score
    [verdict] => medium             // clean, low, medium, high, malicious
    [domain] => example.com
    [ip] => 93.184.216.34
    [signals] => Array              // Detected threat signals
    (
        [0] => missing_mx_records
        [1] => ip_is_hosting
    )
    [geo_location] => Array         // IP geolocation data
    (
        [country] => US
        [city] => Mountain View
        [isp] => Google LLC
    )
    [dns_security] => Array(...)    // DNS analysis
    [ssl_info] => Array(...)        // SSL certificate info
    [tech_stack] => Array(...)      // Detected technologies
    [cached] => false               // Whether result was cached
    [process_time_ms] => 234        // Processing time
)
```

## Rate Limits

Different tiers have different rate limits:

- **Demo**: 10 requests/day
- **Startup**: 10,000 requests/day
- **Business**: 100,000 requests/day

### Checking Rate Limit Status

The API returns rate limit information in response headers. You can access these using cURL info:

```php
// Note: The current SDK doesn't expose headers, but you can extend it:
class CyberAPIExtended extends CyberAPI {
    public $lastHeaders = [];
    
    public function check($domain = null, $ip = null) {
        // ... existing code ...
        
        // After curl_exec:
        $this->lastHeaders = [
            'X-RateLimit-Limit' => curl_getinfo($ch, CURLINFO_HEADER_OUT),
            'X-RateLimit-Remaining' => '...',
            'X-Cache' => '...'
        ];
        
        return $data;
    }
}
```

## Advanced Usage

### Batch Processing

```php
$domains = ['example.com', 'test.com', 'demo.org'];
$results = [];

foreach ($domains as $domain) {
    try {
        $results[$domain] = $client->check($domain);
        
        // Respect rate limits
        usleep(100000); // 100ms delay between requests
    } catch (Exception $e) {
        $results[$domain] = ['error' => $e->getMessage()];
    }
}

// Process results
foreach ($results as $domain => $result) {
    if (isset($result['error'])) {
        echo "$domain: Error - {$result['error']}\n";
    } else {
        echo "$domain: Risk Score {$result['risk_score']}\n";
    }
}
```

### Integration with Laravel

```php
// In a Laravel controller
namespace App\Http\Controllers;

use CyberAPI\CyberAPI;
use Illuminate\Http\Request;

class ThreatCheckController extends Controller
{
    protected $cyberapi;
    
    public function __construct()
    {
        $this->cyberapi = new CyberAPI(config('services.cyberapi.key'));
    }
    
    public function check(Request $request)
    {
        $request->validate([
            'domain' => 'required|string'
        ]);
        
        try {
            $result = $this->cyberapi->check($request->domain);
            return response()->json($result);
        } catch (\Exception $e) {
            return response()->json([
                'error' => $e->getMessage()
            ], $e->getCode() ?: 500);
        }
    }
}
```

## Troubleshooting

### cURL Error: SSL Certificate Problem

```php
// For development only - disable SSL verification
curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, false);
```

**Note**: Never disable SSL verification in production!

### Timeout Issues

If you're experiencing timeouts, increase the timeout value:

```php
$client = new CyberAPI('your_key', 'https://www.cyberapi.io/api/v1', 60);
```

### Memory Issues with Large Responses

For memory-constrained environments, process results immediately:

```php
$result = $client->check('example.com');

// Extract only what you need
$riskScore = $result['risk_score'];
$verdict = $result['verdict'];

// Unset large result to free memory
unset($result);
```

## Requirements

- PHP 7.4 or higher
- cURL extension
- JSON extension

### Checking Requirements

```bash
php -m | grep curl
php -m | grep json
php -v
```

## Support

- 📚 [API Documentation](https://www.cyberapi.io/docs)
- 📧 [Email Support](mailto:inbox@cyberapi.io)

## License

MIT License - See repository for details
