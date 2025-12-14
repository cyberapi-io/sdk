# CyberAPI Go SDK

Official Go client for [CyberAPI](https://www.cyberapi.io) Threat Intelligence API.

## Installation

```bash
go get github.com/cyberapi-io/sdk/go/cyberapi
```

## Quick Start

```go
package main

import (
	"fmt"
	"log"
	
	"github.com/cyberapi-io/sdk/go/cyberapi"
)

func main() {
	client := cyberapi.NewClient("your_api_key_here")
	
	result, err := client.Check("example.com", "")
	if err != nil {
		log.Fatal(err)
	}
	
	fmt.Printf("Risk Score: %d\n", result.RiskScore)
	fmt.Printf("Verdict: %s\n", result.Verdict)
}
```

## Usage Examples

### Basic Domain Check

```go
package main

import (
	"fmt"
	"log"
	"os"
	
	"github.com/cyberapi-io/sdk/go/cyberapi"
)

func main() {
	apiKey := os.Getenv("CYBERAPI_KEY")
	if apiKey == "" {
		apiKey = "demo"
	}
	
	client := cyberapi.NewClient(apiKey)
	
	result, err := client.Check("suspicious-site.com", "")
	if err != nil {
		log.Fatalf("Error: %v", err)
	}
	
	fmt.Printf("Domain: %s\n", result.Domain)
	fmt.Printf("Risk Score: %d/100\n", result.RiskScore)
	fmt.Printf("Verdict: %s\n", result.Verdict)
	
	if len(result.Signals) > 0 {
		fmt.Println("\nThreat Signals:")
		for _, signal := range result.Signals {
			fmt.Printf("  ⚠️  %s\n", signal)
		}
	} else {
		fmt.Println("\n✅ No threat signals detected")
	}
}
```

### IP Address Check

```go
result, err := client.Check("", "8.8.8.8")
if err != nil {
	log.Fatal(err)
}

geo := result.Geo
fmt.Printf("IP: %s\n", geo["ip"])
fmt.Printf("Country: %s\n", geo["country"])
fmt.Printf("City: %s\n", geo["city"])
fmt.Printf("ISP: %s\n", geo["isp"])
```

### Check Both Domain and IP

```go
result, err := client.Check("example.com", "93.184.216.34")
if err != nil {
	log.Fatal(err)
}

fmt.Printf("Domain: %s\n", result.Domain)
fmt.Printf("IP: %s\n", result.IP)
fmt.Printf("Combined Risk Score: %d\n", result.RiskScore)
```

### Quick Malicious Check

```go
func isMalicious(client *cyberapi.Client, domain string) (bool, error) {
	result, err := client.Check(domain, "")
	if err != nil {
		return false, err
	}
	return result.RiskScore >= 80, nil
}

malicious, err := isMalicious(client, "phishing-site.com")
if err != nil {
	log.Fatal(err)
}

if malicious {
	fmt.Println("⚠️  Warning: Malicious domain detected!")
} else {
	fmt.Println("✅ Domain appears safe")
}
```

## Error Handling

The SDK provides comprehensive error handling:

### Basic Error Handling

```go
result, err := client.Check("example.com", "")
if err != nil {
	log.Printf("Error checking domain: %v", err)
	return
}

fmt.Printf("Risk Score: %d\n", result.RiskScore)
```

### Handling Specific HTTP Errors

```go
import (
	"errors"
	"fmt"
	"strings"
)

result, err := client.Check("example.com", "")
if err != nil {
	errMsg := err.Error()
	
	switch {
	case strings.Contains(errMsg, "403"):
		fmt.Println("Invalid API key. Please check your credentials.")
	case strings.Contains(errMsg, "429"):
		fmt.Println("Rate limit exceeded. Please wait before retrying.")
		time.Sleep(60 * time.Second)
		// Retry logic here
	case strings.Contains(errMsg, "500"), strings.Contains(errMsg, "503"):
		fmt.Println("Server error. Please try again later.")
	default:
		fmt.Printf("Error: %v\n", err)
	}
	return
}
```

### Retry Logic with Exponential Backoff

```go
import (
	"math"
	"strings"
	"time"
)

func checkWithRetry(client *cyberapi.Client, domain string, maxRetries int) (*cyberapi.ThreatResponse, error) {
	var result *cyberapi.ThreatResponse
	var err error
	
	for attempt := 0; attempt < maxRetries; attempt++ {
		result, err = client.Check(domain, "")
		if err == nil {
			return result, nil
		}
		
		// Retry on rate limit errors
		if strings.Contains(err.Error(), "429") && attempt < maxRetries-1 {
			waitTime := time.Duration(math.Pow(2, float64(attempt))) * time.Second
			fmt.Printf("Rate limited. Waiting %v before retry...\n", waitTime)
			time.Sleep(waitTime)
			continue
		}
		
		return nil, err
	}
	
	return nil, fmt.Errorf("max retries exceeded: %w", err)
}

// Usage
result, err := checkWithRetry(client, "example.com", 3)
if err != nil {
	log.Fatalf("Failed after retries: %v", err)
}
fmt.Printf("Success: %s\n", result.Verdict)
```

## Context Support

### Using Context for Timeouts

```go
import (
	"context"
	"time"
)

// Create a client with custom HTTP client
httpClient := &http.Client{
	Timeout: 30 * time.Second,
}

client := &cyberapi.Client{
	APIKey:     "your_key",
	BaseURL:    cyberapi.DefaultBaseURL,
	HTTPClient: httpClient,
}

// Use context with timeout
ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
defer cancel()

// Note: Current SDK doesn't support context, but you can extend it:
// result, err := client.CheckWithContext(ctx, "example.com", "")
```

### Cancellation Support

```go
ctx, cancel := context.WithCancel(context.Background())

// Cancel after 5 seconds
go func() {
	time.Sleep(5 * time.Second)
	cancel()
}()

// This would use context if SDK supported it
// result, err := client.CheckWithContext(ctx, "example.com", "")
```

## Concurrent Checking

### Check Multiple Domains Concurrently

```go
import (
	"sync"
)

func checkDomainsConcurrently(client *cyberapi.Client, domains []string) map[string]*cyberapi.ThreatResponse {
	results := make(map[string]*cyberapi.ThreatResponse)
	var mu sync.Mutex
	var wg sync.WaitGroup
	
	for _, domain := range domains {
		wg.Add(1)
		go func(d string) {
			defer wg.Done()
			
			result, err := client.Check(d, "")
			if err != nil {
				log.Printf("Error checking %s: %v", d, err)
				return
			}
			
			mu.Lock()
			results[d] = result
			mu.Unlock()
		}(domain)
	}
	
	wg.Wait()
	return results
}

// Usage
domains := []string{"example.com", "test.com", "demo.org"}
results := checkDomainsConcurrently(client, domains)

for domain, result := range results {
	fmt.Printf("%s: Risk Score %d\n", domain, result.RiskScore)
}
```

### Rate-Limited Concurrent Checking

```go
import (
	"golang.org/x/time/rate"
)

func checkDomainsWithRateLimit(client *cyberapi.Client, domains []string, rps int) {
	limiter := rate.NewLimiter(rate.Limit(rps), 1)
	
	for _, domain := range domains {
		// Wait for rate limiter
		limiter.Wait(context.Background())
		
		go func(d string) {
			result, err := client.Check(d, "")
			if err != nil {
				log.Printf("Error: %v", err)
				return
			}
			fmt.Printf("%s: %d\n", d, result.RiskScore)
		}(domain)
	}
}

// Check 10 domains per second
checkDomainsWithRateLimit(client, domains, 10)
```

## Configuration

### Custom Base URL

```go
client := &cyberapi.Client{
	APIKey:  "your_key",
	BaseURL: "https://custom-endpoint.example.com/api/v1",
	HTTPClient: &http.Client{
		Timeout: 10 * time.Second,
	},
}
```

### Custom HTTP Client

```go
import (
	"crypto/tls"
	"net/http"
)

// Custom HTTP client with custom transport
transport := &http.Transport{
	TLSClientConfig: &tls.Config{
		MinVersion: tls.VersionTLS12,
	},
	MaxIdleConns:        100,
	MaxIdleConnsPerHost: 10,
}

client := &cyberapi.Client{
	APIKey:  "your_key",
	BaseURL: cyberapi.DefaultBaseURL,
	HTTPClient: &http.Client{
		Transport: transport,
		Timeout:   30 * time.Second,
	},
}
```

## Response Structure

```go
type ThreatResponse struct {
	RiskScore int                    `json:"risk_score"`  // 0-100 risk score
	Verdict   string                 `json:"verdict"`     // clean, low, medium, high, malicious
	Geo       map[string]interface{} `json:"geo,omitempty"`
	Signals   []string               `json:"signals,omitempty"`
	Details   map[string]interface{} `json:"details,omitempty"`
}
```

## Testing

### Unit Testing with Mock

```go
import (
	"testing"
)

type MockClient struct {
	CheckFunc func(domain, ip string) (*cyberapi.ThreatResponse, error)
}

func (m *MockClient) Check(domain, ip string) (*cyberapi.ThreatResponse, error) {
	return m.CheckFunc(domain, ip)
}

func TestDomainCheck(t *testing.T) {
	mock := &MockClient{
		CheckFunc: func(domain, ip string) (*cyberapi.ThreatResponse, error) {
			return &cyberapi.ThreatResponse{
				RiskScore: 25,
				Verdict:   "low",
			}, nil
		},
	}
	
	result, err := mock.Check("example.com", "")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	
	if result.RiskScore != 25 {
		t.Errorf("expected risk score 25, got %d", result.RiskScore)
	}
}
```

### Integration Testing

```go
func TestIntegration(t *testing.T) {
	if testing.Short() {
		t.Skip("skipping integration test")
	}
	
	apiKey := os.Getenv("CYBERAPI_KEY")
	if apiKey == "" {
		t.Skip("CYBERAPI_KEY not set")
	}
	
	client := cyberapi.NewClient(apiKey)
	result, err := client.Check("example.com", "")
	
	if err != nil {
		t.Fatalf("API call failed: %v", err)
	}
	
	if result.RiskScore < 0 || result.RiskScore > 100 {
		t.Errorf("invalid risk score: %d", result.RiskScore)
	}
}
```

## Rate Limits

Different tiers have different rate limits:

- **Demo**: 10 requests/day
- **Startup**: 10,000 requests/day
- **Business**: 100,000 requests/day

### Respecting Rate Limits

```go
import (
	"time"
)

// Simple rate limiting
func checkWithDelay(client *cyberapi.Client, domains []string, delayMs int) {
	for _, domain := range domains {
		result, err := client.Check(domain, "")
		if err != nil {
			log.Printf("Error: %v", err)
			continue
		}
		
		fmt.Printf("%s: %d\n", domain, result.RiskScore)
		time.Sleep(time.Duration(delayMs) * time.Millisecond)
	}
}

// 100ms delay between requests
checkWithDelay(client, domains, 100)
```

## Troubleshooting

### SSL Certificate Verification Issues

```go
import (
	"crypto/tls"
)

// For development only - disable SSL verification
transport := &http.Transport{
	TLSClientConfig: &tls.Config{
		InsecureSkipVerify: true, // NOT recommended for production
	},
}

client := &cyberapi.Client{
	APIKey:  "your_key",
	BaseURL: cyberapi.DefaultBaseURL,
	HTTPClient: &http.Client{
		Transport: transport,
	},
}
```

**Warning**: Never disable SSL verification in production!

### Timeout Issues

```go
// Increase timeout for slow connections
client := cyberapi.NewClient("your_key")
client.HTTPClient.Timeout = 60 * time.Second
```

### Debug Logging

```go
import (
	"log"
	"net/http"
	"net/http/httputil"
)

// Create HTTP client with debug logging
type debugTransport struct {
	Transport http.RoundTripper
}

func (t *debugTransport) RoundTrip(req *http.Request) (*http.Response, error) {
	// Log request
	reqDump, _ := httputil.DumpRequestOut(req, true)
	log.Printf("Request:\n%s", reqDump)
	
	// Execute request
	resp, err := t.Transport.RoundTrip(req)
	if err != nil {
		return nil, err
	}
	
	// Log response
	respDump, _ := httputil.DumpResponse(resp, true)
	log.Printf("Response:\n%s", respDump)
	
	return resp, nil
}

// Use debug transport
client := cyberapi.NewClient("your_key")
client.HTTPClient.Transport = &debugTransport{
	Transport: http.DefaultTransport,
}
```

## Requirements

- Go 1.16 or higher
- No external dependencies (uses standard library only)

### Checking Requirements

```bash
go version
```

## Support

- 📚 [API Documentation](https://www.cyberapi.io/docs)
- 📧 [Email Support](mailto:inbox@cyberapi.io)

## License

MIT License - See repository for details
