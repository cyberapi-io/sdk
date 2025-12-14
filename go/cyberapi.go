package cyberapi

import (
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"net/url"
	"strings"
	"time"
)

const DefaultBaseURL = "https://threats.cyberapi.io/api/v1"

type Client struct {
	APIKey     string
	BaseURL    string
	HTTPClient *http.Client
}

func NewClient(apiKey string) *Client {
	return &Client{
		APIKey:  apiKey,
		BaseURL: DefaultBaseURL,
		HTTPClient: &http.Client{
			Timeout: 10 * time.Second,
		},
	}
}

type ThreatResponse struct {
	RiskScore int                    `json:"risk_score"`
	Verdict   string                 `json:"verdict"`
	Geo       map[string]interface{} `json:"geo,omitempty"`
	Signals   []string               `json:"signals,omitempty"`
	Details   map[string]interface{} `json:"details,omitempty"`
}

func (c *Client) Check(domain, ip string) (*ThreatResponse, error) {
	if domain == "" && ip == "" {
		return nil, fmt.Errorf("either domain or ip must be provided")
	}

	params := url.Values{}
	if domain != "" {
		params.Add("domain", domain)
	}
	if ip != "" {
		params.Add("ip", ip)
	}

	reqURL := fmt.Sprintf("%s/check?%s", strings.TrimRight(c.BaseURL, "/"), params.Encode())

	req, err := http.NewRequest("GET", reqURL, nil)
	if err != nil {
		return nil, err
	}

	req.Header.Set("X-API-Key", c.APIKey)
	req.Header.Set("Accept", "application/json")
	req.Header.Set("User-Agent", "CyberAPI-Go/1.0.0")

	resp, err := c.HTTPClient.Do(req)
	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, err
	}

	if resp.StatusCode >= 400 {
		var errResp map[string]interface{}
		if err := json.Unmarshal(body, &errResp); err == nil {
			if detail, ok := errResp["detail"].(string); ok {
				return nil, fmt.Errorf("api error: %s", detail)
			}
		}
		return nil, fmt.Errorf("http error: %d", resp.StatusCode)
	}

	var result ThreatResponse
	if err := json.Unmarshal(body, &result); err != nil {
		return nil, err
	}

	return &result, nil
}
